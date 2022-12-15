using System.Net;
using System.Net.Http.Json;
using WeeControl.Common.SharedKernel;
using WeeControl.Common.SharedKernel.DataTransferObjects.User;
using WeeControl.Common.SharedKernel.RequestsResponses;
using WeeControl.Frontend.AppService.AppInterfaces;
using WeeControl.Frontend.AppService.AppModels;
using WeeControl.Frontend.AppService.Contexts.Essential.Interfaces;
using WeeControl.Frontend.AppService.Contexts.Essential.Models;
using WeeControl.Frontend.AppService.Interfaces;

namespace WeeControl.Frontend.AppService.Contexts.Essential.Services;

internal class UserService : IUserService
{
    private readonly IDevice device;
    private readonly IServerOperation server;
    private readonly IAuthorizationService userAuthorizationService;
    
    public IEnumerable<CountryModel> Countries { get; }
    public string GreetingMessage { get; private set; }
    public bool IsEmployee { get; private set; }
    public bool IsCustomer { get; private set; }
    public List<MenuItemModel> MenuItems { get; }
    public List<HomeFeedModel> FeedsList { get; }
    public List<HomeNotificationModel> NotificationsList { get; }

    public UserService(IDevice device, IServerOperation server, IPersistedLists persistedLists, IAuthorizationService userAuthorizationService)
    {
        this.device = device;
        this.server = server;
        this.userAuthorizationService = userAuthorizationService;

        GreetingMessage = "Hello ";
        Countries = persistedLists.Countries;
        MenuItems = new List<MenuItemModel>();
        FeedsList = new List<HomeFeedModel>();
        NotificationsList = new List<HomeNotificationModel>();
    }
    
    public async Task<bool> PropertyIsAllowed(string propertyName, string username)
    {
        HttpRequestMessage message = new()
        {
            RequestUri = 
                new Uri(
                    device.Server.GetFullAddress(Api.Essential.Customer.EndPoints.Service.Customer +
                                                 "/" + propertyName+
                                                 "/" + username
                    )),
            Version = new Version("1.0"),
            Method = HttpMethod.Head,
        };
        
        var responseMessage = await server.Send<object>(message, null);

        return responseMessage.IsSuccessStatusCode;
    }

    public async Task Init()
    {
        if (await userAuthorizationService.IsAuthorized())
        {
            await Refresh();
        }

        await device.Navigation.NavigateToAsync(ApplicationPages.Essential.HomePage, forceLoad:true);
    }

    public async Task Refresh()
    {
        await SetupMenuAsync();
        
        GreetingMessage = "Hello " + await device.Storage.GetAsync(nameof(AuthenticationResponseDto.FullName));
        var claims = await device.Security.GetClaimsAsync();
        if (claims.FirstOrDefault(x => x.Type == ClaimsValues.ClaimTypes.Territory)?.Value is not null)
        {
            IsEmployee = true;
        }
            
        if (claims.FirstOrDefault(x => x.Type == ClaimsValues.ClaimTypes.Country)?.Value is not null)
        {
            IsCustomer = true;
        }
        
        NotificationsList.Clear();
    }

    public async Task Register(CustomerRegisterModel registerModel)
    {
        await RegisterAsync(registerModel);
    }

    public async Task RequestPasswordReset(PasswordResetModel passwordResetModel)
    {
        if (string.IsNullOrWhiteSpace(passwordResetModel.Email) || string.IsNullOrWhiteSpace(passwordResetModel.Username))
        {
            await device.Alert.DisplayAlert("Please enter your email and username first!");
            return;
        }
        
        await ProcessPasswordReset(passwordResetModel);
    }

    public async Task ChangeMyPassword(PasswordChangeModel passwordChangeModel)
    {
        if (string.IsNullOrWhiteSpace(passwordChangeModel.OldPassword) ||
            string.IsNullOrWhiteSpace(passwordChangeModel.NewPassword) ||
            string.IsNullOrWhiteSpace(passwordChangeModel.ConfirmPassword) ||
            passwordChangeModel.NewPassword != passwordChangeModel.ConfirmPassword)
        {
            await device.Alert.DisplayAlert("Invalid Properties");
            return;
        }
        
        await ProcessChangingPassword(passwordChangeModel);
    }

    private async Task ProcessPasswordReset(UserPasswordResetRequestDto? dtoV1)
    {
        HttpRequestMessage message = new()
        {
            RequestUri = new Uri(device.Server.GetFullAddress(Api.Essential.Customer.EndPoints.Service.Password)),
            Version = new Version("1.0"),
            Method = HttpMethod.Post,
        };
        
        var responseMessage = await server.Send(message, dtoV1);
        if (responseMessage.IsSuccessStatusCode)
        {
            await device.Alert.DisplayAlert("New Password was created, please check your email.");
            await device.Navigation.NavigateToAsync(ApplicationPages.Essential.UserPage, forceLoad:true);
            return;
        }
        
        Console.WriteLine("Invalid message: " + responseMessage.ReasonPhrase);
        await device.Alert.DisplayAlert("Something went wrong!");
    }
    
    private async Task RegisterAsync(RegisterCustomerDto model)
    {
        HttpRequestMessage message = new()
        {
            RequestUri = new Uri(device.Server.GetFullAddress(Api.Essential.Customer.EndPoints.Service.Customer)),
            Version = new Version("1.0"),
            Method = HttpMethod.Post,
        };
        
        var response = await server.Send(message, model);

        if (response.IsSuccessStatusCode)
        {
            var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto<AuthenticationResponseDto>>();
            var token = responseDto?.Payload?.Token;
            await device.Security.UpdateTokenAsync(token ?? string.Empty);
            await device.Navigation.NavigateToAsync(ApplicationPages.Essential.SplashPage, forceLoad: true);
            return;
        }

        var displayString = response.StatusCode switch
        {
            HttpStatusCode.Conflict => "Either username or email or mobile number already exist!",
            HttpStatusCode.BadRequest => "Invalid details, please try again.",
            HttpStatusCode.BadGateway => "Please check your internet connection then try again.",
            _ => throw new ArgumentOutOfRangeException(response.StatusCode.ToString())
        };
        
        await device.Alert.DisplayAlert(displayString);
    }
    
    private async Task ProcessChangingPassword(UserPasswordChangeRequestDto? dto)
    {
        HttpRequestMessage message = new()
        {
            RequestUri = new Uri(device.Server.GetFullAddress(Api.Essential.Customer.EndPoints.Service.Password)),
            Version = new Version("1.0"),
            Method = HttpMethod.Patch,
        };
        
        var response = await server.Send(message, dto);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
            case HttpStatusCode.Accepted:
                await device.Alert.DisplayAlert("PasswordUpdatedSuccessfully");
                await device.Navigation.NavigateToAsync(ApplicationPages.Essential.SplashPage);
                return;
            case HttpStatusCode.NotFound:
                await device.Alert.DisplayAlert("InvalidPassword");
                break;
            default:
                await device.Alert.DisplayAlert("DeveloperMinorBug");
                break;
        }
        
        await device.Navigation.NavigateToAsync(ApplicationPages.Essential.UserPage, forceLoad: true);
    }
    
    private async Task SetupMenuAsync()
    {
        var list = new List<MenuItemModel>();

        foreach (var claim in await device.Security.GetClaimsAsync())
        {
            if (claim.Type is ClaimsValues.ClaimTypes.Session or ClaimsValues.ClaimTypes.Territory or ClaimsValues.ClaimTypes.Country)
            {
                continue;
            }

            if (ClaimsValues.GetClaimTypes().ContainsValue(claim.Type))
            {
                var name = ClaimsValues.GetClaimTypes().First(x => x.Value == claim.Type);
                list.Add(MenuItemModel.Create(name.Key));
            }
        }

        MenuItems.Clear();
        MenuItems.AddRange(list.DistinctBy(x => x.PageName));
    }
}