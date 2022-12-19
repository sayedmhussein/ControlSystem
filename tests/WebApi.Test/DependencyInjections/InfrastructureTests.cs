using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WeeControl.ApiApp.Domain.Interfaces;
using WeeControl.ApiApp.Infrastructure;
using Xunit;

namespace WeeControl.ApiApp.WebApi.Test.DependencyInjections;

public class InfrastructureTests
{
    [Fact]
    public void WhenAddingInfrastructure_EmailServiceObjectMustNotBeNull()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x.GetSection("ConnectionStrings")["EmailProvider"])
            .Returns("Connection String of Email Provider");

        var services = new ServiceCollection();
        services.AddInfrastructure(configMock.Object);
        var provider = services.BuildServiceProvider();
        
        var service = provider.GetService<IEmailNotificationService>();

        Assert.NotNull(service);
    }
}