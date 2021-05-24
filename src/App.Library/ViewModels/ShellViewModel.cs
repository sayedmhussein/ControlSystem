﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using MySystem.Web.ClientService.Services;
using MySystem.Web.Shared.Dtos.V1;

namespace MySystem.ClientService.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        #region Private Properties
        private readonly IDevice device;
        private readonly IClientServices service;
        
        #endregion

        #region Public Properties
        public string NameOfUser => device.FullUserName;

        #endregion

        #region Commands
        public ICommand HelpCommand { get; }
        public ICommand LogoutCommand { get; }
        #endregion

        #region Constructors
        public ShellViewModel() : this(Ioc.Default.GetService<IDevice>(), Ioc.Default.GetRequiredService<IClientServices>())
        {
        }

        public ShellViewModel(IDevice device, IClientServices service)
        {
            this.device = device;
            this.service = service;

            HelpCommand = new RelayCommand(async () => await device.OpenWebPageAsync("http://www.google.com/"));
            LogoutCommand = new AsyncRelayCommand(Logout);
        }
        #endregion

        #region Private Functions
        private async Task Logout()
        {
            device.Token = string.Empty;
            await Task.Run(async () =>
            {
                try
                {
                    var dto = new RequestDto<object>(device.DeviceId);
                    var response = await service.HttpClientInstance.PostAsJsonAsync("/Api/Credentials/logout", dto);
                }
                catch
                { }
            });
        }
        #endregion
    }
}