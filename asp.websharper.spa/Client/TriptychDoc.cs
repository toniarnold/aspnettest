using asp.websharper.spa.Model;
using asp.websharper.spa.Remoting;
using System.Threading.Tasks;
using WebSharper;
using WebSharper.UI;
using SessionStorage = asplib.Model.Storage;

namespace asp.websharper.spa.Client
{
    /// <summary>
    /// Triptych containing three CalculatorDoc which different storage each.
    /// </summary>
    [JavaScript]
    public class TriptychDoc
    {
        // Prefixes for constructing the unique client id (mimicking the
        // ASP.NET WebForms UserControl hierarchy) to be referenced in tests.
        public const string ViewStateDoc = "ViewStateDoc";

        public const string SessionDoc = "SessionDoc";
        public const string DatabaseDoc = "DatabaseDoc";

        /// <summary>
        /// Set up three calculator model objects for each calculator view
        /// Explicitly set to null to enforce loading needed (unlike in F#)
        /// </summary>
        public static WebSharper.UI.Doc MainDoc(Var<string> page)
        {
            var varViewStateCalculator = Var.Create(new CalculatorViewModel());
            varViewStateCalculator.Set(null);
            var viewViewStateCalculator = varViewStateCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(c =>
            {
                if (c == null)
                    return CalculatorServer.Load(SessionStorage.ViewState);
                else
                    return Task.FromResult(c);
            });

            var varSessionCalculator = Var.Create(new CalculatorViewModel());
            varSessionCalculator.Set(null);
            var viewSessionCalculator = varSessionCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(c =>
            {
                if (c == null)
                    return CalculatorServer.Load(SessionStorage.Session);
                else
                    return Task.FromResult(c);
            });

            var varDatabaseCalculator = Var.Create(new CalculatorViewModel());
            varDatabaseCalculator.Set(null);
            var viewDatabaseCalculator = varDatabaseCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(c =>
            {
                if (c == null)
                    return CalculatorServer.Load(SessionStorage.Database);
                else
                    return Task.FromResult(c);
            });

            return new Template.Triptych.Main()
                .ViewState(CalculatorDoc.MainDoc(viewViewStateCalculator, varViewStateCalculator, page, id: ViewStateDoc))
                .Session(CalculatorDoc.MainDoc(viewSessionCalculator, varSessionCalculator, page, id: SessionDoc))
                .Database(CalculatorDoc.MainDoc(viewDatabaseCalculator, varDatabaseCalculator, page, id: DatabaseDoc))
                .Doc();
        }
    }
}