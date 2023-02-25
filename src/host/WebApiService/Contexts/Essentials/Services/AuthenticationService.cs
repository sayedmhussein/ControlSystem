using System.Net;
using WeeControl.Core.DataTransferObject.Contexts.Essentials;
using WeeControl.Core.SharedKernel;
using WeeControl.Host.WebApiService.DeviceInterfaces;
using WeeControl.Host.WebApiService.Internals.Interfaces;

namespace WeeControl.Host.WebApiService.Contexts.Essentials.Services;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IGui gui;
    private readonly IServerOperation server;
    private readonly IDeviceSecurity security;
    private readonly IStorage storage;

    public AuthenticationService(IGui gui, IServerOperation server, IDeviceSecurity security, IStorage storage)
    {
        this.gui = gui;
        this.server = server;
        this.security = security;
        this.storage = storage;
    }
    
    public async Task Login(LoginRequestDto dto)
    {
        var errors = dto.GetModelValidationError();
        if (errors.Any())
        {
            await gui.DisplayAlert(errors.Keys.First());
            return;
        }

        var response = await server
            .GetResponseMessage(HttpMethod.Post, new Version("1.0"), dto, ControllerApi.Essentials.Authorization.Route);

        if (response.IsSuccessStatusCode)
        {
            var token = await server.ReadFromContent<TokenResponseDto>(response.Content);
            if (token?.Token is not null)
            {
                await security.UpdateToken(token.Token);
                await gui.NavigateToAsync(ApplicationPages.Essential.OtpPage);
                return;
            }
        }

        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                await gui.DisplayAlert("Ínvalid credentials, please try again.");
                break;
            case HttpStatusCode.Forbidden:
                await gui.DisplayAlert("Account is locked, please contact your admin.");
                break;
            default:
                await gui.DisplayAlert("Unexpected Error, please try again.");
                break;
        }
    }

    public async Task UpdateToken()
    {
        if (await security.IsAuthenticated())
        {
            await server.RefreshToken();
            return;
        }

        await gui.DisplayAlert("Please login.");
        await gui.NavigateToAsync(ApplicationPages.Essential.LoginPage, forceLoad:true);
    }
    
    public async Task UpdateToken(string otp)
    {
        if (await security.IsAuthenticated())
        {
            var response = await server
                .GetResponseMessage(HttpMethod.Put, new Version("1.0"), new object(), ControllerApi.Essentials.Authorization.Route, 
                    query: new []{"otp", otp});

            if (response.IsSuccessStatusCode)
            {
                var token = await server.ReadFromContent<TokenResponseDto>(response.Content);
                if (token?.Token is not null)
                {
                    await security.UpdateToken(token.Token);
                    await gui.NavigateToAsync(ApplicationPages.Essential.HomePage, forceLoad: true);
                    return;
                }
            }
        }
        
        await gui.DisplayAlert("Please login.");
        await gui.NavigateToAsync(ApplicationPages.Essential.LoginPage);
    }

    public async Task Logout()
    {
        await security.DeleteToken();
        await storage.ClearKeysValues();
        var response = await server.GetResponseMessage(
            HttpMethod.Delete, new Version("1.0"),
            ControllerApi.Essentials.Authorization.Route);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            await gui.DisplayAlert(
                "An issue was encountered while logging out, please report this case to the developer.");
        }

        await gui.NavigateToAsync(ApplicationPages.Essential.LoginPage, forceLoad: true);
    }
}