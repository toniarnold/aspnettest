using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using asplib.View;

using asp.calculator.Control;


namespace asp.calculator.View
{
    public partial class Error : CalculatorControl
    { 
        public string Msg { get; set; }
    }
}
