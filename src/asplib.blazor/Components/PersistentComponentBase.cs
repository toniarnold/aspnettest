using asplib.Model;
using asplib.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;

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
        // OnParametersSetAsync double rendering mitigation:

        private bool _firstRender = true;
        private bool _shouldRender = true;
        private bool _doSaveToBrowserUrl = true;

        /// <summary>
        /// ?clear=true requests prior deletion of the stored object
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery]
        public bool? Clear { get; set; }

        /// <summary>
        /// Local type of the session storage to override the storage configured in the appsettings
        /// </summary>
        public Storage? SessionStorage { get; set; }

        [Inject]
        protected IConfiguration Configuration { get; set; } = default!;

        [Inject]
        public IServiceProvider ServiceProvider { get; set; } = default!;

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; } = default!;

        [Inject]
        private ProtectedLocalStorage ProtectedLocalStore { get; set; } = default!;

        [Inject]
        private NavigationManager uriHelper { get; set; } = default!;

        [Inject]
        private ILogger<PersistentComponentBase<T>> Logger { get; set; } = default!;

        /// <summary>
        /// Set to true to avoid storing the old state in the new storage after
        /// changing the storage. Will be reinitialized to false after
        /// reinstantiating the component on reload after a storage change,
        /// </summary>
        public bool StorageHasChanged { get; set; } = false;

        /// <summary>
        /// True when Main has the IsRequestedInstanceAttribute indicating that
        /// it has been injected by the ?session= GET parameter
        /// </summary>
        private bool IsMainRequestedInstance =>
            (from Attribute a in TypeDescriptor.GetAttributes(this.Main)
             where a is IsRequestedInstanceAttribute
             select a).Any();

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
                    case Storage.ViewState:
                    case Storage.Database:
                        this.HydrateMain();
                        break;

                    case Storage.SessionStorage:
                    case Storage.LocalStorage:
                    case Storage.UrlQuery:
                        if (IsMainRequestedInstance)
                        {
                            this.HydrateMain(); // the instance we got from the database
                        }
                        else
                        {
                            _shouldRender = false; // overwrite Main in OnAfterRenderAsync
                        }
                        break;

                    default:
                        throw new NotImplementedException(String.Format(
                            "Storage {0}", this.GetStorage()));
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
        /// Handle the "clear=true" GET argument
        /// </summary>
        /// <param name="firstRender"></param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Clear != null && (bool)Clear)
            {
                await this.DeleteBrowserStore();
                this.Clear = false;  // only clear once
            }
            await PersistMain(firstRender);
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Instantiate the Main instance received from the browser and
        /// re-render, then always persist Main after it might have been changed
        /// on re-render.
        /// </summary>
        /// <param name="firstRender"></param>
        protected async Task PersistMain(bool firstRender)
        {
            Trace.Assert(Main != null);  // Guaranteed by Direct Injection from PersistentMainFactoryExtension.AddPersistent<T>

            if (firstRender)
            {
                if (!IsMainRequestedInstance)
                {
                    switch (this.GetStorage())
                    {
                        case Storage.SessionStorage:
                            Logger.LogInformation(0, "Load SessionStorage");
                            Main = await this.LoadFromBrowser(k => ProtectedSessionStore.GetAsync<string>(k));
                            this.HydrateMain();
                            _shouldRender = true;
                            this.StateHasChanged();
                            break;

                        case Storage.LocalStorage:
                            Logger.LogInformation(0, "Load LocalStorage");
                            Main = await this.LoadFromBrowser(k => ProtectedLocalStore.GetAsync<string>(k));
                            this.HydrateMain();
                            _shouldRender = true;
                            this.StateHasChanged();
                            break;

                        case Storage.UrlQuery:
                            Logger.LogInformation(0, "Load UrlQuery");
                            Main = this.LoadFromBrowserUrl();
                            this.HydrateMain();
                            _doSaveToBrowserUrl = false;    // the same state that was just loaded
                            _shouldRender = true;
                            this.StateHasChanged();
                            break;
                    }
                }
            }
            else if (!StorageHasChanged)
            {
                // The IsRequestedInstanceAttribute would be serialized, too, thus remove it now
                var provider = TypeDescriptor.GetProvider(Main!);
                TypeDescriptor.RemoveProvider(provider, Main!);

                switch (this.GetStorage())
                {
                    case Storage.ViewState:
                        break;  // Default instance lifetime in Blazor Server

                    case Storage.Database:
                        Logger.LogInformation(0, "Save Database");
                        this.SaveToDatabase();
                        break;

                    case Storage.SessionStorage:
                        Logger.LogInformation(0, "Save SessionStorage");
                        await this.SaveToBrowser((k, v) => ProtectedSessionStore.SetAsync(k, v));
                        break;

                    case Storage.LocalStorage:
                        Logger.LogInformation(0, "Save LocalStorage");
                        await this.SaveToBrowser((k, v) => ProtectedLocalStore.SetAsync(k, v));
                        break;

                    case Storage.UrlQuery:
                        if (_doSaveToBrowserUrl)
                        {
                            Logger.LogInformation(0, "Save UrlQuery");
                            this.SaveToBrowserUrl();
                        }
                        else
                        {
                            _doSaveToBrowserUrl = true; // next time
                        }
                        break;

                    default:
                        throw new NotImplementedException(String.Format(
                            "Storage {0}", this.GetStorage()));
                }
            }
        }

        /// <summary>
        /// Hook to perform scaffolding with the deserialized new Main instance,
        /// e.g. by setting transient object references or reacting to state.
        /// </summary>
        protected virtual void HydrateMain()
        { }

        /// <summary>
        /// Delete the Main instance from the current browser stores,
        /// regardless of the current Storage mechanism active.
        /// Only callable OnAfterRender.
        /// </summary>
        /// <returns></returns>
        protected async Task DeleteBrowserStore()
        {
            var storageId = StorageImplementation.GetStorageID(typeof(T).Name);
            await ProtectedLocalStore.DeleteAsync(storageId);
            await ProtectedSessionStore.DeleteAsync(storageId);
            this.Main = PersistentMainFactory<T>.Instantiate(ServiceProvider);
            this.HydrateMain();
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
                storage = StorageImplementation.SessionStorage;       // static and global override
            }
            if (storage == null)                // configuration or default
            {
                var configStorage = this.Configuration.GetValue<string>("SessionStorage");
                storage = String.IsNullOrWhiteSpace(configStorage) ?
                            Storage.SessionStorage :    // default value
                            (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
        }

        /// <summary>
        /// Returns a deserialized Main instance from the browser if available,
        /// else just returns the one already there.
        /// </summary>
        /// <param name="storeGetAsync"></param>
        /// <returns></returns>
        private async Task<T> LoadFromBrowser(Func<string, ValueTask<ProtectedBrowserStorageResult<string>>> storeGetAsync)
        {
            var storageId = StorageImplementation.GetStorageID(typeof(T).Name);
            var result = await storeGetAsync(storageId);
            if (result.Success && result.Value != null)
            {
                var viewState = result.Value;
                var filter = StorageImplementation.DecryptDecompressFilter(Configuration);
                var main = StorageImplementation.LoadFromViewstate(() => this.Main, viewState, filter);
                PersistentMainFactory<T>.PerformPropertyInjection(ServiceProvider, main);   // always
                return main;
            }
            else
            {
                return this.Main;   // Got no instance from the bowser -> return the one from PersistentMainFactoryExtension
            }
        }

        /// <summary>
        /// Store the serialized Main with the ProtectedSessionStore method given
        /// by storeSetAsync.
        /// </summary>
        /// <param name="storeSetAsync"></param>
        /// <returns></returns>
        private async Task SaveToBrowser(Func<string, object, ValueTask> storeSetAsync)
        {
            var storageId = StorageImplementation.GetStorageID(typeof(T).Name);
            var filter = StorageImplementation.CompressEncryptFilter(Configuration);
            var viewState = StorageImplementation.ViewState(Main, filter);
            await storeSetAsync(storageId, viewState);
        }

        /// <summary>
        /// Store the serialized Main to the database. Reference to it is
        /// kept in a persistent cookie set in PersistentMainFactoryExtension.
        /// </summary>
        private void SaveToDatabase()
        {
            StorageImplementation.SaveDatabase(Configuration, Main);
        }

        // Url storage is quite common according to
        // https://news.ycombinator.com/item?id=34312546:
        // "How to store your app's entire state in the url" - but they're using
        // client side JSON serialization, not the server side binary
        // serialization used here.
        // Size limit: "seems Chrome/Fx/Safari all support at least 32k"

        /// <summary>
        /// Returns a deserialized Main instance from the query parameter
        /// storageId if available, else just returns the one already there.
        /// </summary>
        /// <returns></returns>
        private T LoadFromBrowserUrl()
        {
            var query = QueryHelpers.ParseQuery(uriHelper.ToAbsoluteUri(uriHelper.Uri).Query);
            var storageId = StorageImplementation.GetStorageID(typeof(T).Name);
            if (query.TryGetValue(storageId, out var viewState))
            {
                var filter = StorageImplementation.DecryptDecompressFilter(Configuration);
                var main = StorageImplementation.LoadFromViewstate(
                            () => this.Main, HttpUtility.UrlDecode(viewState[0]), filter);
                PersistentMainFactory<T>.PerformPropertyInjection(ServiceProvider, main);   // always
                return main;
            }
            else
            {
                return this.Main;   // Got no instance from the bowser -> return the one from PersistentMainFactoryExtension
            }
        }

        /// <summary>
        /// Store the serialized Main in the query parameter storageId.
        /// </summary>
        private void SaveToBrowserUrl()
        {
            var filter = StorageImplementation.CompressEncryptFilter(Configuration);
            var viewState = StorageImplementation.ViewState(Main, filter);
            var query = QueryHelpers.ParseQuery(uriHelper.ToAbsoluteUri(uriHelper.Uri).Query);
            var storageId = StorageImplementation.GetStorageID(typeof(T).Name);
            query[storageId] = new StringValues(HttpUtility.UrlEncode(viewState));
            var uri = uriHelper.ToAbsoluteUri(uriHelper.Uri).GetLeftPart(UriPartial.Path);
            var newUri = QueryHelpers.AddQueryString(uri, query);
            _doSaveToBrowserUrl = false;    // avoid setting the query twice due to the navigation
            uriHelper.NavigateTo(newUri);
        }
    }
}