using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.JSInterop;

namespace ClassDispense.Pages
{
    public partial class Home : ComponentBase
    {
        struct Pupil
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public DateTime? DispenseEndDate { get; set; }

            public int DaysRemaining => DispenseEndDate.HasValue ? Math.Max((DispenseEndDate.Value.Date - DateTime.Today).Days, 0) : 0;

            public string Reason { get; set; }
        }

        List<Pupil> pupils = new List<Pupil>();
        IEnumerable<Pupil> sortedPupils => pupils.OrderBy(p => p.Class).ThenBy(p => p.Name);

        MudForm form = new MudForm();
        Pupil newPupil = new Pupil();
        private bool formVisible = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                pupils = await JS.InvokeAsync<List<Pupil>>("loadPupils") ?? new List<Pupil>();
                StateHasChanged();
            }
        }

        private void ToggleForm()
        {
            formVisible = !formVisible;
        }

        async Task LoadPupils()
        {
            pupils = await JS.InvokeAsync<List<Pupil>>("loadPupils") ?? new List<Pupil>();
        }

        async Task SavePupils()
        {
            await JS.InvokeVoidAsync("savePupils", pupils);
        }

        async Task AddPupil()
        {
            await form.Validate();
            if (form.IsValid)
            {
                pupils.Add(newPupil);
                newPupil = new Pupil();
                StateHasChanged();
                await SavePupils();
            }
        }

        async Task RemovePupil(Pupil pupil)
        {
            pupils.Remove(pupil);
            await SavePupils();
        }
    }
}
