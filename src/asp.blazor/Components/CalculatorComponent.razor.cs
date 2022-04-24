using asp.blazor.Components.CalculatorParts;
using statemap;

namespace asp.blazor.Components
{
    public partial class CalculatorComponent
    {
        private Type pageType = typeof(Splash);
        private string? errorMsg;
        private Dictionary<string, object>? errorParams => errorMsg != null ? new() { { "ErrorMsg", errorMsg } } : null;

        // Blazor State Container / SMC event notification pattern
        public void StateChanged(object sender, StateChangeEventArgs args)
        {
            ReRender();
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            Main.Fsm.StateChange += StateChanged;
        }

        public void Dispose()
        {
            Main.Fsm.StateChange -= StateChanged;
        }

        private void ReRender()
        {
            errorMsg = null;
            switch (Main.State)
            {
                case var s when s == CalculatorContext.Map1.Splash:
                    pageType = typeof(Splash);
                    break;

                case var s when s == CalculatorContext.Map1.Calculate:
                    pageType = typeof(Calculate);
                    break;

                case var s when s == CalculatorContext.Map1.Enter:
                    pageType = typeof(Enter);
                    break;

                case var s when s == CalculatorContext.Map1.ErrorNumeric:
                    errorMsg = "The input was not numeric.";
                    pageType = typeof(Error);
                    break;

                case var s when s == CalculatorContext.Map1.ErrorTuple:
                    errorMsg = "Need two values on the stack to compute.";
                    pageType = typeof(Error);
                    break;

                case var s when s == CalculatorContext.Map1.ErrorEmpty:
                    errorMsg = "Need a value on the stack to compute.";
                    pageType = typeof(Error);
                    break;

                default:
                    throw new NotImplementedException(String.Format("State {0}", Main.State));
            }
        }
    }
}