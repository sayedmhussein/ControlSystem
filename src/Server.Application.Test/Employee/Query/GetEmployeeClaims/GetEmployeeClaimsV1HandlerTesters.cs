﻿using System;
using Xunit;
using Moq;
using MySystem.Application.Employee.Query.GetEmployeeClaims;
using MySystem.Application.Common.Interfaces;
using MySystem.SharedKernel.Interfaces;
using MySystem.SharedKernel.Services;
using Microsoft.Extensions.DependencyInjection;
using MySystem.Persistence;
using MediatR;
using MySystem.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using MySystem.Domain.EntityDbo.EmployeeSchema;
using System.Linq;

namespace MySystem.Application.Test.Employee.Query.GetEmployeeClaims
{
    public class GetEmployeeClaimsV1HandlerTesters : IDisposable
    {
        private readonly ISharedValues sharedValues = new SharedValues();
        private IMySystemDbContext dbContext;
        private Mock<ICurrentUserInfo> userInfoMock;
        private Mock<IMediator> mediatRMock;

        private GetEmployeeClaimsV1Handler handler;


        public GetEmployeeClaimsV1HandlerTesters()
        {
            dbContext = new ServiceCollection().AddPersistenceAsInMemory(new Random().NextDouble().ToString()).BuildServiceProvider().GetService<IMySystemDbContext>();
            userInfoMock = new Mock<ICurrentUserInfo>();

            mediatRMock = new Mock<IMediator>();

            handler = new GetEmployeeClaimsV1Handler(dbContext, userInfoMock.Object, sharedValues, mediatRMock.Object);
        }

        public void Dispose()
        {
            dbContext = null;
            userInfoMock = null;

            handler = null;
        }

        #region Constructor
        [Fact]
        public void WhenFirstParameterIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetEmployeeClaimsV1Handler(null, userInfoMock.Object, sharedValues, mediatRMock.Object));
        }

        [Fact]
        public void WhenSecondParameterIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetEmployeeClaimsV1Handler(dbContext, null, sharedValues, mediatRMock.Object));
        }

        [Fact]
        public void WhenThridParameterIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetEmployeeClaimsV1Handler(dbContext, userInfoMock.Object, null, mediatRMock.Object));
        }

        [Fact]
        public void WhenFourthParameterIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetEmployeeClaimsV1Handler(dbContext, userInfoMock.Object, sharedValues, null));
        }
        #endregion

        #region Query
        [Fact]
        public async void WhenQueryIsNull_ThrowArgumentNullExceptino()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.Handle(null, default));
        }

        [Fact]
        public async void WhenAllQueryPropertiesWhereSupplied_ThrowBadRequest()
        {
            var query = new GetEmployeeClaimsV1Query() { Username = "admin", Password = "admin", Device = "device", EmployeeId = Guid.NewGuid() };

            await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(query, default));
        }

        [Fact]
        public async void WhenNitherOfAnyQueryPropertiesWhereSupplied_ThrowBadRequest()
        {
            var query = new GetEmployeeClaimsV1Query();

            await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(query, default));
        }

        [Theory]
        [InlineData("username", "password", null)]
        [InlineData("username", "password", "")]
        [InlineData("username", null, "device")]
        [InlineData("username", "", "device")]
        [InlineData(null, "password", "device")]
        [InlineData("", "password", "device")]
        public async void WhenCombinationOfUsernameAndPasswordAndDeviceAreNotSuppliedTogether_ThrowBadRequestException(string username, string password, string device)
        {
            var query = new GetEmployeeClaimsV1Query() { Username = username, Password = password, Device = device };

            await Assert.ThrowsAsync<BadRequestException>(async () => await handler.Handle(query, default));
        }
        #endregion

        #region Security Username And Password
        [Theory]
        [InlineData("admin", "admin", true)]
        [InlineData("admin", "admin1", false)]
        [InlineData("admin1", "admin", false)]
        public async void UsernameAndPasswordTheoryTests(string username, string password, bool isCorrect)
        {
            var query = new GetEmployeeClaimsV1Query() { Username = username, Password = password, Device = "Device"};

            if (isCorrect)
            {
                var claims = await handler.Handle(query, default);
                Assert.Single(claims);
            }
            else
            {
                await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, default));
            }
        }

        [Fact]
        public async void WhenCorrectUsernameAndPasswordButUserHasLockArgument_ThrowNotAllowedException()
        {
            var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Username == "admin");
            employee.AccountLockArgument = "locked";
            await dbContext.SaveChangesAsync(default);

            var query = new GetEmployeeClaimsV1Query() { Username = "admin", Password = "admin", Device = "Device" };

            await Assert.ThrowsAsync<NotAllowedException>(async () => await handler.Handle(query, default));
        }
        #endregion

        #region Query Device
        [Fact]
        public async void WhenDeviceIsSuppliedButTokenDoNotCarryValidSessionId_ThrowNotFoundException()
        {
            var query = new GetEmployeeClaimsV1Query() { Device = "device" };

            await Assert.ThrowsAsync<NotAllowedException>(async () => await handler.Handle(query, default));
        }

        [Theory]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, false, true, true)]
        [InlineData(false, true, true, true)]
        public async void WhenDeviceOnlyIsSuppliedTheoryTests(bool isSessionActive, bool isSameDevice, bool isEmployeeNotLocked, bool throwNotAllowedException)
        {
            var device = "device";
            var query = new GetEmployeeClaimsV1Query() { Device = device};
            var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Username == "admin");

            var session = new EmployeeSessionDbo() { DeviceId = isSameDevice ? device : device + "other", EmployeeId = employee.Id };
            session.TerminationTs = isSessionActive ? null : DateTime.UtcNow;
            await dbContext.EmployeeSessions.AddAsync(session);
            await dbContext.SaveChangesAsync(default);

            userInfoMock.Setup(x => x.SessionId).Returns(session.Id);

            if (isEmployeeNotLocked == false)
            {
                employee.AccountLockArgument = "locked";
                await dbContext.SaveChangesAsync(default);
            }

            var handler = new GetEmployeeClaimsV1Handler(dbContext, userInfoMock.Object, sharedValues, mediatRMock.Object);

            if (throwNotAllowedException)
            {
                await Assert.ThrowsAsync<NotAllowedException>(async () => await handler.Handle(query, default));
            }
            else
            {
                var claims = await handler.Handle(query, default);
                Assert.True(claims.Count() > 1, "At least session claim with another stored clam must be returned.");
            }
        }
        #endregion

        #region Query EmployeeId
        [Fact]
        public async void WhenInvalidEmployeeIdIsSupplied_ThrowNotFoundException()
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, false, true, true)]
        [InlineData(false, true, true, true)]
        public async void WhenEmployeeIdIsSuppliedTheories(bool sameTerritory, bool hasCorrectClaimType, bool hasCorrectClaimTag, bool throwNotFoundException)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
