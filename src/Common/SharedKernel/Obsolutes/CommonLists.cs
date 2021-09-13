﻿using System;
using System.Collections.Generic;
using System.Linq;
using WeeControl.SharedKernel.Interfaces;

namespace WeeControl.SharedKernel.Obsolutes
{
    [Obsolete]
    public class CommonLists : AppSettings, ICommonLists
    {
        private Dictionary<ApiRouteEnum, string> routes;

        public string GetRoute(ApiRouteEnum api)
        {
            if (routes != null) return routes[api];
            
            routes = new Dictionary<ApiRouteEnum, string>();
            foreach (var e in Enum.GetValues(typeof(ApiRouteEnum)).Cast<ApiRouteEnum>())
            {
                string value = json.ApiRoute[e.ToString()];
                routes.Add(e, value);
            }
            return routes[api];
        }
    }
}