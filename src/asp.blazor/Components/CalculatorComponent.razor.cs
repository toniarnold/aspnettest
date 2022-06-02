using asp.blazor.Components.CalculatorParts;
using asplib.Model;
using Microsoft.AspNetCore.Components;
using statemap;
using StorageEnum = asplib.Model.Storage;

namespace asp.blazor.Components
{
    public partial class CalculatorComponent
    {
        public ElementReference storageLink;
        public DynamicComponent calculatorPart = default!;
        public Footer footer = default!;

        private Type pageType = typeof(Splash);
        private string? errorMsg;
        private Dictionary<string, object>? errorParams => errorMsg != null ? new() { { "ErrorMsg", errorMsg } } : null;

        [Inject]
        private NavigationManager uriHelper { get; set; } = default!;

        [Parameter]
        public string StorageLinkUrl { get; set; } = default!;

        [Parameter]
        public string StorageLinkClientID { get; set; } = default!;

        [Parameter]
        public string Storage
        {
            get { return this.GetStorage().ToString(); }
            set { this.SetStorage(value); }
        }

        public string Encrypted
        {
            get
            {
                return (this.GetStorage() == StorageEnum.Database && StorageImplementation.GetEncryptDatabaseStorage(this.Configuration)) ?
                    "&#x1f512;" : String.Empty;
            }
        }

        /// <summary>
        /// Force a reload to be able to change to database
        /// </summary>
        public void ChangeStorage()
        {
            uriHelper.NavigateTo(StorageLinkUrl, forceLoad: true);
        }

        protected override void RenderMain()
        {
            errorMsg = null;
            switch (State)
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