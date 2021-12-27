using asplib.Common;
using asplib.Model;
using asplib.Model.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace asplib.Services
{
    public static class PersistentMainActivatorExtension
    {
        public static void AddPersistent<T>(this IServiceCollection services) where T : class, new()
        {
            services.AddScoped<T>(provider =>
            {
                // Implemented after PersistentControllerActivator
                var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;
                var configuration = provider.GetService<IConfiguration>();
                var mainType = typeof(T);
                var storageID = StorageImplementation.GetStorageID(mainType.Name);
                var sessionStorageID = StorageImplementation.GetSessionStorageID(mainType.Name);

                ASP_DBEntities.ConnectionString = configuration["ASP_DBEntities"];  // globally
                var storage = StorageImplementation.GetStorage(configuration, httpContext, sessionStorageID);
                StorageImplementation.ClearIfRequested(httpContext, storage, storageID);

                Guid sessionOverride;
                Guid session;
                byte[] bytes;
                Func<byte[], byte[]> filter = null;
                T main = null;

                // ---------- Direct GET request ?session= from the Database ----------
                if (httpContext.Request.Method == WebRequestMethods.Http.Get &&
                    Guid.TryParse(httpContext.Request.Query["session"], out sessionOverride))
                {
                    using (var db = new ASP_DBEntities())
                    {
                        (bytes, filter) = StorageImplementation.DatabaseBytes(configuration, httpContext, storageID, sessionOverride);
                        main = DeserializeMain<T>(bytes, filter);
                    }
                }
                else
                {
                    // ---------- Load from Session ----------
                    if (storage == Storage.Session)
                    {
                        if (httpContext.Session.TryGetValue(storageID, out bytes))  // existing session
                        {
                            main = DeserializeMain<T>(bytes);
                        }
                        else
                        {
                            httpContext.Session.Set(storageID, new byte[0]);    // immediately establish the new session
                        }
                    }

                    // ---------- Load from Database ----------
                    else if (storage == Storage.Database)
                    {
                        if (Guid.TryParse(httpContext.Request.Cookies[storageID].FromCookieString()["session"], out session))   // existing session
                        {
                            (bytes, filter) = StorageImplementation.DatabaseBytes(configuration, httpContext, storageID, session);
                            main = DeserializeMain<T>(bytes, filter);
                        }
                        else
                        {
                            StorageImplementation.SaveDatabase(configuration, httpContext, new T());    // immediately save the cookie
                        }
                    }

                    // ---------- Load from X-ViewState Header ----------
                    else if (storage == Storage.Header &&
                                httpContext.Request.Headers.ContainsKey(StorageImplementation.HeaderName))
                    {
                        // input type=hidden from <input viewstate="@ViewBag.ViewState" />
                        var controllerString = httpContext.Request.Headers[StorageImplementation.HeaderName];
                        if (!String.IsNullOrEmpty(controllerString))
                        {
                            (bytes, filter) = StorageImplementation.ViewStateBytes(configuration, controllerString);
                            main = DeserializeMain<T>(bytes, filter);
                        }
                    }

                    if (main == null)
                    {
                        // ASP.NET Core implementation, no persistence, just return the new controller
                        main = new T();
                    }
                }

                MainAccessor<T>.Instance = main;
                return main;
            });
        }

        private static T DeserializeMain<T>(byte[] bytes, Func<byte[], byte[]> filter = null)
        {
            return (T)Serialization.Deserialize(bytes, filter);
        }
    }
}