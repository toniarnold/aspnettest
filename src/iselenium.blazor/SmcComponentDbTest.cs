using asplib.Components;
using asplib.Model;
using OpenQA.Selenium;

namespace iselenium
{
    public class SmcComponentDbTest<TWebDriver, TComponent, TAppClass, TFSMContext, TState> : StaticOwningComponentDbTest<TWebDriver, TComponent, TAppClass>
        where TWebDriver : IWebDriver, new()
        where TComponent : SmcComponentBase<TAppClass, TFSMContext, TState>
        where TAppClass : class, IAppClass<TFSMContext, TState>, new()
        where TFSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        protected TFSMContext? Fsm => Main?.Fsm;
        protected TState? State => Main?.State;
    }
}