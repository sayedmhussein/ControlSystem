using Microsoft.Extensions.DependencyInjection;
using Moq;
using WeeControl.ApiApp.Application.Interfaces;
using WeeControl.ApiApp.WebApi;
using WeeControl.Common.SharedKernel.Contexts.Temporary.User;
using WeeControl.Frontend.AppService;
using WeeControl.Frontend.AppService.Internals.Temporary.Interfaces;
using WeeControl.Frontend.AppService.Internals.Temporary.Models;
using Xunit;

namespace WeeControl.User.UserApplication.Test.Integration.Services.Essential;

public class UserServiceTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> factory;

    public UserServiceTests(CustomWebApplicationFactory<Startup> factory)
    {
        this.factory = factory;
    }

    #region PropertyIsAllowed()
    [Theory]
    [InlineData(nameof(RegisterCustomerDto.User.Username), "username", false)]
    [InlineData(nameof(RegisterCustomerDto.User.Username), "Username", false)]
    [InlineData(nameof(RegisterCustomerDto.User.Username), "username2", true)]
    public async void UsernameAllowedTest(string propertyName, string username, bool isAllowed)
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);

        var allowed = await helper.Service.PropertyIsAllowed(propertyName, username);

        Assert.Equal(isAllowed, allowed);
    }
    #endregion

    #region ChangeMyPassword()
    [Fact]
    public async void ChangeMyPassword_WhenSuccess()
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);
        await helper.Authorize("username", "password");

        await helper.Service.ChangeMyPassword(new PasswordChangeModel()
        {
            OldPassword = "password",
            NewPassword = "someNewPassword",
            ConfirmPassword = "someNewPassword"
        });
            
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.SplashPage, It.IsAny<bool>()), Times.Once);
    }
    
    [Fact]
    public async void ChangeMyPassword_WhenUnauthorized()
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);

        await helper.Service.ChangeMyPassword(new PasswordChangeModel()
        {
            OldPassword = "password",
            NewPassword = "someNewPassword",
            ConfirmPassword = "someNewPassword"
        });
            
        helper.DeviceMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.HomePage, It.IsAny<bool>()), Times.Never);
    }
    
    [Fact]
    public async void ChangeMyPassword_WhenBusinessNotAllow_InvalidPassword()
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);
        await helper.Authorize("username", "password");

        await helper.Service.ChangeMyPassword(new PasswordChangeModel()
        {
            OldPassword = "invalid password",
            NewPassword = "someNewPassword",
            ConfirmPassword = "someNewPassword"
        });
            
        helper.DeviceMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.HomePage, It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async void ChangeMyPassword_WhenUserIsLocked()
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                user.Suspend("This is for testing only");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);
        await helper.Authorize("username", "password");

        await helper.Service.ChangeMyPassword(new PasswordChangeModel()
        {
            OldPassword = "password",
            NewPassword = "someNewPassword",
            ConfirmPassword = "someNewPassword"
        });
            
        helper.DeviceMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.HomePage, It.IsAny<bool>()), Times.Never);
    }
    #endregion

    #region RequestPasswordReset()
    [Fact]
    public async void RequestPasswordReset_WhenSuccess()
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);

        await helper.Service.RequestPasswordReset(new PasswordResetModel()
        {
            Email = "email@email.com",
            Username = "username"
        });
            
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.UserPage, It.IsAny<bool>()), Times.Once);
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData("email@email.com", "")]
    [InlineData("", "username")]
    public async void RequestPasswordReset_WhenInvalidEmailAndUsernameMatchingOrNotExist(string email, string username)
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);

        await helper.Service.RequestPasswordReset(new PasswordResetModel()
        {
            Email = email,
            Username = username
        });
            
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.UserPage, It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async void RequestPasswordReset_WhenBusinessNotAllow_IsLockedUser()
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                user.Suspend("for testing");
                db.Users.Add(user);
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);

        await helper.Service.RequestPasswordReset(new PasswordResetModel()
        {
            Email = "username@email.com",
            Username = "username"
        });
            
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.UserPage, It.IsAny<bool>()), Times.Never);
    }
    #endregion

    #region Register()
    [Theory]
    [InlineData("someEmail@email.com", "someUsername", "0123456789")]
    public async void Register_WhenSuccess(string email, string username, string mobileNo)
    {
        var model = new CustomerRegisterModel
        {
            Personal =
            {
                FirstName = username,
                LastName = username,
                Nationality = "EGP"
            },
            User =
            {
                Email = email,
                Username = username,
                Password = "somePassword",
                PasswordConfirmation = "somePassword",
                MobileNo = mobileNo
            },
            Customer =
            {
                CountryCode = "EGP"
            }
        };

        using var helper = new TestHelper<IUserService>(factory.CreateClient());
        
        await helper.Service.Register(model);

        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.SplashPage, It.IsAny<bool>()), Times.AtLeastOnce);
    }
    
    [Theory]
    [InlineData("username@email.com", "username", "0123456")]
    [InlineData("username1@email.com1", "username", "0123456")]
    [InlineData("username@email.com", "username1", "0123456")]
    public async void Register_WhenBusinessNotAllow_ExistingUser(string email, string username, string mobileNo)
    {
        var httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IEssentialDbContext>();
                var user = TestHelper<object>.GetUserDboWithEncryptedPassword("username", "password");
                db.Users.Add(user);
                user.Suspend("for testing");
                db.SaveChanges();
            });
        }).CreateClient();
        
        using var helper = new TestHelper<IUserService>(httpClient);

        var model = new CustomerRegisterModel
        {
            Personal =
            {
                FirstName = username,
                LastName = username,
                Nationality = "EGP"
            },
            User =
            {
                Email = email,
                Username = username,
                Password = "somePassword",
                MobileNo = mobileNo
            },
            Customer =
            {
                CountryCode = "EGP"
            }
        };
        
        await helper.Service.Register(model);
            
        helper.DeviceMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        helper.DeviceMock.Verify(x => 
            x.NavigateToAsync(ApplicationPages.Essential.SplashPage, It.IsAny<bool>()), Times.Never);;
    }
    #endregion
}