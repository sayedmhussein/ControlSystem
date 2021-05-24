﻿using System;
using MySystem.Web.ClientService.ViewModels;
using Xunit;
using Moq;
using MySystem.Web.ClientService.Services;
using System.Threading.Tasks;
using MySystem.Web.Shared.Configuration.Models;
using Microsoft.Extensions.Logging;
using MySystem.Web.ClientService.Test.Tools;
using MySystem.Web.Shared.Dtos.V1;
using MySystem.Web.Shared.Dtos;

namespace MySystem.Web.ClientService.Test.Unit.ViewModels
{
    public class LoginViewModelTesters
    {
        #region Constructor
        [Fact]
        public void Constructor_WhenNullService_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new LoginViewModel(null));
        }
        #endregion

        #region Commands
        [Fact]
        public async void LoginCommand_WhenEmptyUsername_ThrowInvalidArgumentException()
        {
            var device = TestMocks.GetDeviceMock();
            //
            var service = new ClientServices(device.Object, new Mock<IApi>().Object, new Mock<ILogger<ClientServices>>().Object);

            var vm = new LoginViewModel(service)
            {
                Username = string.Empty,
                Password = "SomePassword"
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await vm.LoginCommand.ExecuteAsync(null));
        }

        [Fact]
        public async void LoginCommand_WhenNullPassword_ThrowInvalidArgumentException()
        {
            var device = TestMocks.GetDeviceMock();
            var api = TestMocks.ApiMock;
            var logger = TestMocks.LoggerMock;

            var service = new ClientServices(device.Object, api.Object, logger.Object);

            var vm = new LoginViewModel(service)
            {
                Username = "SomeUsername",
                Password = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await vm.LoginCommand.ExecuteAsync(null));
        }

        [Fact]
        public async void LoginCommand_WhenNoInternet_DisplayMessage()
        {
            var device = TestMocks.DeviceMock;
            device.Setup(x => x.Internet).Returns(false);
            var api = TestMocks.ApiMock;
            var logger = TestMocks.LoggerMock;
            var handler = TestMocks.GetHttpMessageHandlerMock(System.Net.HttpStatusCode.OK, new ResponseDto<string>("token").SerializeToJson());
            //
            var service = new ClientServices(device.Object, api.Object, logger.Object, handler.Object);
            service.SystemUnderTest = true;

            var vm = new LoginViewModel(service)
            {
                Username = "username",
                Password = "password"
            };

            await vm.LoginCommand.ExecuteAsync(new object());

            device.Verify(x => x.DisplayMessageAsync(IDevice.Message.NoInternet), Times.Once);
        }

        [Fact]
        public async void LoginCommand_WhenValidCredentials_OpenHomePage()
        {
            var device = TestMocks.DeviceMock;
            var api = TestMocks.ApiMock;
            var logger = TestMocks.LoggerMock;
            var handler = TestMocks.GetHttpMessageHandlerMock(System.Net.HttpStatusCode.OK, new ResponseDto<string>("token").SerializeToJson());
            //
            //var service = new ClientServices(device.Object, api.Object, logger.Object, handler.Object);
            var service = TestMocks.GetClientServicesMock(device.Object, api.Object, handler.Object);

            var vm = new LoginViewModel(service.Object)
            {
                Username = "username",
                Password = "password"
            };

            await vm.LoginCommand.ExecuteAsync(new object());

            device.Verify(x => x.NavigateToPageAsync("HomePage"), Times.Once);
        }

        [Fact]
        public async void LoginCommand_WhenInvalidCredentials_DisplayMessageAndNeverOpenHomePage()
        {
            var device = TestMocks.DeviceMock;
            var api = TestMocks.ApiMock;
            var logger = TestMocks.LoggerMock;
            var handler = TestMocks.GetHttpMessageHandlerMock(System.Net.HttpStatusCode.Unauthorized, new ResponseDto<string>("token").SerializeToJson());
            //
            var service = TestMocks.GetClientServicesMock(handler.Object);

            var vm = new LoginViewModel(service.Object)
            {
                Username = "username",
                Password = "password"
            };

            await vm.LoginCommand.ExecuteAsync(new object());

            device.Verify(x => x.NavigateToPageAsync("HomePage"), Times.Never);
            device.Verify(x => x.DisplayMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        #endregion


    }
}