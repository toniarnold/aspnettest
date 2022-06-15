using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace asplib.Components
{
    /// <summary>
    /// Testable Blazor OwningComponent which sets a static reference to its
    /// instance on TestFocus.Component and injects a public Main instance.
    /// (the owned service).
    /// </summary>
    public abstract class StaticOwningComponentBase<T> : OwningComponentBase<T>, ITestFocus
        where T : class, new()
    {
        private EditContext? _editContext;
        private readonly List<EventHandler<ValidationStateChangedEventArgs>> _validationStateChangedHandlers = new();
        private bool _isDisposed = false;

        [Inject]
        public T Main { get; protected set; } = default!;

        /// <summary>
        /// Set the static reference to the component in focus on each Render,
        /// as the ElementReference.Id can change.
        /// Signals the TestFocus.Event if the component has focus.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.EndRenderAsync(firstRender);
        }

        /// <summary>
        /// Threat ValidationStateChanged as a full StateHasChanged()
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void HandleValidationStateChanged(object? o, ValidationStateChangedEventArgs args)
        {
            StateHasChanged();  // InvokeAsync(() => this.EndRenderAsync(false)); releases the waiting test thread too early
        }

        /// <summary>
        /// ITestFocus: Click() with validation errors doesn't trigger any
        /// OnAfterRender events on the server (not even in
        /// DataAnnotationsValidator), but we need to synchronize. Therefore
        /// enforce a (redundant) re-render when currently having focus in a
        /// running test by registering the EditContext.
        protected void AddEditContextTestFocus(EditContext editContext)
        {
            if (this.HasFocus())
            {
                _editContext = editContext;
                editContext.OnValidationStateChanged += HandleValidationStateChanged;
                _validationStateChangedHandlers.Add(HandleValidationStateChanged);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (_editContext != null)
            {
                foreach (var handler in _validationStateChangedHandlers)
                {
                    _editContext.OnValidationStateChanged -= handler;
                }
                _validationStateChangedHandlers.Clear();
            }
            _isDisposed = true;
        }
    }
}