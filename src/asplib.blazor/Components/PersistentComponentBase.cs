using asplib.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace asplib.Components
{
    public abstract class PersistentComponentBase<T> : StaticComponentBase<T> where T : class, new()
    {
        [Inject]
        protected IConfiguration Configuration { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private ProtectedLocalStorage ProtectedLocalStore { get; set; }

        [Inject]
        private ILogger<PersistentComponentBase<T>> Logger { get; set; }

        /// <summary>
        /// Local type of the session storage to override AppSettings["SessionStorage"]
        /// </summary>
        public Storage? SessionStorage { get; set; }

        /// <summary>
        /// Set the instance-local storage type
        /// </summary>
        /// <param name="storage"></param>
        protected void SetStorage(Storage storage)
        {
            this.SessionStorage = storage;
        }

        /// <summary>
        /// Set the instance-local storage type from a string parameter
        /// </summary>
        /// <param name="storage"></param>
        protected void SetStorage(string storage)
        {
            this.SessionStorage = (Storage)Enum.Parse(typeof(Storage), storage, true);
        }

        /// <summary>
        /// Instantiate the Main instance received from the browser,
        /// then always persist Main after it might have been changed
        /// on re-render.
        /// </summary>
        /// <param name="firstRender"></param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Logger.LogInformation("Load Storage {0}", this.GetStorage());

                switch (this.GetStorage())
                {
                    case Storage.ViewState:
                    case Storage.Database:
                        Trace.Assert(Main != null);  // Provided by Direct Injection from PersistentMainFactoryExtension
                        break;

                    case Storage.SessionStorage:
                        Main = await this.LoadFromBrowser(k => ProtectedSessionStore.GetAsync<string>(k));
                        this.StateHasChanged();
                        break;

                    case Storage.LocalStorage:
                        Main = await this.LoadFromBrowser(k => ProtectedLocalStore.GetAsync<string>(k));
                        this.StateHasChanged();
                        break;

                    default:
                        throw new NotImplementedException(String.Format(
                            "Storage {0}", this.GetStorage()));
                }
                this.StateHasChanged();
            }
            else
            {
                Logger.LogInformation("Save Storage {0}", this.GetStorage());

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
            await base.OnAfterRenderAsync(firstRender);
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
            this.Main = new T();
            this.StateHasChanged();
        }

        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Global config override in StorageImplementation.SessionStorage e.g. from unit tests
        /// 2. Configured storage in key="SessionStorage" value="Database"
        /// 3. Defaults to ViewState
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
                storage = String.IsNullOrWhiteSpace(configStorage) ? Storage.ViewState : (Storage)Enum.Parse(typeof(Storage), configStorage);
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
                var main = StorageImplementation.LoadFromViewstate(() => new T(), viewState, filter);
                return main;
            }
            else
            {
                return new T();
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