﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using WeeControl.Applications.BaseLib.Enumerators;
using WeeControl.Applications.BaseLib.Interfaces;
using WeeControl.SharedKernel.BasicSchemas.Common.DtosV1;
using WeeControl.SharedKernel.BasicSchemas.Common.Enums;
using WeeControl.SharedKernel.BasicSchemas.Common.Interfaces;
using WeeControl.SharedKernel.BasicSchemas.Employee.DtosV1;

namespace WeeControl.Applications.BaseLib.ViewModels
{
    public class SplashViewModel : ObservableObject
    {
        private readonly IDevice device;
        private readonly HttpClient httpClient;
        private readonly string route;
        private readonly ILogger logger;
        private readonly RequestMetadata metadata;

        public IAsyncRelayCommand RefreshTokenCommand { get; }

        public SplashViewModel()
            : this(Ioc.Default.GetRequiredService<IViewModelDependencyFactory>(), Ioc.Default.GetRequiredService<IApiDicts>())
        {
        }

        public SplashViewModel(IViewModelDependencyFactory dependencyFactory, IApiDicts commonValues)
        {
            if (dependencyFactory == null || commonValues == null)
            {
                throw new ArgumentNullException();
            }

            httpClient = dependencyFactory.HttpClientInstance;
            route = commonValues.ApiRoute[ApiRouteEnum.EmployeeSession];
            device = dependencyFactory.Device;
            logger = dependencyFactory.Logger;
            metadata = (RequestMetadata)device.Metadata;

            RefreshTokenCommand = new AsyncRelayCommand(VerifyTokenAsync);
        }

        private async Task VerifyTokenAsync()
        {
            if (string.IsNullOrEmpty(device.Token))
            {
                await device.NavigateToPageAsync(nameof(ApplicationPageEnum.LoginPage));// "LoginPage");
                return;
            }

            if (device.Internet == false)
            {
                await device.DisplayMessageAsync(IDevice.Message.NoInternet);
                await device.TerminateAppAsync();
                return;
            }

            var dto = new RefreshLoginDto() { Metadata = metadata };

            await CommunicateWithServer(dto);
        }

        private async Task CommunicateWithServer(RefreshLoginDto dto)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(device.Token);
                var response = await httpClient.PutAsJsonAsync(route, dto);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        var responseDto = await response.Content.ReadAsAsync<EmployeeTokenDto>();
                        device.Token = responseDto?.Token;
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(device.Token);
                        await device.NavigateToPageAsync("HomePage");
                        break;
                    default:
                        await device.NavigateToPageAsync("LoginPage");
                        break;
                }
            }
            catch (System.Net.WebException)
            {
                await device.DisplayMessageAsync("Server Connection Error", "The Application can't connect to the server, ensure that the applicaiton is updated or try again later.");
                await device.TerminateAppAsync();
                return;
            }
            catch (Exception e)
            {
                logger?.LogCritical(e.Message);
                await device.DisplayMessageAsync("Exception", e.Message);
                await device.TerminateAppAsync();
                return;
            }
        }
    }
}