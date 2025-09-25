using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.JSInterop;
using System.Text;

namespace ClassDispense.Pages
{
    public partial class Dispense : ComponentBase
    {
        struct PupilDispense
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public DateTime? DispenseEndDate { get; set; }

            public int DaysRemaining => DispenseEndDate.HasValue ? Math.Max((DispenseEndDate.Value.Date - DateTime.Today).Days, 0) : 0;

            public string Reason { get; set; }
        }

        List<PupilDispense> pupils = new List<PupilDispense>();
        IEnumerable<PupilDispense> sortedPupils => pupils.OrderBy(p => p.Class).ThenBy(p => p.Name);

        MudForm form = new MudForm();
        PupilDispense newPupil = new PupilDispense();
        bool formVisible = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadPupils();
                StateHasChanged();
            }
        }

        void ToggleForm()
        {
            formVisible = !formVisible;
        }

        async Task LoadPupils()
        {
            pupils = await JS.InvokeAsync<List<PupilDispense>>("loadList", "pupils") ?? new List<PupilDispense>();
        }

        async Task SavePupils()
        {
            await JS.InvokeVoidAsync("saveList", "pupils", pupils);
        }

        async Task AddPupil()
        {
            await form.Validate();
            if (form.IsValid)
            {
                pupils.Add(newPupil);
                newPupil = new PupilDispense();
                StateHasChanged();
                await SavePupils();

                Snackbar.Add("Élève ajouté", Severity.Success, config =>
                {
                    config.ShowCloseIcon = false;
                    config.VisibleStateDuration = 2000; // ms
                    config.SnackbarVariant = Variant.Filled;
                });
            }
        }

        async Task RemovePupil(PupilDispense pupil)
        {
            pupils.Remove(pupil);
            await SavePupils();

            Snackbar.Add("Élève supprimé", Severity.Error, config =>
            {
                config.ShowCloseIcon = false;
                config.VisibleStateDuration = 2000; // ms
                config.SnackbarVariant = Variant.Filled;
            });
        }

        async Task ConfirmRemovePupil(PupilDispense pupil)
        {
            bool? result = await DialogService.ShowMessageBox(
                "Confirmation",
                $"Voulez-vous vraiment supprimer {pupil.Name} ?",
                yesText: "Supprimer",
                noText: "Annuler",
                options: new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall }
            );

            if (result == true)
            {
                await RemovePupil(pupil);
            }
        }

        async Task ExportCsv()
        {
            var csv = new StringBuilder();
            
            csv.AppendLine("Date de sauvegarde: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            csv.AppendLine();
            csv.AppendLine("Nom, Classe, Date fin de dispense, Jours restants, Raison");
            csv.AppendLine();
            
            foreach (PupilDispense p in sortedPupils)
            {
                csv.AppendLine($"{p.Name}, {p.Class}, {p.DispenseEndDate:yyyy-MM-dd}, {p.DaysRemaining}, {p.Reason}");
            }

            await JS.InvokeVoidAsync("downloadFile", "eleves_dispenses.csv", csv.ToString());
        }
    }
}
