using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using WeeControl.Frontend.AppService.GuiInterfaces.Obsolute;

namespace WeeControl.Frontend.Wasm.Services;

public class DevicePageNavigationService : IDevicePageNavigation
{
    private readonly NavigationManager navigationManager;
    private readonly ICollection<string> history;

    public DevicePageNavigationService(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
        
        history = new List<string>();
        history.Add(AppService.ApplicationPages.Essential.HomePage);
    }

    public Task NavigateToAsync(string pageName, bool forceLoad = false)
    {
        navigationManager.NavigateTo($"/{pageName}", forceLoad: forceLoad);
        return Task.CompletedTask;
    }

  

    public Task GoBackAsync()
    {
        return NavigateToAsync(history.LastOrDefault());
    }
}