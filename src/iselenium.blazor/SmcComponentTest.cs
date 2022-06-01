using asplib.Components;
using asplib.Model;
using Microsoft.AspNetCore.Components;
using OpenQA.Selenium;

namespace iselenium
{
    public class SmcComponentTest<TWebDriver, TComponent, TAppClass, TFSMContext, TState> : StaticOwningComponentTest<TWebDriver, TComponent, TAppClass>
        where TWebDriver : IWebDriver, new()
        where TComponent : SmcComponentBase<TAppClass, TFSMContext, TState>
        where TAppClass : class, IAppClass<TFSMContext, TState>, new()
        where TFSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        protected TFSMContext? Fsm => Main?.Fsm;
        protected TState? State => Main?.State;

        /// <summary>
        /// SMC State Transition Click
        /// </summary>
        /// <param name="element"></param>
        /// <param name="expectRerender">Set to true for awaiting a re-render which sets TestFocus.AwaitingRerender = false, as e.g. SmcComponentBase</param>
        public void Click(ElementReference element, bool expectRerender = true)
        {
            base.Click(element, expectRerender: expectRerender);
        }
    }
}