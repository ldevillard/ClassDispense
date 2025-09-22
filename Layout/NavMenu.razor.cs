using Microsoft.AspNetCore.Components;

namespace ClassDispense.Layout
{
    public partial class NavMenu : ComponentBase
    {
        bool drawerOpen = false;
        bool isDarkMode = true;

        void ToggleNavMenu()
        {
            drawerOpen = !drawerOpen;
        }

        void ToggleDarkLight()
        {
            isDarkMode = !isDarkMode;
        }
    }
}
