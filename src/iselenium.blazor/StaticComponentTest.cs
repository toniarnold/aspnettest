using asplib.Components;
using OpenQA.Selenium;

namespace iselenium
{
    public class StaticComponentTest<TWebDriver, TComponent, TMain> : ComponentTest<TWebDriver, TComponent>
        where TWebDriver : IWebDriver, new()
        where TComponent : StaticComponentBase<TMain>
        where TMain : class, new()
    {
        public TMain? Main => ((StaticComponentBase<TMain>?)TestFocus.Component)?.Main;
    }
}