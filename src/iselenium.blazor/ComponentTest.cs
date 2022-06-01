using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NUnit.Framework;
using OpenQA.Selenium;

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
        /// <param name="component"></param>
        /// <returns></returns>
        public T Dynamic<T>(DynamicComponent component)
        {
            Assert.That(component.Instance, Is.Not.Null);
            object dynamicObject = component.Instance ?? default!;
            Assert.That(dynamicObject, Is.AssignableTo<T>());
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

        public void Click(ElementReference element, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true, bool expectRerender = false)
        {
            Click(By.CssSelector, $"*[{element.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender, expectRerender: expectRerender);
        }

        // The built-in components with an .Element property don't expose that as interface or by inheritance -> enumerate them explicitly:

        public void Click(InputCheckbox component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        public void Click<T>(InputDate<T> component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        public void Click(InputFile component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        public void Click<T>(InputNumber<T> component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        //public void Click<T>(InputRadio<T> component,bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        //{
        //    Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest,awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        //}

        public void Click<T>(InputSelect<T> component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        public void Click(InputText component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        public void Click(InputTextArea component, bool expectRequest = false, int expectedStatusCode = 200, bool expectRender = true)
        {
            Click(By.CssSelector, $"*[{component.Element?.IdAttr()}]", index: 0, expectRequest, awaitRemoved: null, expectedStatusCode, delay: 0, pause: 0, wait: 0, expectRender: expectRender);
        }

        /// <summary>
        /// Write to  HTML element (usually a text input) given by the ElementReference
        /// </summary>
        /// <param name="element">Blazor ElementReference of the HTML element</param>
        /// <param name="text"></param>
        /// <param name="throttle"></param>
        public void Write(ElementReference element, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{element.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        public void Write<T>(InputNumber<T> component, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{component.Element?.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        public void Write(InputText component, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{component.Element?.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }

        public void Write(InputTextArea component, string text, int throttle = 0)
        {
            SeleniumExtensionBase.Write(this, By.CssSelector, $"*[{component.Element?.IdAttr()}]", text, index: 0, wait: 0, throttle);
        }
    }
}