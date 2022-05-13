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
        where TComponent : IStaticComponent
    {
        /// <summary>
        /// Accessor for the Component in focus
        /// </summary>
        public TComponent? Component => (TComponent?)TestFocus.Component;

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
        /// Navigate to a StaticComponentBase<T> which signals TestFocus.Event
        /// on OnAfterRenderAsync to continue
        /// </summary>
        /// <param name="path">Path part of the URL (without Domain)</param>
        /// <param name="expectedStatusCode">Expected StatusCode of the response</param>
        /// <param name="delay">Optional delay time in seconds before navigating to the url</param>
        /// <param name="pause">Optional pause time in seconds after Selenium claims DocumentComplete when expectRender is false</param>
        /// <param name="expectRender">Set to false if TestFocus.Event is not set on OnAfterRenderAsync</param>
        public override void Navigate(string path, int expectedStatusCode = 200, int delay = 0, int pause = 0, bool expectRender = true)
        {
            if (expectRender)
            {
                TestFocus.Event.Reset();    // Reset to "block" state after unrelated renders
            }
            SeleniumExtensionBase.Navigate(this, path, expectedStatusCode, delay, pause);
            if (expectRender)
            {
                // Navigate returns (possibly) before the page is fully rendered.
                // If not, the Event is already in signaled state and lets the Wait pass.
                TestFocus.Event.WaitOne(SeleniumExtensionBase.RequestTimeout * 1000);
            }
        }

        /// <summary>
        /// Click the HTML element and wait for the response when expectRender
        /// is true.
        /// </summary>
        /// <param name="selector">Selenium By selector function</param>
        /// <param name="selector">string passed to the selector function</param>
        /// <param name="expectRequest">Whether to expect a GET/POST request to the server from the click</param>
        /// <param name="awaitRemoved">Whether to wait for the HTML element to disappear (in an SPA)</param>
        /// <param name="expectedStatusCode">Expected StatusCofe of the response</param>///
        /// <param name="delay">Optional delay time in milliseconds before clicking the element</param>
        /// <param name="pause">Optional pause time in milliseconds after IE claims DocumentComplete (set to 0 if expectRender)</param>
        /// <param name="wait">Explicit WebDriverWait in seconds for the element to appear. 0 when expectRender is true</param>
        /// <param name="expectRender">Set to false if TestFocus.Event is not set onOnAfterRenderAsync</param>
        public override void Click(Func<string, By> selector, string selectString, int index = 0,
                            bool expectRequest = false, bool? awaitRemoved = null,
                            int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0, bool expectRender = true)
        {
            var doAwaitRemoved = awaitRemoved ?? this.awaitRemovedDefault;
            if (expectRender)
            {
                TestFocus.Event.Reset();    // Reset to "block" state after unrelated renders
            }
            SeleniumExtensionBase.Click(this, selector, selectString, index: 0,
                                            expectRequest: expectRequest, samePage: false, // In Blazor the "same" page receives new Ids
                                            awaitRemoved: doAwaitRemoved, expectedStatusCode: expectedStatusCode,
                                            delay: delay, pause: pause,
                                            wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
            if (expectRender && !expectRequest)
            {
                TestFocus.Event.WaitOne(SeleniumExtensionBase.RequestTimeout * 1000);
            }
        }
    }
}