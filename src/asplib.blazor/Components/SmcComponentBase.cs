using asplib.Model;
using statemap;

namespace asplib.Components
{
    public abstract class SmcComponentBase<T, F, S> : PersistentComponentBase<T>
        where T : class, IAppClass<F, S>, new()
        where F : statemap.FSMContext
        where S : statemap.State
    {
        /// <summary>
        /// The state of the FSM class
        /// </summary>
        public S State
        {
            get { return Main.GetState(); }
        }

        private List<statemap.StateChangeEventHandler> _stateChangedHandlers = new();
        private bool _isDisposed = false;

        // Blazor State Container / SMC event notification handler pattern
        public void StateChanged(object sender, StateChangeEventArgs args)
        {
            ReRender();
            StateHasChanged();
        }

        /// <summary>
        /// Dynamically set the pageType to display state-dependent parts here
        /// </summary>
        protected abstract void ReRender();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Main.Fsm.StateChange += StateChanged;
                _stateChangedHandlers.Add(StateChanged);
                Main.SetOwner();
                // React to the loaded state:
                ReRender();
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
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
            foreach (var handler in _stateChangedHandlers)
            {
                Main.Fsm.StateChange -= handler;
            }
            _stateChangedHandlers.Clear();
            TestFocus.RemoveFocus();    // finally removes the reference
            _isDisposed = true;
        }
    }
}