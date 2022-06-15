using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Diagnostics;
using TestContext = Bunit.TestContext;

namespace iselenium
{
    /// <summary>
    /// Extended Setup for our rich Component base classes in asplib.blazor
    /// </summary>
    public class BUnitTestContext : TestContext
    {
        [OneTimeSetUp]
        public void RegisterServices()
        {
            Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build());
            Services.AddSingleton<ProtectedSessionStorage>(new ProtectedSessionStorage(JSInterop.JSRuntime, new EphemeralDataProtectionProvider()));
            Services.AddSingleton<ProtectedLocalStorage>(new ProtectedLocalStorage(JSInterop.JSRuntime, new EphemeralDataProtectionProvider()));
            Services.AddSingleton<IWebHostEnvironment>(Mock.Of<IWebHostEnvironment>());
            JSInterop.Mode = JSRuntimeMode.Loose;
        }

        /// <summary>
        /// Non-nullable typed accessor function to get the DynamicComponent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        protected T Dynamic<T>(DynamicComponent component)
        {
            Trace.Assert(component.Instance != null,
                $"DynamicComponent.Instance for {typeof(T)} is null");
            object dynamicObject = component.Instance ?? default!;
            Trace.Assert(typeof(T).IsAssignableFrom(dynamicObject.GetType()),
                $"{dynamicObject.GetType()} is not of type {typeof(T)}");
            return (T)dynamicObject;
        }
    }
}