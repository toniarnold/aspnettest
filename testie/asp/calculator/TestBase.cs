using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using iie;

using asplib.Model;
using asplib.View;

using asp.calculator.Control;


namespace testie.asp.calculator
{
    /// <summary>
    /// Base class with typed accessors for the Calculator and and the SetUpIE()/TearDownIE() pair
    /// </summary>
    public abstract class TestBase : IIE
    {
        protected long max_mainid;

        protected ISmcControl<Calculator, CalculatorContext, CalculatorContext.CalculatorState> MainControl
        {
            get
            {
                return (ISmcControl<Calculator, CalculatorContext, CalculatorContext.CalculatorState>)
                          ControlRootExtension.RootControl;
            }
        }

        protected Calculator Main
        {
            get { return this.MainControl.Main; }
        }

        protected Stack<string> Stack
        {
            get { return this.MainControl.Main.Stack; }
        }

        protected CalculatorContext.CalculatorState State
        {
            get { return this.MainControl.State; }
        }


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (var db = new ASP_DBEntities())
            {
                var sql = @"
                    SELECT MAX(mainid) 
                    FROM Main
                    ";
                this.max_mainid = db.Database.SqlQuery<long>(sql).FirstOrDefault();
            }

            this.SetUpIE();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.TearDownIE();

            using (var db = new ASP_DBEntities())
            {
                var sql = @"
                    DELETE FROM Main
                    WHERE mainid > @max_mainid
                ";
                var param = new SqlParameter("max_mainid", this.max_mainid);
                db.Database.ExecuteSqlCommand(sql, param);
            }
        }

        [SetUp]
        public virtual void SetUpStorage()
        {
            ControlStorageExtension.SessionStorage = Storage.Viewstate;    // default, no special TearDown required
        }
    }
}
