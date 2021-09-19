﻿using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WeeControl.SharedKernel.EntityGroups.Employee.Enums;
using WeeControl.SharedKernel.EntityGroups.Employee.Interfaces;
using WeeControl.UserSecurityLib.Helpers.CustomHandlers.TokenRefreshment;

namespace WeeControl.Backend.WebApi.Security.Policies
{
    public class BasicPolicies
    {
        public const string HasActiveSession = "HasSessionPolicy";
        //
        public const string CanAlterTerritories = "CanAlterTerritories";
        //
        public const string CanAddNewEmployee = "CanAddNewEmployee";
        public const string CanEditEmployeeDetails = "CanEditEmployeeDetails";

        private readonly IEmployeeAttribute employeeAttribute;

        public BasicPolicies(IEmployeeAttribute employeeAttribute)
        {
            this.employeeAttribute = employeeAttribute;
        }

        public void BuildOptions(AuthorizationOptions options)
        {
            options.AddPolicy(HasActiveSession, GetPolicy(HasActiveSession));
            //
            options.AddPolicy(CanAlterTerritories, GetPolicy(CanAlterTerritories));
            //
            options.AddPolicy(CanAddNewEmployee, GetPolicy(CanAddNewEmployee));
            options.AddPolicy(CanEditEmployeeDetails, GetPolicy(CanEditEmployeeDetails));
        }

        private AuthorizationPolicy GetPolicy(string policy)
        {
            var p = new AuthorizationPolicyBuilder();
            p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            p.RequireClaim(employeeAttribute.GetClaimType(ClaimTypeEnum.Session));

            switch (policy)
            {
                case CanAlterTerritories:
                    p.Requirements.Add(new TokenRefreshmentRequirement(TimeSpan.FromMinutes(5)));
                    break;
                case CanAddNewEmployee:
                    break;
                case CanEditEmployeeDetails:
                    break;
                default:
                    break;
            }

            return p.Build();
        }
    }
}
