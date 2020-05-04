using asplib.Model;
using System.Collections.Generic;
using WebSharper;

namespace asp.websharper.spa.Model
{
    /// <summary>
    /// Stored ViewModel for the calculator, passed back from remoting methods
    /// </summary>
    public class CalculatorViewModel : SmcViewModel<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
    {
        /// General: SMC state as enumeration
        public Map1 Map1;

        /// Specific for Calculator: Make its Stack visible
        public List<string> Stack;

        [JavaScript]
        public CalculatorViewModel() : base()
        {
        }

        public override void LoadMembers()
        {
            base.LoadMembers();
            this.Stack = new List<string>(this.Main.Stack);
        }

        protected override void LoadStateNames()
        {
            this.Map1 = new Map1()
            {
                Splash = CalculatorContext.Map1.Splash.Name,
                Enter = CalculatorContext.Map1.Enter.Name,
                Calculate = CalculatorContext.Map1.Calculate.Name,
                ErrorNumeric = CalculatorContext.Map1.ErrorNumeric.Name,
                ErrorTuple = CalculatorContext.Map1.ErrorTuple.Name,
                ErrorEmpty = CalculatorContext.Map1.ErrorEmpty.Name,
            };
        }
    }

    /// <summary>
    /// Typed state names for the context class must be retrieved from the server
    /// </summary>
    [JavaScript]
    public class Map1
    {
        public string Splash;
        public string Enter;
        public string Calculate;
        public string ErrorNumeric;
        public string ErrorTuple;
        public string ErrorEmpty;
    }
}