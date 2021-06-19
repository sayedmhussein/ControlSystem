﻿using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WeeControl.Server.WebApi.Security.TokenRefreshment.CustomHandlers;
using WeeControl.SharedKernel.BasicSchemas.Employee.Dicts;
using WeeControl.SharedKernel.BasicSchemas.Employee.Enums;

namespace WeeControl.Server.WebApi.Security.Policies.Territory
{
    public static class CanGetTerritoryPolicy
    {
        private static IClaimDicts values = new ClaimDicts();

        public const string Name = "CanGetTerritoryPolicy"; 

        public static AuthorizationPolicy Policy
        {
            get
            {
                var p = new AuthorizationPolicyBuilder();
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireClaim(values.ClaimType[ClaimTypeEnum.Session]);
                p.Requirements.Add(new TokenRefreshmentRequirement(TimeSpan.FromMinutes(100)));

                return p.Build();
            }
        }
    }
}