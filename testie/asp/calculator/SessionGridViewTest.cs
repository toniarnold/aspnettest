using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

using NUnit.Framework;

using iie;

using asplib.View;

namespace testie.asp.calculator
{
    /// <summary>
    /// Test the sessionDumpGridView functionality in Main.ascx
    /// Requires Storage.Database
    /// </summary>
    [Category("SHDocVw.InternetExplorer")]
    public class SessionGridViewTest : TestBase
    {
        private int rowCountBefore = 0;


        [OneTimeSetUp]
        public override void SetUpStorage()
        {
            ControlMainExtension.SessionStorage = Storage.Database;
            this.ClearSession();
            this.Navigate("/asp/default.aspx");
        }

        [OneTimeTearDown]
        public void ClearSession()
        {
            this.Navigate("/asp/default.aspx?clear=true&endresponse=true");
        }

        [SetUp]
        public void RowCount()
        {
            
        }

        // Accessors for the control under test
        private GridView GridView
        {
            get { return (GridView)this.GetControl("sessionDumpGridView"); }
        }


        [Test]
        public void InsertOrphaneDeleteTest()
        {
            // Initialize (eventually with a new cookie/row)
            this.Navigate("/asp/default.aspx");
            this.rowCountBefore = this.GridView.Rows.Count;

            // Create a unique test number to store
            var rnd = new Random();
            var unique = rnd.NextDouble().ToString();
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            this.Write("enter.operandTextBox", unique);
            this.Click("footer.enterButton");
            Assert.That(this.Stack, Does.Contain(unique));
            Assert.That(this.GridView.Rows.Count, Is.EqualTo(this.rowCountBefore));

            // Click on the hamburger to make that number optically visible 
            // and find the row containing the number
            this.Click("hamburgerDiv", expectPostBack: false);
            var row = this.SelectRowContainig(unique);
            Assert.That(row, Is.Not.Null);


            // Throw an exception to create a session dump row (as a duplicate) and return
            this.Click("footer.enterButton");
            this.Write("enter.operandTextBox", "except");
            this.Click("footer.enterButton", expectedStatusCode: 500);
            Assert.That(this.Html(), Does.Contain("Deliberate Exception"));
            this.Navigate("/asp/default.aspx");
            this.Click("hamburgerDiv", expectPostBack: false);
            Assert.That(this.GridView.Rows.Count, Is.EqualTo(this.rowCountBefore + 1));

            // Calculate to make our old unique row in the session dump unique again
            Assert.That(this.Stack, Does.Contain(unique));
            this.Write("enter.operandTextBox", "0");
            this.Click("footer.enterButton");
            this.Click("calculate.clrButton");
            this.Click("calculate.sqrtButton");
            Assert.That(this.Stack, Does.Not.Contain(unique));

            // Click on the link button in the old row to retrieve the old value in the current stack
            this.Click("hamburgerDiv", expectPostBack: false);
            row = this.SelectRowContainig(unique);
            var link = row.FindControl("linkHyperLink");
            this.Click(link);
            Assert.That(this.Stack, Does.Contain(unique));
            Assert.That(this.GridView.Rows.Count, Is.EqualTo(this.rowCountBefore + 1)); // unchanged
            // Now we have a duplicate (dump and current), this clear the current instance
            this.Write("enter.operandTextBox", "0");
            this.Click("footer.enterButton");
            this.Click("calculate.clrAllButton");
            Assert.That(this.Stack, Does.Not.Contain(unique));

            // At last click on the delete button to delete the dump row
            this.Click("hamburgerDiv", expectPostBack: false);
            row = this.SelectRowContainig(unique);
            var delete = row.FindControl("deleteLinkButton");
            // Partial PostBack does not trigger DocumentComplete, 
            // as a fall back just wait for the row to disappear
            this.Click(delete, expectPostBack: false, pause: 100);  
            Assert.That(this.GridView.Rows.Count, Is.EqualTo(this.rowCountBefore));  // as in the beginning<
        }

        private GridViewRow SelectRowContainig(string substr)
        {
            return (from GridViewRow r in this.GridView.Rows
                    where (
                      from TableCell c in r.Cells
                      where c.FindControl("stackLabel") != null &&
                            ((Label)c.FindControl("stackLabel")).Text.Contains(substr)
                      select c).FirstOrDefault() != null
                    select r).FirstOrDefault();
        }
    }
}
