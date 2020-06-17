using asplib.Model;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Base class for Browser tests of WebSharper SPA applications using SMC.
    /// Provides typed accessors to the SMC model, a [OneTimeSetUp] /
    /// [OneTimeTearDown] pair for starting/stopping the browser and a
    /// [OneTimeSetUp] / [OneTimeTearDown] pair for Storage.Database which
    /// cleans up newly added Main rows
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TViewModel">ViewModel for the SMC model classr</typeparam>
    /// <typeparam name="TModel">the SMC model class itself</typeparam>
    /// <typeparam name="TFSMContext">SMC context class</typeparam>
    /// <typeparam name="TState">SMC state</typeparam>
    public abstract class SpaSmcDbTest<TWebDriver, TViewModel, TModel, TFSMContext, TState> : SpaDbTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
        where TViewModel : SmcViewModel<TModel, TFSMContext, TState>, new()
        where TModel : class, ISmcTask<TModel, TFSMContext, TState>, new()
        where TFSMContext : statemap.FSMContext
        where TState : statemap.State
    {
        /// <summary>
        /// Should return the concrete ViewModel container for the SMC model class
        /// from the static reference in the static WebSharper class holding the [Remote]
        /// methods to load the SMC model class.
        /// </summary>
        /// <returns></returns>
        protected abstract TViewModel GetViewModel();

        protected TViewModel ViewModel
        {
            get { return this.GetViewModel(); }
        }

        protected TModel Main
        {
            get { return this.ViewModel.Main; }
        }

        protected TFSMContext Fsm
        {
            get { return this.Main.Fsm; }
        }

        protected TState State
        {
            get { return this.Main.State; }
        }
    }
}