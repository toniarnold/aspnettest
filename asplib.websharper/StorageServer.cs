﻿using asplib.Common;
using asplib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using WebSharper;

namespace asplib
{
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
        /// Globally sets the storage, overriding SessionStorage from the
        /// configuration.
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
        ///  Mirrors the MVC StorageControllerActivator for the WebSharper
        ///  context.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="accessor">non-thread safe static reference</param>
        /// <returns></returns>
        // [Remote] -> Yields "Remote methods must not be generic" from WebSharper:
        // https://github.com/dotnet-websharper/core/issues/1048
        public static S Load<S, M>(string viewState, ref M accessor)
            where S : Stored<M>, new()
            where M : class, new()
        {
            S retval;
            Guid sessionOverride;
            Guid session;
            byte[] bytes;
            Func<byte[], byte[]> filter;

            var storage = StorageImplementation.GetStorage(Configuration, HttpContext);
            var storageID = StorageImplementation.GetStorageID(typeof(M).Name);
            StorageImplementation.ClearIfRequested(HttpContext, storage, storageID);

            if (HttpContext.Request.Method == WebRequestMethods.Http.Get &&
                Guid.TryParse(HttpContext.Request.Query["session"], out sessionOverride))
            {
                (bytes, filter) = StorageImplementation.DatabaseBytes(Configuration, HttpContext, storageID, sessionOverride);
                retval = new S();
                retval.Main = (M)StorageImplementation.LoadFromBytes(() => new M(), bytes, filter);
            }
            else
            {
                // ---------- Load ViewState ----------
                if (storage == Storage.ViewState)
                {
                    retval = new S();
                    filter = StorageImplementation.DecryptViewState(Configuration);
                    retval.ViewState = viewState;
                    retval.DeserializeMain(filter);   // creates a new Main if viewState is null
                }

                // ---------- Load Session ----------
                else if (storage == Storage.Session &&
                        HttpContext.Session.TryGetValue(storageID, out bytes))
                {
                    retval = new S();
                    retval.Main = StorageImplementation.LoadFromBytes(() => new M(), bytes);
                }

                // ---------- Load Database ----------
                else if (storage == Storage.Database &&
                        Guid.TryParse(HttpContext.Request.Cookies[storageID].FromCookieString()["session"], out session))
                {
                    (bytes, filter) = StorageImplementation.DatabaseBytes(Configuration, HttpContext, storageID, session);
                    retval = new S();
                    retval.Main = StorageImplementation.LoadFromBytes(() => new M(), bytes, filter);
                }
                else
                {
                    // No persisted object available yet -> return a new one
                    retval = new S();
                    retval.Main = new M();
                }
            }
            // An instantiated Main is now guaranteed -> make its members
            // visible to WebSharper:
            retval.LoadMembers();

            return retval;
        }

        /// <summary>
        /// Saves the after having been loaded within the same remote method.
        /// Reloads potentially modified members in the M object to make
        /// changes visible in WebSharper.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="stored">The stored.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string Save<M>(Stored<M> stored)
            where M : class, new()
        {
            stored.LoadMembers();

            var storage = StorageImplementation.GetStorage(Configuration, HttpContext);
            switch (storage)
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
                    throw new NotImplementedException(String.Format("Storage {0} not implemented", storage));
            }
        }

        /// <summary>
        /// Saves the stored object discretely a second time to store
        /// potentially client-side modified members after the initial
        /// transition.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="stored">The stored object.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string SaveDiscretely<M>(Stored<M> stored)
            where M : class, new()
        {
            Trace.Assert(stored != null, "Object main to save must not be null");

            // M must explicitly be recreated without LoadMembers() to capture
            // potentially modified WebSharper members:
            stored.DeserializeMain();
            stored.SaveMembers();

            return Save(stored);
        }
    }
}