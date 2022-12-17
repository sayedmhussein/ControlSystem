using Microsoft.Extensions.DependencyInjection;
using WeeControl.Common.SharedKernel;
using WeeControl.Frontend.AppService.Contexts.Authentication;
using WeeControl.Frontend.AppService.Contexts.Home;
using WeeControl.Frontend.AppService.Contexts.Temporary.Interfaces;
using WeeControl.Frontend.AppService.Contexts.Temporary.Services;
using WeeControl.Frontend.AppService.Internals.Interfaces;
using WeeControl.Frontend.AppService.Internals.Services;

namespace WeeControl.Frontend.AppService;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddUserSecurityService(); //From Shared Kernel
        
        services.AddSingleton<IPersistedLists, PersistedListService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IDeviceSecurity, SecurityService>();
        
        //services.AddSingleton<IDevice, DeviceService>();
        
        services.AddTransient<IServerOperation, ServerOperationService>();
        
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<IHomeService, HomeService>();
        
        services.AddTransient<IUserService, UserService>();
        
        //services.AddTransient<TerritoryService>();

        return services;
    }
}