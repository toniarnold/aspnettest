// Parts copy-pasted from Microsoft.AspNetCore.Components.ComponentFactory under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace asplib.Services
{
    /// <summary>
    /// Simplified generic (in the context of AddPersistent<T>) version of
    /// Microsoft.AspNetCore.Components.ComponentFactory to support the [Inject]
    /// attribute w/o externally supplied ComponentActivator (as the
    /// DefaultComponentActivator is internal). This provides a functionality
    /// called "Rehydration" in BizTalk terms.
    /// </summary>
    internal static class PersistentMainFactory<T>
        where T : class, new()  // for serialization
    {
        private const BindingFlags _injectablePropertyBindingFlags
            = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static Action<IServiceProvider, T>? _cachedInitializer;

        /// <summary>
        /// Create a new instance of T and perform [Inject] attribute property injection.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        internal static T Instantiate(IServiceProvider serviceProvider)
        {
            T main = (T)ActivatorUtilities.CreateInstance(serviceProvider, typeof(T));  // non-generic version required
            PerformPropertyInjection(serviceProvider, main);
            return main;
        }

        /// <summary>
        /// Instantiates [Inject] property attribute services.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="main"></param>
        internal static void PerformPropertyInjection(IServiceProvider serviceProvider, T main)
        {
            // Original: "This is thread-safe because _cachedInitializers is a
            // ConcurrentDictionary." Generic version doesn't need a dictionary:
            // might generate the initializer more than once for a given type,
            // but would still produce the correct result.
            _cachedInitializer ??= (Action<IServiceProvider, T>)CreateInitializer();
            _cachedInitializer(serviceProvider, main);
        }

        private static Action<IServiceProvider, T> CreateInitializer()
        {
            // Do all the reflection up front
            List<(string name, Type propertyType, Action<T, object>)>? injectables = null;
            foreach (var property in typeof(T).GetProperties(_injectablePropertyBindingFlags))
            {
                if (!property.IsDefined(typeof(InjectAttribute)))
                {
                    continue;
                }

                injectables ??= new();
                injectables.Add((property.Name, property.PropertyType, (T obj, object value) => property.SetValue(obj, value)));
            }

            if (injectables is null)
            {
                return static (_, _) => { };
            }

            return Initialize;

            // Return an action whose closure can write all the injected properties
            // without any further reflection calls (just typecasts)
            void Initialize(IServiceProvider serviceProvider, T main)
            {
                foreach (var (propertyName, propertyType, setter) in injectables)
                {
                    var serviceInstance = serviceProvider.GetService(propertyType);
                    if (serviceInstance == null)
                    {
                        throw new InvalidOperationException($"Cannot provide a value for property " +
                            $"'{propertyName}' on type '{typeof(T).FullName}'. There is no " +
                            $"registered service of type '{propertyType}'.");
                    }

                    setter(main, serviceInstance);
                }
            }
        }
    }
}