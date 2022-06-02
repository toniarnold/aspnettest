using asplib.Model;
using statemap;

namespace asplib.Components
{
    /// Testable Blazor OwningComponent which sets a static reference to its
    /// instance on TestFocus.Component, injects a public Main instance of an
    /// SMC AppClass with accessors (the owned service) and sets up the
    /// configured persistence for that service.
    public abstract class SmcComponentBase<T, F, S> : PersistentComponentBase<T>
        where T : class, IAppClass<F, S>, new()
        where F : statemap.FSMContext
        where S : statemap.State
    {
        private readonly List<statemap.StateChangeEventHandler> _stateChangedHandlers = new();
        private bool _isDisposed = false;

        /// <summary>
        /// The state of the FSMContext class
        /// </summary>
        public S State
        {
            get { return Main.GetState(); }
        }

        /// <summary>
        /// The FSMContext class itself
        /// </summary>
        public F Fsm
        {
            get { return Main.Fsm; }
        }

        /// <summary>
        /// Blazor State Container / SMC event notification handler pattern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void StateChanged(object sender, StateChangeEventArgs args)
        {
            RenderMain();
            TestFocus.AwaitingRerender = false; // re-rendering starts now
            InvokeAsync(StateHasChanged); // Switch context in case we're not being called from the UI thread
        }

        /// <summary>
        /// Hook for changing the UI Component state according to FSM state,
        /// e.g. setting the pageType in a DynamicComponent to display
        /// state-dependent parts.
        /// </summary>
        protected virtual void RenderMain()
        { }

        /// <summary>
        /// Wire up the FSM with StateChange event and owner instance.
        /// </summary>
        protected override void HydrateMain()
        {
            Main.Fsm.StateChange += StateChanged;
            _stateChangedHandlers.Add(StateChanged);
            Main.SetOwner();
            this.RenderMain();
        }

        /// <summary>
        /// Remove the handlers from the Main object in case there is some
        /// static reference to it left from the test such that the component
        /// can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            DisposeFsm();
            // Must not remove an eventual stalled reference with
            // TestFocus.RemoveFocus();
            // The component in focus must remain there for test assertions. In
            // production, there will never be a component in focus.
            _isDisposed = true;
        }

        /// <summary>
        /// Remove the StateChanged handlers
        /// </summary>
        private void DisposeFsm()
        {
            foreach (var handler in _stateChangedHandlers)
            {
                Main.Fsm.StateChange -= handler;
            }
            _stateChangedHandlers.Clear();
        }
    }
}