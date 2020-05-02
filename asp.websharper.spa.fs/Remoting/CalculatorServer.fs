module asp.websharper.spa.fs.Server

open asp.websharper.spa.Model
open asp.websharper.spa.fs.Model
open asplib.Model
open asplib.Remoting
open WebSharper

module CalculatorServer =

    let mutable Calculator = new Calculator()

    [<Rpc>]
    let LoadNew () =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(null, &Calculator)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Load sessionStorage =
        use calculator =  StorageServer.Load<Calculator, CalculatorViewModel>(null, &Calculator, sessionStorage)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Enter (viewState, value) =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Enter(value)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Add viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Add(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Sub viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Sub(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Mul viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Add(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Div viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Div(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Pow viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Pow(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Sqrt viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Sqrt(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let Clr viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.Clr(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()

    [<Rpc>]
    let ClrAll viewState =
        use calculator = StorageServer.Load<Calculator, CalculatorViewModel>(viewState, &Calculator)
        calculator.Fsm.ClrAll(calculator.Stack)
        calculator.ViewModelTask<Calculator, CalculatorViewModel>()