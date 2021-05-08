﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySystem.XamarinForms.Services;
using MySystem.XamarinForms.Views;
using MySystem.ClientService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MySystem.ClientService.ViewModels;
using System.IO;
using System.Reflection;
using MySystem.XamarinForms.Models;
using Newtonsoft.Json;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace MySystem.XamarinForms
{
    public partial class Application : Xamarin.Forms.Application
    {
        private static AppSettings appSetting;
        public static AppSettings AppSettings
        {
            get
            {
                if (appSetting == null)
                    LoadAppSetting();

                return appSetting;
            }
        }

        public Application()
        {
            InitializeComponent();
            
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<IAppSettings, AppSettings>()
                .AddSingleton<IDeviceInfo, DeviceInfo>()
                .BuildServiceProvider());

            DeviceResources.InitializeClient(AppSettings.ApiBase, appSetting.ApiVersion);

            MainPage = new SplashPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private static void LoadAppSetting()
        {
            var appsettingResouceStream = Assembly.GetAssembly(typeof(AppSettings)).GetManifestResourceStream("MySystem.XamarinForms.Configuration.appsettings.json");
            if (appsettingResouceStream == null)
                return;

            using (var streamReader = new StreamReader(appsettingResouceStream))
            {
                var jsonStream = streamReader.ReadToEnd();
                appSetting = JsonConvert.DeserializeObject<AppSettings>(jsonStream);
            }
            
        }
    }
}
