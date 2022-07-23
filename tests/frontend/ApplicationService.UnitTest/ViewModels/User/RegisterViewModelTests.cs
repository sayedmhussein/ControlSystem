using System.Net;
using System.Net.Http;
using WeeControl.Frontend.ApplicationService.Essential.ViewModels;
using WeeControl.SharedKernel.Essential.DataTransferObjects;
using WeeControl.SharedKernel.Essential.Interfaces;
using WeeControl.SharedKernel.RequestsResponses;
using UserModel = WeeControl.Frontend.ApplicationService.Essential.Models.UserModel;

namespace WeeControl.Frontend.ApplicationService.UnitTest.ViewModels.User;

public class RegisterViewModelTests : ViewModelTestsBase
{
    public RegisterViewModelTests() : base(nameof(UserLegacyViewModel))
    {
    }
    
    [Fact]
    public async void WhenSuccessResponseCode()
    {
        var vm = new UserLegacyViewModel(Mock.GetObject(HttpStatusCode.OK, GetResponseContent()), GetRegisterDto());

        await vm.RegisterAsync();

        Mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Shared.IndexPage, It.IsAny<bool>()));
    }
    
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.Conflict)]
    public async void WhenOtherResponseCode(HttpStatusCode code)
    {
        var vm = new UserLegacyViewModel(Mock.GetObject(code, GetResponseContent()), GetRegisterDto());

        await vm.RegisterAsync();
        
        Mock.AlertMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        Mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Shared.IndexPage, It.IsAny<bool>()), Times.Never);
        Assert.False(vm.IsLoading);
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("email@email.com", "", "")]
    [InlineData("", "username", "")]
    [InlineData("", "", "password")]
    public async void WhenInvalidProperties(string email, string username, string password)
    {
        var vm = new UserLegacyViewModel(Mock.GetObject(HttpStatusCode.OK, GetResponseContent()))
        {
            Email = email,
            Username = username,
            Password = password
        };

        await vm.RegisterAsync();
        
        Mock.AlertMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        Mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Shared.IndexPage, It.IsAny<bool>()), Times.Never);
        Assert.False(vm.IsLoading);
    }

    private UserModel GetRegisterDto()
    {
        return new UserModel()
        {
            TerritoryId = nameof(IUserModel.TerritoryId),
            FirstName = nameof(IUserModel.FirstName),
            LastName = nameof(IUserModel.LastName),
            Email = nameof(IUserModel.Email) + "@email.com",
            Username = nameof(IUserModel.Username),
            Password = nameof(IUserModel.Password),
            MobileNo = "0123456789",
            Nationality = "TST"
        };
    }

    private HttpContent GetResponseContent()
    {
        var dto =
            ResponseDto.Create<TokenDtoV1>(TokenDtoV1.Create("token", "name", "url"));
        return GetJsonContent(dto);
    }
}