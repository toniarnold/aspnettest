using asplib.Model;
using asplib.Model.Db;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace asplib.Components
{
    public class PersistentComponentBase<T> : OwningComponentBase<T> where T : notnull
    {
        [Inject]
        public T Main { get; private set; }

        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        private HttpContext HttpContext => HttpContextAccessor.HttpContext;

        /// <summary>
        /// Always persist Main after it might have been changed
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            switch (this.GetStorage())
            {
                case Storage.ViewState:
                    break;  // implied in Blazor Server

                case Storage.Session:
                    this.SaveSession();
                    break;

                case Storage.Database:
                    this.SaveDatabase();
                    break;

                default:
                    throw new NotImplementedException(String.Format(
                        "Storage {0}", this.GetStorage()));
            }
        }

        /// <summary>
        /// Get the actual storage type to use in this precedence:
        /// 1. Global config override in StorageImplementation.SessionStorage e.g. from unit tests
        /// 2. Configured storage in key="SessionStorage" value="Database"
        /// 3. Defaults to ViewState
        /// </summary>
        /// <returns></returns>
        private Storage GetStorage()
        {
            var storage = StorageImplementation.SessionStorage;       // static and global override
            if (storage == null)                // configuration or default
            {
                var configStorage = this.Configuration.GetValue<string>("SessionStorage");
                storage = String.IsNullOrWhiteSpace(configStorage) ? Storage.ViewState : (Storage)Enum.Parse(typeof(Storage), configStorage);
            }
            return (Storage)storage;
        }

        /// <summary>
        /// Add the serialized Main to the session
        /// </summary>
        private void SaveSession()
        {
            var mainType = typeof(T);
            var storageID = StorageImplementation.GetStorageID(mainType.Name);
            if (HttpContext.Session.IsAvailable)
            {
                HttpContext.Session.Set(storageID, StorageImplementation.Bytes(Main));
            }
        }

        /// <summary>
        /// Store the serialized Main to the database. Reference to it is
        /// kept in a persistent cookie.
        /// </summary>
        private void SaveDatabase()
        {
            StorageImplementation.SaveDatabase(Configuration, HttpContext, Main);
        }
    }
}