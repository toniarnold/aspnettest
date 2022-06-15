using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace iselenium
{
    /// <summary>
    /// Test fixture for any Blazor Component. No synchronization with
    /// TestFocus takes place, thus tests must resort to AssertPoll()
    /// </summary>
    /// <typeparam name="TWebDriver"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public abstract class ComponentTest<TWebDriver, TComponent> : SpaTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// Got rid of the AssertPoll() SPA abracadabra in Blazor
        /// </summary>
        [OneTimeSetUp]
        public void NoAwaitRemoved()
        {
            this.awaitRemovedDefault = false;
        }

        /// <summary>
        /// Without PollHttpOK() abracadabra for WebSharper
        /// </summary>
        public override void OneTimeSetUpBrowser()
        {
            this.SetUpBrowser<TWebDriver>();
        }

        /// <summary>
        /// Non-nullable typed accessor function to get the DynamicComponent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public T Dynamic<T>(DynamicComponent component)
        {
            Trace.Assert(component.Instance != null,
                $"DynamicComponent.Instance for {typeof(T)} is null");
            object dynamicObject = component.Instance ?? default!;
            Trace.Assert(typeof(T).IsAssignableFrom(dynamicObject.GetType()),
                $"{dynamicObject.GetType()} is not of type {typeof(T)}");
            return (T)dynamicObject;
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
        public virtual void Navigate(string path, int expectedStatusCode = 200, int delay = 0, int pause = 0, bool expectRender = true)
        {
            SeleniumExtensionBase.Navigate(this, path, expectedStatusCode, delay, pause);
        }

        #region Click

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
        /// <param name="expectRender">Set to false if TestFocus.Event is not set on OnAfterRenderAsync</param>
        public virtual void Click(Func<string, By> selector, string selectString, int index = 0,
                            bool expectRequest = false, bool? awaitRemoved = null,
                            int expectedStatusCode = 200, int delay = 0, int pause = 0, int wait = 0,
                            bool expectRender = true,
                            bool expectRerender = false)
        {
            var doAwaitRemoved = awaitRemoved ?? this.awaitRemovedDefault;
            SeleniumExtensionBase.Click(this, selector, selectString, index: 0,
                                            expectRequest: expectRequest, samePage: false, // In Blazor the "same" page receives new Ids
                                            awaitRemoved: doAwaitRemoved, expectedStatusCode: expectedStatusCode,
                                            delay: delay, pause: pause,
                                            wait: (wait == 0) ? SeleniumExtensionBase.RequestTimeout : wait);
        }

        /// <summary>
        /// Click the HTML element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        /// <param name="expectRerender"></param>
        public void Click(ElementReference element, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true, bool expectRerender = false)
        {
            Click(By.CssSelector, $"*[{element.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender, expectRerender: expectRerender);
        }

        // The built-in components with an .Element property don't expose that as interface or by inheritance -> enumerate them explicitly:

        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click(InputCheckbox component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click<T>(InputDate<T> component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click(InputFile component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click<T>(InputNumber<T> component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click<T>(InputRadio<T> component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }
#endif

        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click<T>(InputSelect<T> component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click(InputText component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        /// <summary>
        /// Click the @ref component
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="expectRequest"></param>
        /// <param name="expectedStatusCode"></param>
        /// <param name="expectRender"></param>
        public void Click(InputTextArea component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        #endregion Click

        #region Write

        /// <summary>
        /// Write to a HTML element (usually a text input) given by the ElementReference
        /// </summary>
        /// <param name="element">Blazor ElementReference of the HTML element</param>
        /// <param name="text"></param>
        /// <param name="throttle"></param>
        public void Write(ElementReference element, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{element.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        /// <summary>
        /// Write to the @ref component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="text"></param>
        /// <param name="throttle"></param>
        public void Write<T>(InputDate<T> component, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{component.Element?.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        /// <summary>
        /// Write to the @ref component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="text"></param>
        /// <param name="throttle"></param>
        public void Write<T>(InputNumber<T> component, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{component.Element?.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        /// <summary>
        /// Write to the @ref component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="text"></param>
        /// <param name="throttle"></param>
        public void Write(InputText component, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{component.Element?.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        /// <summary>
        /// Write to the @ref component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The @ref component instance.</param>
        /// <param name="text"></param>
        /// <param name="throttle"></param>
        public void Write(InputTextArea component, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{component.Element?.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        #endregion Write

        #region Find

        /// <summary>
        /// bUnit-like function to get a the first Selenium IWebElement found by the cssSelector
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public IWebElement Find(string cssSelector)
        {
            return SeleniumExtensionBase.GetHTMLElement(this, By.CssSelector, cssSelector);
        }

        /// <summary>
        /// bUnit-like function to get all the Selenium IWebElements found by the cssSelector
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public ReadOnlyCollection<IWebElement> FindAll(string cssSelector)
        {
            return SeleniumExtensionBase.GetHTMLElements(this, By.CssSelector, cssSelector);
        }

        /// <summary>
        /// bUnit-like function to get a the first Selenium IWebElement found by the selectString
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="selectString"></param>
        /// <returns></returns>
        public IWebElement Find(Func<string, By> selector, string selectString)
        {
            return SeleniumExtensionBase.GetHTMLElement(this, selector, selectString);
        }

        /// <summary>
        /// bUnit-like function to get all the Selenium IWebElements found by the selectString
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="selectString"></param>
        /// <returns></returns>
        public ReadOnlyCollection<IWebElement> FindAll(Func<string, By> selector, string selectString)
        {
            return SeleniumExtensionBase.GetHTMLElements(this, selector, selectString);
        }

        /// <summary>
        /// Get the @ref element as Selenium IWebElement
        /// </summary>
        /// <param name="element">The @ref element instance.</param>
        /// <returns></returns>
        public IWebElement Find(ElementReference element)
        {
            return Find(By.CssSelector, $"*[{element.IdAttr()}]");
        }

        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find(InputCheckbox component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find<T>(InputDate<T> component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find(InputFile component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find<T>(InputNumber<T> component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find<T>(InputRadio<T> component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

#endif

        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find<T>(InputSelect<T> component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find(InputText component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

        /// <summary>
        /// Get the Selenium IWebElement for the @ref component instance
        /// </summary>
        /// <param name="component">The @ref component instance.</param>
        /// <returns></returns>
        public IWebElement Find(InputTextArea component)
        {
            return Find(By.CssSelector, $"*[{component.Element?.IdAttr()}]");
        }

        #endregion Find
    }
}