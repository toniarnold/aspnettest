using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Threading;

namespace iselenium
{
    /// <summary>
    /// AssertPoll corresponds to Assert.That, but quickly polls for
    /// RequestTimeout seconds until the assertion passes.
    /// </summary>
    public static partial class SeleniumExtensionBase
    {
        #region NUnit Assert.That signatures rewritten as AssertPoll extension methods

        // Taken from nunit\src\NUnitFramework\framework\Assert.That.cs
        // and back-ported from C# 8.0 to C# 7.3 for .NET Standard 2.0

        // ***********************************************************************
        // Copyright (c) 2011 Charlie Poole, Rob Prouse
        //
        // Permission is hereby granted, free of charge, to any person obtaining
        // a copy of this software and associated documentation files (the
        // "Software"), to deal in the Software without restriction, including
        // without limitation the rights to use, copy, modify, merge, publish,
        // distribute, sublicense, and/or sell copies of the Software, and to
        // permit persons to whom the Software is furnished to do so, subject to
        // the following conditions:
        //
        // The above copyright notice and this permission notice shall be
        // included in all copies or substantial portions of the Software.
        //
        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
        // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
        // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
        // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
        // LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
        // OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
        // WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        // ***********************************************************************

        #region ActualValueDelegate

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        public static void AssertPoll<TActual>(this ISeleniumBase _, ActualValueDelegate<TActual> del, Func<IResolveConstraint> expr)
        {
            AssertPoll(del, expr, null, null);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void That<TActual>(ActualValueDelegate<TActual> del, Func<IResolveConstraint> expr, string message, params object[] args)
        {
            AssertPoll(del, expr, message, args);
        }

        /// <summary>
        /// Apply a constraint to a delegate. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        /// <typeparam name="TActual">The Type being compared.</typeparam>
        /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
        /// <param name="expr">A Constraint expression to be applied</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void That<TActual>(
            ActualValueDelegate<TActual> del,
            Func<IResolveConstraint> expr,
            Func<string> getExceptionMessage)
        {
            AssertPoll(del, expr, getExceptionMessage());
        }

        #endregion ActualValueDelegate

        #endregion NUnit Assert.That signatures rewritten as AssertPoll extension methods

        #region Private AssertPoll implementations

        private static void AssertPoll<TActual>(ActualValueDelegate<TActual> del, Func<IResolveConstraint> expression, string message, params object[] args)
        {
            TryAssertPoll(del, expression);
            Assert.That(del, expression(), message, args);
        }

        private static void AssertPoll<TActual>(ActualValueDelegate<TActual> del, Func<IResolveConstraint> expression, Func<string> getExceptionMessage)
        {
            TryAssertPoll(del, expression);
            Assert.That(del, expression(), getExceptionMessage);
        }

        /// <summary>
        /// Truncated Assert.That implementation throwing no exceptions on failure
        /// </summary>
        /// <typeparam name="TActual"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expression"></param>
        /// <returns>whether the tess passed</returns>
        private static bool TryAssertPoll<TActual>(ActualValueDelegate<TActual> del, Func<IResolveConstraint> expression)
        {
            var constraint = expression().Resolve();
            for (int i = 0; i < RequestTimeout * 1000 / FAST_POLL_MILLISECONDS; i++)
            {
                try
                {
                    var result = constraint.ApplyTo(del);
                    if (result.IsSuccess)
                        return true;    // break on success
                }
                catch { }
                Thread.Sleep(FAST_POLL_MILLISECONDS);
            }   // continue with attempts until IsSuccess or RequestTimeout
            return false;   // return on failure
        }

        #endregion Private AssertPoll implementations
    }
}