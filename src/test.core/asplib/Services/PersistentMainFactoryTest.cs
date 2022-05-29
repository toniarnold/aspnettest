using asplib.Model;
using asplib.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace test.asplib.Services
{
    public class PersistentMainFactoryTest
    {
        /// <summary>
        /// Not serializable
        /// </summary>
        public class Service
        { }

        /// <summary>
        /// Usage pattern: Serializable Main with a contained injected
        /// non-serializable service
        /// </summary>
        [Serializable]
        public class Main
        {
            [NonSerialized]
            private Service _service = default!;

            [Inject]
            public Service Service
            {
                get => _service;
                set => _service = value;
            }
        }

        [Test]
        public void InstantiateMainTest()
        {
            var services = new ServiceCollection();
            services.AddSingleton<Service>();
            var provider = services.BuildServiceProvider();
            var main = PersistentMainFactory<Main>.Instantiate(provider);
            Assert.That(main, Is.Not.Null);
            Assert.That(main.Service, Is.Not.Null);
        }

        [Test]
        public void PerformPropertyInjectionTest()
        {
            var main = new Main();
            var services = new ServiceCollection();
            services.AddSingleton<Service>();
            var provider = services.BuildServiceProvider();
            Assert.That(main.Service, Is.Null);
            PersistentMainFactory<Main>.PerformPropertyInjection(provider, main);
            Assert.That(main.Service, Is.Not.Null);
            // Perform the serialize/deserialize/rehydrate roundtrip
            var serialized = Serialization.Serialize(main);  // succeeds due to [NonSerialized]
            var copy = (Main)Serialization.Deserialize(serialized);
            Assert.That(copy.Service, Is.Null); ;   // not serialized
            PersistentMainFactory<Main>.PerformPropertyInjection(provider, copy); // will use _cachedInitializer
            Assert.That(main.Service, Is.Not.Null);
        }
    }
}