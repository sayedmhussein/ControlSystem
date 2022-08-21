using Microsoft.Extensions.DependencyInjection;
using WeeControl.Frontend.ApplicationService.Contexts.Business.Elevator;
using WeeControl.Frontend.ApplicationService.Contexts.Essential.Interfaces;
using WeeControl.Frontend.ApplicationService.Contexts.Essential.Services;
using WeeControl.Frontend.ApplicationService.Interfaces;
using WeeControl.Frontend.ApplicationService.Services;

namespace WeeControl.Frontend.ApplicationService;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IPersistedLists, PersistedListService>();
        services.AddTransient<IServerOperation, ServerOperationService>();
        
        services.AddTransient<IUserAuthorizationService, UserAuthorizationService>();
        services.AddTransient<IUserService, UserService>();
        
        services.AddTransient<TerritoryService>();

        return services;
    }
}