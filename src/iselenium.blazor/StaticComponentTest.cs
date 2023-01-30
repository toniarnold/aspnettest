using asplib.Components;
using NUnit.Framework;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Test fixture for a StaticComponentBase Component. The TestFocus is set
    /// to the Component type and its instance is assigned to the Component
    /// property at OnAfterRenderAsync.
    /// </summary>
    /// <typeparam name="TWebDriver"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public class StaticComponentTest<TWebDriver, TComponent> : ComponentTest<TWebDriver, TComponent>
        where TWebDriver : IWebDriver, new()
        where TComponent : ITestFocus
    {
        /// <summary>
        /// Accessor for the Component in focus
        /// </summary>
        [Obsolete("Cut for 'component under test' has become a well known convention")]
        public TComponent Component => (TComponent?)TestFocus.Component ?? default!;

        // Reference for using "Cut": https://learn.microsoft.com/en-us/aspnet/core/blazor/test

        /// <summary>
        /// Accessor for the 'component under test' in focus
        /// </summary>
        public TComponent Cut => (TComponent?)TestFocus.Component ?? default!;

        [SetUp]
        public void SetFocus()
        {
            TestFocus.SetFocus(typeof(TComponent));
        }

        [TearDown]
        public void RemoveFocus()
        {
            TestFocus.RemoveFocus();
        }

        /// <summary>
        /// Navigate to a StaticComponentBase<T> and signal TestFocus.Event to
        /// continue OnAfterRenderAsync if expectRenders is > 0.
        /// </summary>
        /// <param name="path">Path part of the URL (without Domain)</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in seconds before navigating to the url</param>
        /// <param name="pause">Optional pause time in seconds after Selenium claims DocumentComplete when expectRenders > 0</param>
        /// <param name="expectRenders">Number of renderings where TestFocus.Event is set OnAfterRenderAsync</param>
        public override void Navigate(string path, int expectedStatusCode = 200, int delay = 0, int pause = 0, int expectRenders = 1)
        {
            if (expectRenders > 0)
            {
                TestFocus.Event.Reset();
            }
            SeleniumExtensionBase.Navigate(this, path, expectedStatusCode, delay, pause);
            while (expectRenders > 0)
            {
                if (!TestFocus.Event.WaitOne(SeleniumExtensionBase.RequestTimeout * 1000))
                {
                    throw new TimeoutException($"Navigate({path}): TestFocus.Event not signaled");
                }
                expectRenders--;
            }
        }

        /// <summary>
        /// Synchronized click on the HTML element, waits for a response when expectRequest is true and
        /// for TestFocus.Event getting signaled OnAfterRenderAsync when expectRenders is > 0.
        /// is true.
        /// </summary>
        /// <param name="selector">Selenium By selector function</param>
        /// <param name="selectString">string passed to the selector function</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="awaitRemoved">Whether to wait for the HTML element to disappear (in an SPA)</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>///
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after Selenium claims DocumentComplete (omit if expectRenders is > 0)</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear. 0 when expectRenders is > 0</param>
        /// <param name="expectRenders">Number of renderings where TestFocus.Event is set OnAfterRenderAsync</param>
        /// <param name="expectRerender">Set to true for awaiting a re-render which sets TestFocus.AwaitingRerender = false, as e.g. SmcComponentBase</param>
        public override void Click(Func<string, By> selector, string selectString, int index = 0,
                            bool expectRequest = false, bool? awaitRemoved = null,
                            int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0,
                            int expectRenders = 1,
                            bool expectRerender = false)
        {
            var doAwaitRemoved = awaitRemoved ?? this.awaitRemovedDefault;
            if (expectRenders > 0)
            {
                if (expectRequest || expectRerender)
                {
                    TestFocus.AwaitingRerender = true;
                }
                TestFocus.Event.Reset();    // defensive, should have been AutoReset
            }
            SeleniumExtensionBase.Click(this, selector, selectString, index: 0,
                                            expectRequest: expectRequest, samePage: false, // In Blazor the "same" page receives new Ids
                                            awaitRemoved: doAwaitRemoved, expectedStatusCode: expectedStatusCode,
                                            delay: delay, pause: pause,
                                            wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
            while (expectRenders > 0)
            {
                if (!TestFocus.Event.WaitOne(SeleniumExtensionBase.RequestTimeout * 1000))
                {
                    throw new TimeoutException($"Click({selectString}): TestFocus.Event not signaled");
                }
                expectRenders--;
            }
        }

        /// <summary>
        /// Synchronized page reload, waits for TestFocus.Event getting signaled
        /// OnAfterRenderAsync when expectRenders is > 0.
        /// </summary>
        /// <param name="expectRenders">Number of renderings where TestFocus.Event is set OnAfterRenderAsync</param>
        /// <param name="pause">Optional pause time in milliseconds after Selenium claims DocumentComplete</param>
        public void Refresh(int expectRenders = 1, int pause = 0)
        {
            if (expectRenders > 0)
            {
                TestFocus.AwaitingRerender = true;
                TestFocus.Event.Reset();
            }
            SeleniumExtensionBase.Refresh(this, pause: pause);
            while (expectRenders > 0)
            {
                if (!TestFocus.Event.WaitOne(SeleniumExtensionBase.RequestTimeout * 1000))
                {
                    throw new TimeoutException("Refresh(): TestFocus.Event not signaled");
                }
                expectRenders--;
            }
        }

#pragma warning disable CS0809 // obsolete member (WebSharper)

        [Obsolete("For Blazor, you probably want synchronized Click(By.Id, someId), if intended asynchronously use it with expectRenders: 0", true)]
        public override void Click(string id, int index = 0,
                            bool expectRequest = false, bool? samePage = null, bool? awaitRemoved = null,
                            int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0)
#pragma warning restore CS0809 // obsolete member (WebSharper)
        {
            base.Click(id, index, expectRequest, samePage, awaitRemoved, expectedStatusCode, delay, wait);
        }
    }
}