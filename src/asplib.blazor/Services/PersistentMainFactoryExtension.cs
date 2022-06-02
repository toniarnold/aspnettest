using asplib.Common;
using asplib.Model;
using asplib.Model.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;
using System.Net;

namespace asplib.Services
{
    public static class PersistentMainFactoryExtension
    {
        private static Guid? sessionOverrideCached;

        /// <summary>
        /// Add a persistent Main object as a service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        public static void AddPersistent<T>(this IServiceCollection services)
            where T : class, new()  // for serialization
        {
            // Add implicit dependencies for Persistent
            services.TryAddSingleton<IServiceProvider>(sp => sp);
            services.AddHttpContextAccessor();

            // A scoped service would behave as Singleton in Blazor Server -
            // thus use a transient one. But then the factory gets called twice,
            // once for the initial request, once for the request with the id
            // query string to set up the SignalR WebSocket for Blazor.
            services.AddTransient<T>(provider =>
            {
                // Implemented after PersistentControllerActivator for ASP.NET Core MVC
                // In ASP.NET Core Blazor, HttpContext is only available on page initialization
                // and cookies can only be set before the response has been started -  which is
                // the case for DI instantiation.
                var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                var configuration = provider.GetService<IConfiguration>();
                var mainType = typeof(T);
                var storageID = StorageImplementation.GetStorageID(mainType.Name);
                var sessionStorageID = StorageImplementation.GetSessionStorageID(mainType.Name);

                ASP_DBEntities.ConnectionString = configuration?["ASP_DBEntities"];  // globally
                var storage = StorageImplementation.GetStorage(configuration, httpContext, sessionStorageID);
                StorageImplementation.ClearIfRequested(httpContext, storage, storageID);

                Guid sessionOverride;
                Guid session;
                byte[] bytes;
                Func<byte[], byte[]>? filter = null;
                T? main = null;

                // Distinct 1st/2nd request, both instantiate the service, but
                // the 2nd clears the original GET string in favor of the id:
                bool isWebSocketRequest = (httpContext != null) ? httpContext.Request.Query.ContainsKey("id") : false;

                // ---------- Direct GET request ?session= from the Database ----------
                if (httpContext?.Request.Method == WebRequestMethods.Http.Get &&
                    Guid.TryParse(httpContext.Request.Query["session"], out sessionOverride))
                {
                    sessionOverrideCached = sessionOverride;
                    // isWebSocketRequest is still false during the 1st request
                }
                if (isWebSocketRequest && sessionOverrideCached != null)    // 2nd request
                {
                    if (typeof(T).IsSerializable) // exclude e.g. the TestRunnerFsm
                    {
                        using (var db = new ASP_DBEntities())
                        {
                            (bytes, filter) = StorageImplementation.DatabaseBytes(
                                configuration, httpContext, storageID, (Guid)sessionOverrideCached);
                        }
                        sessionOverrideCached = null;   // immediately neutralize the static global
                        main = DeserializeMain<T>(bytes, filter);
                        TypeDescriptor.AddAttributes(main, new IsRequestedInstanceAttribute());
                    }
                }
                else
                {
                    // ---------- Load from Database ----------
                    if (storage == Storage.Database)
                    {
                        if (Guid.TryParse(httpContext?.Request.Cookies[storageID].FromCookieString()["session"], out session))   // existing session
                        {
                            (bytes, filter) = StorageImplementation.DatabaseBytes(configuration, httpContext, storageID, session);
                            main = DeserializeMain<T>(bytes, filter);
                            TypeDescriptor.AddAttributes(main, new DatabaseSessionAttribute(session));  // remember the session Guid
                            if (StorageImplementation.GetEncryptDatabaseStorage(configuration))
                            {
                                var cookie = httpContext.Request.Cookies[storageID].FromCookieString();
                                var key = (cookie?["key"] != null) ? Convert.FromBase64String(cookie?["key"] ?? "") : null;
                                TypeDescriptor.AddAttributes(main, new DatabaseKeyAttribute(key));
                            }
                        }
                        else
                        {
                            main = PersistentMainFactory<T>.Instantiate(provider);
                            if (typeof(T).IsSerializable) // too early to override global storage settings
                            {
                                // Immediately save the new instance to obtain a cookie and instance attributes before the initial request is disposed
                                StorageImplementation.SaveDatabase(configuration, httpContext, main);
                            }
                        }
                    }
                }

                if (main == null)   // New instance required by PersistentComponentBase
                {
                    main = PersistentMainFactory<T>.Instantiate(provider);
                }
                else  // Hydrate the instance from the database
                {
                    PersistentMainFactory<T>.PerformPropertyInjection(provider, main);
                }

                return main;
            });
        }

        private static T DeserializeMain<T>(byte[] bytes, Func<byte[], byte[]>? filter = null)
        {
            return (T)Serialization.Deserialize(bytes, filter);
        }
    }
}