using asp.websharper.spa.Model;
using asplib.Model;
using asplib.Remoting;
using System.Threading.Tasks;
using WebSharper;

namespace asp.websharper.spa.Remoting
{
    /// <summary>
    /// Remoting server for the FSM task class.
    /// All public FSM state transitions with their arguments are mirrored as
    /// remote methods. Additionally provides a parameterless Load()
    /// constructor for creating an initial persistent instance in the
    /// SPAEntryPoint via StorageServer.Load() in absence of a ViewState.
    /// The "using { }" block guarantees that the changed object state always
    /// gets saved.
    /// </summary>
    public static class CalculatorServer
    {
        /// <summary>
        /// Static reference to the Model fur NUnit test assertions
        /// </summary>
        public static CalculatorViewModel CalculatorViewModel;

        /// <summary>
        /// Initializes a new Calculator by loading it from the null viewState
        /// </summary>
        /// <returns>New FSM instance</returns>
        [Remote]
        public static Task<CalculatorViewModel> Load()
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(null, out CalculatorViewModel))
            {
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        /// <summary>
        /// Initializes a new Calculator with a specific session storage.
        /// Separate method as an optional parameter produces
        /// "System.Exception: RPC parameter not received a single element
        /// array" on startup.
        /// </summary>
        /// <param name="sessionStorage">The session storage.</param>
        /// <returns></returns>
        [Remote]
        public static Task<CalculatorViewModel> Load(Storage sessionStorage)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(null, out CalculatorViewModel, sessionStorage))
            {
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Enter(string viewState, string value)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Enter(value);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Add(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Add(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Sub(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Sub(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Mul(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Mul(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Div(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Div(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Pow(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Pow(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Sqrt(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Sqrt(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> Clr(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.Clr(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }

        [Remote]
        public static Task<CalculatorViewModel> ClrAll(string viewState)
        {
            using (var calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, out CalculatorViewModel))
            {
                calculator.Fsm.ClrAll(calculator.Stack);
                return calculator.ViewModelTask<Calculator, CalculatorViewModel>();
            }
        }
    }
}