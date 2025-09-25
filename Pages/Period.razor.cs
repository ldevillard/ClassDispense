using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text;

namespace ClassDispense.Pages
{
    public partial class Period : ComponentBase
    {
        struct PupilPeriod
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public DateTime? PeriodDate { get; set; }

            public string Note { get; set; }
        }

        List<PupilPeriod> pupils = new List<PupilPeriod>();
        IEnumerable<PupilPeriod> sortedPupils => pupils.OrderBy(p => p.Class).ThenBy(p => p.Name);

        MudForm form = new MudForm();
        PupilPeriod newPupil = new PupilPeriod();
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
            pupils = await JS.InvokeAsync<List<PupilPeriod>>("loadList", "periods") ?? new List<PupilPeriod>();
        }

        async Task SavePupils()
        {
            await JS.InvokeVoidAsync("saveList", "periods", pupils);
        }

        async Task AddPupil()
        {
            await form.Validate();
            if (form.IsValid)
            {
                pupils.Add(newPupil);
                newPupil = new PupilPeriod();
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

        async Task RemovePupil(PupilPeriod pupil)
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

        async Task ConfirmRemovePupil(PupilPeriod pupil)
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
            csv.AppendLine("Nom, Classe, Date de règles, Note");
            csv.AppendLine();

            foreach (PupilPeriod p in pupils)
            {
                csv.AppendLine($"{p.Name}, {p.Class}, {p.PeriodDate:yyyy-MM-dd}, {p.Note}");
            }

            await JS.InvokeVoidAsync("downloadFile", "eleves_règles.csv", csv.ToString());
        }
    }
}
