using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WeeControl.ApiApp.WebApi.Services.Security.CustomHandlers.TokenRefreshment;
using WeeControl.ApiApp.WebApi.Services.Security.Policies;
using WeeControl.Common.SharedKernel.Interfaces;
using WeeControl.Common.SharedKernel.Services;

namespace WeeControl.ApiApp.WebApi.Services.Security;

internal static class UserSecurityServices
{
    internal static IServiceCollection AddUserSecurityService(this IServiceCollection services)
    {
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordSecurity, PasswordSecurity>();
        services.AddSingleton<IAuthorizationHandler, TokenRefreshmentHandler>();

        services.AddAuthorizationCore(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder().AddAuthenticationSchemes("Bearer").RequireAuthenticatedUser().Build();
            options.FallbackPolicy = new AuthorizationPolicyBuilder().AddAuthenticationSchemes("Bearer").RequireAuthenticatedUser().Build();
                
            Configure(options);
        });

        return services;
    }
    
    private static void Configure(AuthorizationOptions options)
    {
        // var types = Assembly
        //     .GetExecutingAssembly()
        //     .GetTypes()
        //     .Where(t => t.BaseType == typeof(PolicyBuilderBase));
        //
        // foreach (var t in types)
        // {
        //     options.AddPolicy(t.Name, t.GetPolicy());
        // }
        
        options.AddPolicy(DeveloperWithDatabaseOperationPolicy.Name, new DeveloperWithDatabaseOperationPolicy().GetPolicy());
        
        options.AddPolicy(nameof(CanEditUserPolicy), new CanEditUserPolicy().GetPolicy());
        options.AddPolicy(CanEditTerritoriesPolicy.Name, new CanEditTerritoriesPolicy().GetPolicy());
    }
}