using System.Net;
using WeeControl.SharedKernel.DataTransferObjects.Authentication;
using WeeControl.SharedKernel.RequestsResponses;
using WeeControl.User.UserApplication.ViewModels.User;

namespace WeeControl.User.UserApplication.Test.ViewModels.User;

public class RegisterViewModelTests : ViewModelTestsBase
{
    public RegisterViewModelTests() : base(nameof(RegisterViewModel))
    {
    }
    
    [Fact]
    public async void WhenSuccess()
    {
        var content = GetJsonContent(new ResponseDto<TokenDtoV1>(TokenDtoV1.Create("token", "name", "url")));
        var vm = new RegisterViewModel(mock.GetObject(HttpStatusCode.OK, content))
        {
            Email = "email@email.com",
            Username = "username",
            Password = "password"
        };

        await vm.RegisterAsync();

        mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Home.Index, It.IsAny<bool>()));
    }
    
    [Fact]
    public async void WhenBadRequest()
    {
        var vm = new RegisterViewModel(mock.GetObject(HttpStatusCode.BadRequest, null!))
        {
            Email = "email@email.com",
            Username = "username",
            Password = "password"
        };

        await vm.RegisterAsync();
        
        mock.AlertMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Home.Index, It.IsAny<bool>()), Times.Never);
        Assert.False(vm.IsLoading);
    }
    
    [Fact]
    public async void WhenConflict()
    {
        var vm = new RegisterViewModel(mock.GetObject(HttpStatusCode.Conflict, null!))
        {
            Email = "email@email.com",
            Username = "username",
            Password = "password"
        };

        await vm.RegisterAsync();
        
        mock.AlertMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Home.Index, It.IsAny<bool>()), Times.Never);
        Assert.False(vm.IsLoading);
    }
    
    [Fact]
    public async void WhenServerCommunicationError()
    {
        var vm = new RegisterViewModel(mock.GetObject(HttpStatusCode.BadGateway, null!))
        {
            Email = "email@email.com",
            Username = "username",
            Password = "password"
        };

        await vm.RegisterAsync();
        
        mock.AlertMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Home.Index, It.IsAny<bool>()), Times.Never);
        Assert.False(vm.IsLoading);
    }
    
    [Theory]
    [InlineData("", "", "")]
    [InlineData("email@email.com", "", "")]
    [InlineData("", "username", "")]
    [InlineData("", "", "password")]
    public async void WhenInvalidProperties(string email, string username, string password)
    {
        var vm = new RegisterViewModel(mock.GetObject(HttpStatusCode.OK, null!))
        {
            Email = email,
            Username = username,
            Password = password
        };

        await vm.RegisterAsync();
        
        mock.AlertMock.Verify(x => x.DisplayAlert(It.IsAny<string>()));
        mock.NavigationMock.Verify(x => x.NavigateToAsync(Pages.Home.Index, It.IsAny<bool>()), Times.Never);
        Assert.False(vm.IsLoading);
    }
}