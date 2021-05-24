﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using MySystem.Web.ClientService.Services;
using MySystem.Web.Shared.Dtos.V1;
using MySystem.Web.Shared.Dtos.V1.Custom;

namespace MySystem.Web.ClientService.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        #region Private Properties
        private readonly IDevice device;
        private readonly IClientServices service;
        private readonly ILogger logger;
        #endregion

        #region Public Properties
        private string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string password;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string Instructions { get => service.Settings.Login.Disclaimer; }
        #endregion

        #region Commands
        public IAsyncRelayCommand LoginCommand { get; }
        #endregion

        #region Constructors
        public LoginViewModel()
            : this(Ioc.Default.GetRequiredService<IClientServices>())
        {
        }

        public LoginViewModel(IClientServices service)
        {
            this.service = service ?? throw new ArgumentNullException();
            this.device = service.Device;
            this.logger = service.Logger;

            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }
        #endregion

        #region Private Functions
        private async Task LoginAsync()
        {
            if (device.Internet)
            {
                //VerifyUserInputs();
                var loginDto = new LoginDto() { Username = Username, Password = Password };
                //Validator.ValidateObject(loginDto, new ValidationContext(this, null, null));
                var dto = new RequestDto<LoginDto>(deviceId: device.DeviceId, payload: loginDto);

                try
                {
                    var response = await service.HttpClientInstance.PostAsJsonAsync(service.Api.Login, dto);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsAsync<ResponseDto<string>>();
                        device.Token = data.Payload;
                        await device.NavigateToPageAsync("HomePage");
                    }
                    else
                    {
                        await device.DisplayMessageAsync("Invalid Credentials", "Invalid uername or password, please try again later.");
                    }
                }
                catch (Exception e)
                {
                    await device.DisplayMessageAsync("Exception", e.Message);
                    throw;
                }
            }
            else
            {
                await device.DisplayMessageAsync(IDevice.Message.NoInternet);
            }
        }

        private void VerifyUserInputs()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentNullException("Invalid Username or Password");
            }
        }
        #endregion
    }
}