using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using NUnit.Framework;

using iie;

using asplib.View;

namespace testie.asp
{
    /// <summary>
    /// Extends CalculatorTest by using Session instead of Viewstate as storage
    /// and executes the same tests declared in the base class.
    /// </summary>
    [Category("SHDocVw.InternetExplorer")]
    public class CalculatorSessionTest : CalculatorTest
    {
        [SetUp]
        public override void SetUpStorage()
        {
            ControlMainExtension.SessionStorage = Storage.Session;
        }

        /// <summary>
        /// Session must be cleared after each single test such that the app behaves like the Viewstate Test
        /// When debugging, HttpContext.Current may already have disappeared.
        /// </summary>
        [TearDown]
        public void ClearSession()
        {
            this.Navigate("/asp/clear.aspx?storage=Session");
        }
    }
}
