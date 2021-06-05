﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MySystem.SharedKernel.Entities.Public.Constants;

namespace MySystem.Web.Api.Security.Policies.Employee
{
    public static class AbleToRefreshTokenPolicy
    {
        public const string Name = "AbleToRefreshTokenPolicy"; 

        public static AuthorizationPolicy Policy
        {
            get
            {
                var p = new AuthorizationPolicyBuilder();
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireClaim(Claims.Types[Claims.ClaimType.Session], Claims.Tags[Claims.ClaimTag.Add]);

                return p.Build();
            }
        }
    }
}
