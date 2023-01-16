using asplib.Common;
using asplib.Model;
using asplib.Model.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;
using System.Net;

namespace asplib.Services
{
    public static class PersistentMainFactoryExtension
    {
        private static MemoryCache s_initialRequestCache = new(new MemoryCacheOptions());

        /// <summary>
        /// Defaults to 2 minutes to be on the safe side according to the Blazor
        /// Server defaults: "When creating a hub connection in a component, set
        /// the ServerTimeout (default: 30 seconds), HandshakeTimeout (default:
        /// 15 seconds), and KeepAliveInterval (default: 15 seconds) on the
        /// built HubConnection."
        /// </summary>
        public static TimeSpan InitialRequestCacheExpiration { get; set; } = new TimeSpan(0, 2, 0);

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
            services.TryAddScoped<ScopeCorrelation>();
            services.TryAddScoped<ScopeCorrelationProvider>();

            // Factory lambda to retrieve a persistent T instance.
            // A scoped service behaves as Singleton in Blazor Server, multiple
            // component instance would share the same state object - therefore
            // use a transient one. But then the factory gets called twice, once
            // for the initial request, once for the request with the id query
            // string to set up the SignalR WebSocket for Blazor. In that case,
            // return the cached instance from the first request.
            // The Guid correlates concurrent requests from distinct clients
            // which otherwise would share the cached instance.
            services.AddTransient<T>(provider =>
            {
                var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext; // null in bUnit
                var scopeCorrelation = provider.GetRequiredService<ScopeCorrelation>();

                if (httpContext != null &&
                    s_initialRequestCache.TryGetValue<T>((scopeCorrelation.Guid, typeof(T)), out var instance))
                {
                    // If found in the cache it is the second and final request setting up SignalR,
                    // thus immediately free the reference in the cache.
                    s_initialRequestCache.Remove((scopeCorrelation.Guid, typeof(T)));
                    return instance;
                }

                // Implemented after PersistentControllerActivator for ASP.NET Core MVC
                // In ASP.NET Core Blazor, the HttpContext is only available on page initialization
                // and cookies can only be set before the response has been started -  which is
                // the case for DI instantiation on the first request.

                IConfiguration configuration = provider.GetService<IConfiguration>() ??
                    throw new NullReferenceException("IConfiguration not registered");
                var mainType = typeof(T);
                var storageID = StorageImplementation.GetStorageID(mainType.Name);
                var sessionStorageID = StorageImplementation.GetSessionStorageID(mainType.Name);

                ASP_DBEntities.ConnectionString = configuration["ASP_DBEntities"];  // globally
                var storage = StorageImplementation.GetStorage(configuration, httpContext, sessionStorageID);

                // For Database handle the ?clear=true GET argument upfront, the
                // others are handled in the PersistentComponentBase
                if (storage == Storage.Database)
                {
                    StorageImplementation.ClearIfRequested(httpContext, storage, storageID);
                }

                Guid sessionOverride;
                Guid session;
                byte[] bytes;
                Func<byte[], byte[]>? filter = null;
                T? main = null;

                // ---------- Direct GET request ?session= from the Database ----------
                if (httpContext?.Request.Method == WebRequestMethods.Http.Get &&
                    Guid.TryParse(httpContext.Request.Query["session"], out sessionOverride))
                {
                    if (typeof(T).IsSerializable) // exclude e.g. the TestRunnerFsm
                    {
                        using (var db = new ASP_DBEntities())
                        {
                            (bytes, filter) = StorageImplementation.DatabaseBytes(
                                configuration, httpContext, storageID, sessionOverride);
                        }
                        main = DeserializeMain<T>(bytes, filter);
                        TypeDescriptor.AddAttributes(main, new IsRequestedInstanceAttribute()); // for PersistentComponentBase
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
                                var cookie = httpContext?.Request.Cookies[storageID].FromCookieString();
                                var key = (cookie?["key"] != null) ? Convert.FromBase64String(cookie!["key"]!) :
                                    throw new Exception("No cookie[\"key\"] ");
                                TypeDescriptor.AddAttributes(main, new DatabaseKeyAttribute(key));
                            }
                        }
                        else
                        {
                            main = PersistentMainFactory<T>.Instantiate(provider);  // new instance
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
                else  // Hydrate the deserialized instance from the database
                {
                    PersistentMainFactory<T>.PerformPropertyInjection(provider, main);
                }

                if (httpContext != null)  // not bUnit
                {
                    s_initialRequestCache.Set(  // cache the T instance from the initial HTTP request
                        (scopeCorrelation.Guid, typeof(T)),
                        main,
                        InitialRequestCacheExpiration); // absoluteExpirationRelativeToNow
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