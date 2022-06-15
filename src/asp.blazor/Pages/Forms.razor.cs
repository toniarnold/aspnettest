using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace asp.blazor.Pages
{
    public partial class Forms
    {
        public EditContext? editContext;
        public InputCheckbox check = default!;
        public InputDate<DateTime> date = default!;
        public InputNumber<decimal> dec = default!;
        public InputNumber<int> integer = default!;
        public ElementReference someSalad = default!;
        public InputText line = default!;
        public InputTextArea paragraph = default!;
        public ElementReference submit = default!;
        //public InputSelect<Salad> saladSelection = default!;    // "Cannot convert lambda expression to intended delegate..."

        protected override void OnInitialized()
        {
            editContext = new(Main);
            this.AddEditContextTestFocus(editContext);
        }

        private void HandleValidSubmit()
        {
            Logger.LogInformation("HandleValidSubmit called");
        }
    }
}