﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MySystem.Application.Common.Interfaces;
using Xunit;

namespace MySystem.Infrastructure.Test
{
    public class DependencyInjectionTesters
    {
        [Fact]
        public void WhenAddingInfrastructure_EmailServiceObjectMustNotBeNull()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["Notification:EmailConfigurationString"]).Returns("MySystem.Api.UnitTest.Controllers.V1.CredentialsController");

            var services = new ServiceCollection();
            services.AddInfrastructure(configMock.Object);
            var provider = services.BuildServiceProvider();
            var service = provider.GetService<IEmailNotificationService>();

            Assert.NotNull(service);
        }
    }
}
