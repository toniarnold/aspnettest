using asplib.Model;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace test.asplib.Model
{
    public class SerializationMetering
    {
        [TestCaseSource(nameof(SerializationCases))]
        public void Measure(IConfiguration config, object[] obj)
        {
            var filter = StorageImplementation.CompressEncryptFilter(config);
            var bytes = Serialization.Serialize(obj, filter);
            TestContext.Write($"{obj[0]} {config["ViewStateCompressionLevel"]} Size:{bytes.Length:e}");
        }

        private static string[] s_levels = new string[] { "NoCompression", "Fastest", "Optimal", "SmallestSize" };

        private static object[] s_objects = new[]
        {
            new object[] { "HighEntropy(big)", HighEntropy(1000000) },
            new object[] { "HighEntropy(small)", HighEntropy(10000) },
            new object[] { "LowEntropy(big)", LowEntropy(100000) },
            new object[] { "LowEntropy(small)", LowEntropy(1000) },
        };

        private static List<int> HighEntropy(int len)
        {
            var l = new List<int>();
            var rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                l.Add(rnd.Next());
            }
            return l;
        }

        private static List<string> LowEntropy(int len)
        {
            var l = new List<string>();
            for (int i = 0; i < len; i++)
            {
                l.Add($"{i}...............................");
            }
            return l;
        }

        private static IEnumerable SerializationCases =
            from level in s_levels
            from obj in s_objects
            select new[]
            {
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>()
                        {
                            { "EncryptViewStateKey", "<-- secret w/o server affinity -->" },
                            { "ViewStateCompressionLevel", level },
                        }
                    ).Build(),
                obj
             };
    }
}