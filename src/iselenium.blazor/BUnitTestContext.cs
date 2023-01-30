using asplib.Components;
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
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
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
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            Services.AddSingleton<IConfiguration>(config);
            Services.AddSingleton<ProtectedSessionStorage>(new ProtectedSessionStorage(JSInterop.JSRuntime, new EphemeralDataProtectionProvider()));
            Services.AddSingleton<ProtectedLocalStorage>(new ProtectedLocalStorage(JSInterop.JSRuntime, new EphemeralDataProtectionProvider()));
            Services.AddSingleton<IWebHostEnvironment>(Mock.Of<IWebHostEnvironment>());
            JSInterop.Mode = JSRuntimeMode.Loose;

            // For synchronized Click() with TestFocus:
            Configure(String.IsNullOrWhiteSpace(config?["RequestTimeout"]) ? 1 :
                        config.GetValue<int>("RequestTimeout"));
        }

        protected void Configure(int requestTimeout)
        {
            SeleniumExtensionBase.RequestTimeout = requestTimeout;
        }

        /// <summary>
        /// TestFocus-setting override of Bunit.TestContext.RenderComponent:
        /// Instantiates and performs a first render of a component of type <typeparamref name="TComponent"/>.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component to render.</typeparam>
        /// <param name="parameters">Parameters to pass to the component when it is rendered.</param>
        /// <returns>The rendered <typeparamref name="TComponent"/>.</returns>
        public override IRenderedComponent<TComponent> RenderComponent<TComponent>(params ComponentParameter[] parameters)
        {
            if (typeof(ITestFocus).IsAssignableFrom(typeof(TComponent)))
            {
                TestFocus.SetFocus(typeof(TComponent));
            }
            return base.RenderComponent<TComponent>(parameters);
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