using asplib.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace asplib.Components
{
    /// <summary>
    /// Testable Blazor OwningComponent which sets a static reference to its
    /// instance on TestFocus.Component, injects a public Main instance.
    /// (the owned service) end sets up the configured persistence for
    /// that service.
    /// </summary>
    public abstract class PersistentComponentBase<T> : StaticOwningComponentBase<T>
        where T : class, new()
    {
        [Inject]
        protected IConfiguration Configuration { get; set; } = default!;

        [Inject]
        public IServiceProvider ServiceProvider { get; set; } = default!;

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;

        [Inject]
        private ProtectedLocalStorage ProtectedLocalStore { get; set; } = default!;

        [Inject]
        private ILogger<PersistentComponentBase<T>> Logger { get; set; } = default!;

        /// <summary>
        /// Local type of the session storage to override AppSettings["SessionStorage"]
        /// </summary>
        public Storage? SessionStorage { get; set; }

        /// <summary>
        /// Set to true to avoid storing the old state in the new storage after
        /// changing the storage. Will be reinitialized to false after
        /// reinstantiating the component on reload after a storage change,
        /// </summary>
        public bool StorageHasChanged { get; set; } = false;

        // OnParametersSetAsync double rendering prevention:

        private bool _firstRender = true;

        private bool _shouldRender = true;

        /// <summary>
        /// Set the instance-local storage type
        /// </summary>
        /// <param name="storage"></param>
        protected void SetStorage(Storage storage)
        {
            this.SessionStorage = storage;
            StorageHasChanged = true;
        }

        /// <summary>
        /// Set the instance-local storage type from a string parameter
        /// </summary>
        /// <param name="storage"></param>
        protected void SetStorage(string storage)
        {
            this.SessionStorage = (Storage)Enum.Parse(typeof(Storage), storage, true);
            StorageHasChanged = true;
        }

        /// <summary>
        /// In case of browser persistence, postpone full rendering as the
        /// object can only be loaded after initially rendering (without the
        /// persistent object). _shouldRender is reset to true in
        /// OnAfterRenderAsync after the loading of Main.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (_firstRender)
            {
                switch (this.GetStorage())
                {
                    case Storage.SessionStorage:
                    case Storage.LocalStorage:
                        _shouldRender = false;
                        break;
                }
                _firstRender = false;
            }
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        /// <summary>
        /// Instantiate the Main instance and signal TestFocus.Event
        /// on re-render.
        /// </summary>
        /// <param name="firstRender"></param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await CreateMain(firstRender);
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Instantiate the Main instance received from the browser and
        /// re-render, then always persist Main after it might have been changed
        /// on re-render.
        /// </summary>
        /// <param name="firstRender"></param>
        protected async Task CreateMain(bool firstRender)
        {
            if (firstRender)
            {
                Logger.LogInformation(0, "Load Storage {storage}", this.GetStorage());

                switch (this.GetStorage())
                {
                    case Storage.ViewState:
                    case Storage.Database:
                        Trace.Assert(Main != null);  // Provided by Direct Injection from PersistentMainFactoryExtension
                        break;

                    case Storage.SessionStorage:
                        Main = await this.LoadFromBrowser(k => ProtectedSessionStore.GetAsync<string>(k));
                        _shouldRender = true;
                        this.StateHasChanged();
                        break;

                    case Storage.LocalStorage:
                        Main = await this.LoadFromBrowser(k => ProtectedLocalStore.GetAsync<string>(k));
                        _shouldRender = true;
                        this.StateHasChanged();
                        break;

                    default:
                        throw new NotImplementedException(String.Format(
                            "Storage {0}", this.GetStorage()));
                }
            }
            else if (!StorageHasChanged)
            {
                Logger.LogInformation(0, "Save Storage {storage}", this.GetStorage());

                switch (this.GetStorage())
                {
                    case Storage.ViewState:
                        break;  // Default instance lifetime in Blazor Server

                    case Storage.Database:
                        this.SaveDatabase();
                        break;

                    case Storage.SessionStorage:
                        await this.SaveToBrowser((k, v) => ProtectedSessionStore.SetAsync(k, v));
                        break;

                    case Storage.LocalStorage:
                        await this.SaveToBrowser((k, v) => ProtectedLocalStore.SetAsync(k, v));
                        break;

                    default:
                        throw new NotImplementedException(String.Format(
                            "Storage {0}", this.GetStorage()));
                }
            }
        }

        /// <summary>
        /// Delete the Main instance from the current browser stores,
        /// regardless of the current Storage mechanism active.
        /// </summary>
        /// <returns></returns>
        protected async Task DeleteBrowserStore()
        {
            var storageId = StorageImplementation.GetStorageID(Main.GetType().Name);
            await ProtectedLocalStore.DeleteAsync(storageId);
            await ProtectedSessionStore.DeleteAsync(storageId);
            this.Main = (T)ActivatorUtilities.CreateInstance(ServiceProvider, typeof(T));
            this.StateHasChanged();
        }

        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Global config override in StorageImplementation.SessionStorage e.g. from unit tests
        /// 2. Configured storage in key="SessionStorage" value="Database"
        /// 3. Defaults to SessionStorage (persistence over reload)
        /// </summary>
        /// <returns></returns>
        protected Storage GetStorage()
        {
            var storage = this.SessionStorage;    // Instance property
            if (storage == null)
            {
                storage = SessionStorage;   // overrides all
            }
            if (storage == null)
            {
                storage = StorageImplementation.SessionStorage;       // static and global override
            }
            if (storage == null)                // configuration or default
            {
                var configStorage = this.Configuration.GetValue<string>("SessionStorage");
                storage = String.IsNullOrWhiteSpace(configStorage) ? Storage.SessionStorage : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
        }

        private async Task<T> LoadFromBrowser(Func<string, ValueTask<ProtectedBrowserStorageResult<string>>> storeGetAsync)
        {
            var storageId = StorageImplementation.GetStorageID(Main.GetType().Name);
            var result = await storeGetAsync(storageId);
            if (result.Success)
            {
                var viewState = result.Value;
                var filter = StorageImplementation.DecryptViewState(Configuration);
                var main = StorageImplementation.LoadFromViewstate(
                    () => (T)ActivatorUtilities.CreateInstance(ServiceProvider, typeof(T)), viewState, filter);
                return main;
            }
            else
            {
                return (T)ActivatorUtilities.CreateInstance(ServiceProvider, typeof(T));
            }
        }

        /// <summary>
        /// Store the serialized Main to the database. Reference to it is
        /// kept in a persistent cookie set in PersistentMainFactoryExtension.
        /// </summary>
        private void SaveDatabase()
        {
            StorageImplementation.SaveDatabase(Configuration, Main);
        }

        private async Task SaveToBrowser(Func<string, object, ValueTask> storeSetAsync)
        {
            var storageId = StorageImplementation.GetStorageID(Main.GetType().Name);
            var filter = StorageImplementation.EncryptViewState(Configuration);
            var viewState = StorageImplementation.ViewState(Main, filter);
            await storeSetAsync(storageId, viewState);
        }
    }
}