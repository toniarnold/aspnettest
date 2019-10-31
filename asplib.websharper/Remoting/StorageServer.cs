using asplib.Common;
using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;
using WebSharper;

namespace asplib.Remoting
{
    /// <summary>
    /// Implements the Load/Save methods to be used within concrete remote
    /// methods to instantiate the model object Main.
    /// </summary>
    public static class StorageServer
    {
        private static IConfigurationRoot Configuration
        {
            get { return RemotingContext.Configuration; }
        }

        private static HttpContext HttpContext
        {
            get { return RemotingContext.HttpContext; }
        }

        /// <summary>
        /// Remote method: Globally sets the storage, overriding SessionStorage
        /// from the configuration.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <returns></returns>
        [Remote]
        public static Task SetStorage(Storage storage)
        {
            StorageImplementation.SetStorage(storage);
            return Task.FromResult(true);
        }

        /// <summary>
        ///  Mirrors the MVC PersistentControllerActivator for the WebSharper
        ///  context.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="accessor">non-thread safe static reference</param>
        /// <returns></returns>
        // [Remote] -> Yields "Remote methods must not be generic" from WebSharper:
        // https://github.com/dotnet-websharper/core/issues/1048
        public static M Load<M, V>(string viewState, out M accessor, Storage? sessionStorage = null)
            where M : class, IStored<M>, new()
            where V : ViewModel<M>, new()
        {
            V viewModel;
            Guid sessionOverride;
            Guid session;
            byte[] bytes;
            Func<byte[], byte[]> filter;

            var storage = sessionStorage ?? StorageImplementation.GetStorage(Configuration, HttpContext);
            var storageID = StorageImplementation.GetStorageID(typeof(M).Name);
            StorageImplementation.ClearIfRequested(HttpContext, storage, storageID);

            if (HttpContext.Request.Method == WebRequestMethods.Http.Get &&
                Guid.TryParse(HttpContext.Request.Query["session"], out sessionOverride))
            {
                viewModel = new V();
                (bytes, filter) = StorageImplementation.DatabaseBytes(Configuration, HttpContext, storageID, sessionOverride);
                viewModel.Main = (M)StorageImplementation.LoadFromBytes(() => new M(), bytes, filter);
            }
            else
            {
                // ---------- Load ViewState ----------
                if (storage == Storage.ViewState)
                {
                    viewModel = new V();
                    filter = StorageImplementation.DecryptViewState(Configuration);
                    viewModel.ViewState = viewState;
                    viewModel.DeserializeMain(filter);   // creates a new Main if viewState is null
                }

                // ---------- Load Session ----------
                else if (storage == Storage.Session &&
                        HttpContext.Session.TryGetValue(storageID, out bytes))
                {
                    viewModel = new V();
                    viewModel.SetMain(StorageImplementation.LoadFromBytes(() => new M(), bytes));
                }

                // ---------- Load Database ----------
                else if (storage == Storage.Database &&
                        Guid.TryParse(HttpContext.Request.Cookies[storageID].FromCookieString()["session"], out session))
                {
                    (bytes, filter) = StorageImplementation.DatabaseBytes(Configuration, HttpContext, storageID, session);
                    viewModel = new V();
                    viewModel.SetMain(StorageImplementation.LoadFromBytes(() => new M(), bytes, filter));
                }
                else
                {
                    // No persisted object available yet -> return a new one
                    viewModel = new V();
                    viewModel.SetMain(new M());
                }
            }

            // Now that an instance is guaranteed remember the storage type for Save() and the client.
            viewModel.SessionStorage = storage;
            viewModel.VSessionStorage = storage.ToString();

            // An instantiated Main is now guaranteed -> make its members
            // visible to WebSharper:
            viewModel.LoadMembers();

            // Include the ViewModel instance as member of the returned Main
            viewModel.Main.ViewModel = viewModel;

            // Set a reference to the Model
            accessor = viewModel.Main;

            return viewModel.Main;
        }

        /// <summary>
        /// Saves the after having been loaded within the same remote method.
        /// Reloads potentially modified members in the M object to make
        /// changes visible in WebSharper.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="stored">The stored.</param>
        /// <returns>The VIewState string</returns>
        public static string Save<M>(ViewModel<M> stored)
            where M : class, IStored<M>, new()
        {
            stored.SaveMembers();   // Captures members directly mutable on the client side.
            stored.LoadMembers();   // Mirrors side effects of methods on M in the ViewModel.

            switch (stored.SessionStorage)  // guaranteed here, whether overridden or not
            {
                case Storage.ViewState:
                    var filter = StorageImplementation.EncryptViewState(Configuration);
                    stored.SerializeMain(filter); // save locally into the object
                    return stored.ViewState;

                case Storage.Session:
                    StorageImplementation.SaveSession(Configuration, HttpContext, stored.Main);
                    return null;

                case Storage.Database:
                    StorageImplementation.SaveDatabase(Configuration, HttpContext, stored.Main);
                    return null;

                default:
                    throw new NotImplementedException(String.Format(
                        "Storage {0}", stored.SessionStorage));
            }
        }
    }
}