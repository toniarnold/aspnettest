namespace asp.websharper.spa.fs

open asplib.Model
open asp.websharper.spa.Model
open System.Collections.Generic
open WebSharper

module Model =

    /// Typed state name structure for easy initialization with [<DefaultValue>]
    [<JavaScript>]
    type Map1 =
        {
            Splash : string
            Enter : string
            Calculate : string
            ErrorNumeric : string
            ErrorTuple : string
            ErrorEmpty : string
        }

    /// The ViewModel is primarily running on the server
    /// and explicitly exposes some fields/properties to the client
    [<JavaScript>]
    type CalculatorViewModel() =
        inherit SmcViewModel<Calculator, CalculatorContext, CalculatorContext.CalculatorState>()

        /// General: SMC state as mutable enumeration
        [<DefaultValue>] val mutable Map1 : Map1
        /// Specific: Stack of the calculator
        member val Stack = new List<string>() with get, set

        [<JavaScript false>]
        override this.LoadMembers() =
            base.LoadMembers()
            this.State <- this.Main.State.ToString()
            this.Map1 <-
                {
                    Splash = CalculatorContext.Map1.Splash.Name
                    Enter = CalculatorContext.Map1.Enter.Name
                    Calculate = CalculatorContext.Map1.Calculate.Name
                    ErrorNumeric = CalculatorContext.Map1.ErrorNumeric.Name
                    ErrorTuple = CalculatorContext.Map1.ErrorTuple.Name
                    ErrorEmpty = CalculatorContext.Map1.ErrorEmpty.Name
                }
            this.Stack <- new List<string>(this.Main.Stack)