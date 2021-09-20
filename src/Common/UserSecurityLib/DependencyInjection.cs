using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WeeControl.Common.UserSecurityLib.Helpers;
using WeeControl.Common.UserSecurityLib.Helpers.CustomHandlers.TokenRefreshment;
using WeeControl.Common.UserSecurityLib.Interfaces;
using WeeControl.Common.UserSecurityLib.Services;

namespace WeeControl.Common.UserSecurityLib
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserSecurityService(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, TokenRefreshmentHandler>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddAuthorizationCore(UserAuthorizationOptions.Configure);

            return services;
        }
        
        public static IServiceCollection AddApiAuthenticationService(this IServiceCollection services)
        {
            return services;
        }
    }
}