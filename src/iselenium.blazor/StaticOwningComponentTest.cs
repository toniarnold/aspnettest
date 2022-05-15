using asplib.Components;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Test fixture for a StaticOwningComponentBase Component. The TestFocus is
    /// set to the Component type and its instance is assigned to the Component
    /// property at OnAfterRenderAsync. The owned Service instance is exposed on
    /// the proparty Main.
    /// </summary>
    /// <typeparam name="TWebDriver"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    /// <typeparam name="TMain"></typeparam>
    public class StaticOwningComponentTest<TWebDriver, TComponent, TMain> : StaticComponentTest<TWebDriver, TComponent>
        where TWebDriver : IWebDriver, new()
        where TComponent : StaticOwningComponentBase<TMain>
        where TMain : class, new()
    {
        public TMain Main => ((StaticOwningComponentBase<TMain>)TestFocus.Component).Main;
    }
}