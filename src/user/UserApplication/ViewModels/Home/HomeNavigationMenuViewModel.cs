using WeeControl.SharedKernel;
using WeeControl.User.UserApplication.Interfaces;

namespace WeeControl.User.UserApplication.ViewModels.Home;

public class HomeNavigationMenuViewModel : ViewModelBase
{
    private readonly IDevice device;

    public IEnumerable<MenuItem> MenuItems { get; private set; } = new List<MenuItem>();

    public HomeNavigationMenuViewModel(IDevice device) : base(device)
    {
        this.device = device;
    }
    
    public Task ChangeMyPasswordAsync()
    {
        return device.Navigation.NavigateToAsync(Pages.Essential.User.SetNewPasswordPage);
    }

    public async Task SetupMenuAsync()
    {
        var list = new List<MenuItem>();
        
        foreach (var claim in await device.Security.GetClaimsAsync())
        {
            if (claim.Type is ClaimsTagsList.Claims.Session or ClaimsTagsList.Claims.Territory)
            {
                continue;
            }

            if (ClaimsTagsList.GetClaimsDictionary().ContainsValue(claim.Type))
            {
                var name = ClaimsTagsList.GetClaimsDictionary().First(x => x.Value == claim.Type);
                list.Add(MenuItem.Create(name.Key));
            }
        }

        MenuItems = new List<MenuItem>(list.DistinctBy(x => x.PageName));
    }
}