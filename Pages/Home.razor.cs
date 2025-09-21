using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClassDispense.Pages
{
    public partial class Home : ComponentBase
    {
        struct Pupil
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public DateTime? DispenseEndDate { get; set; }
            public int DispenseDaysRemaining { get; set; }
            public string Reason { get; set; }
        }

        List<Pupil> pupils = new List<Pupil>
        {
            new Pupil { Name = "Alice Johnson", Class = "5A", DispenseEndDate = DateTime.Now.AddDays(3), DispenseDaysRemaining = 3, Reason = "Medical Appointment" },
            new Pupil { Name = "Bob Smith", Class = "6B", DispenseEndDate = DateTime.Now.AddDays(5), DispenseDaysRemaining = 5, Reason = "Family Emergency" },
            new Pupil { Name = "Charlie Brown", Class = "5C", DispenseEndDate = DateTime.Now.AddDays(2), DispenseDaysRemaining = 2, Reason = "Sports Event" }
        };

        MudForm form = new MudForm();
        Pupil newPupil = new Pupil();
        private bool formVisible = false;

        private void ToggleForm()
        {
            formVisible = !formVisible;
        }

        async Task AddPupil()
        {
            await form.Validate();
            if (form.IsValid)
            {
                pupils.Add(newPupil);
                newPupil = new Pupil();
                StateHasChanged();
            }
        }
    }
}
