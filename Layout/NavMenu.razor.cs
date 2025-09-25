using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ClassDispense.Layout
{
    public partial class NavMenu : ComponentBase
    {
        bool drawerOpen = false;
        
        bool _isDarkMode;
        bool isDarkMode 
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    JS.InvokeVoidAsync("localStorage.setItem", "isDarkMode", value);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            string stored = await JS.InvokeAsync<string>("localStorage.getItem", "isDarkMode");
            if (bool.TryParse(stored, out var result))
            {
                isDarkMode = result;
            }
        }

        void ToggleNavMenu()
        {
            drawerOpen = !drawerOpen;
        }
    }
}
