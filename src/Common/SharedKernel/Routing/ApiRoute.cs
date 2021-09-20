﻿using System.Collections.Generic;
using WeeControl.Common.SharedKernel.Services;

namespace WeeControl.Common.SharedKernel.Routing
{
    public class ApiRoute : IApiRoute
    {
        private readonly AppSettingReader appSettingReader;
        
        private Dictionary<ApiRouteEnum, string> routes;

        public ApiRoute()
        {
            appSettingReader = new AppSettingReader(typeof(ApiRoute).Namespace, "attributes.json");
        }
        
        public string GetRoute(ApiRouteEnum api)
        {
            appSettingReader.PopulateAttribute(ref routes, "ApiRoute");
            return routes[api];
        }
    }
}
