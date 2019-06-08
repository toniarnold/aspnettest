using asp.websharper.spa.Model;
using asplib.Model;
using asplib.Remoting;
using System.Threading.Tasks;
using WebSharper;

namespace asp.websharper.spa.Remoting
{
    public static class CalculatorRemoting
    {
        // Static reference to the Model
        public static Calculator Calculator;

        /// <summary>
        /// Initializes a new Calculator by loading it from the null viewState
        /// </summary>
        /// <returns></returns>
        [Remote]
        public static Task<CalculatorViewModel> New()
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(null, out Calculator))
            {
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Enter(string viewState, string value)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Enter(value);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Add(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Add(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Sub(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Sub(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Mul(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Mul(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Div(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Div(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Pow(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Pow(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Sqrt(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Sqrt(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Clr(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.Clr(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> ClrAll(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out Calculator))
            {
                calculator.Fsm.ClrAll(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }
    }
}