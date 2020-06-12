using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace test.iselenium
{
    [TestFixture]
    public class ISeleniumExtensionBaseAssertPollTest : ISelenium
    {
        #region ISeleniumBase

#pragma warning disable IDE1006 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }
        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006

        #endregion ISeleniumBase

        protected static int tries;   // for  [Retry()]

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            tries = 0;
            SeleniumExtensionBase.RequestTimeout = 1;   // 3 tries for 1 second, the last one succeeds
        }

        [Test]
        [Retry(3)]
        public void Test()
        {
            Assert.Multiple(() =>
            {
                this.AssertPoll(() => { return tries; }, () => Is.EqualTo(2));
                tries++;
            });
        }
    }

    [TestFixture]
    public class AssertPollExactCoutnOperatorTest : ISelenium
    {
        #region ISeleniumBase

#pragma warning disable IDE1006 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }
        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006

        #endregion ISeleniumBase

        [Test]
        public void Test()
        {
            // Formerly (without constraint lambda, which turned out to be required anyway for polling):
            // this.AssertPoll(new int[]{ 1, 2, 3 }, Has.Exactly(3).Items);

            // Problem: Directly passing an already evaluated IResolveConstraint
            // down to the truncated TryAssertPoll method which in turn calls
            // expression.Resolve() resulted in a
            // "System.InvalidOperationException : Stack empty." thrown in
            // NUnit's ConstraintBuilder class in the IConstraint Pop() method.

            // This problem arises when using ExactCountConstraint, but not in
            // simpler expressions like Is.Null. It can be reproduced in the
            // nunit.framework project itself by writing an unit test with
            // above assertion, placing a break point in the Assert class in:
            // nunit\src\NUnitFramework\framework\Assert.That.cs in the That()
            // method on the assignment "var constraint = expression.Resolve();"
            // Additionally place two break points in the ConstraintStack class in:
            // nunit\src\NUnitFramework\framework\Constraints\ConstraintBuilder.cs
            // on the two methods Push(IConstraint constraint) and IConstraint Pop()

            // After the program pauses on expression.Resolve() and continuing,
            // the Push() method will not be called and the test fails in
            // ThrowForEmptyStack() with above InvalidOperationException.

            // Deactivate the first break point such that the program stops only
            // on Push()/Pop(), and the test passes.

            this.AssertPoll(() => new int[] { 1, 2, 3 }, () => Has.Exactly(3).Items);
        }
    }
}