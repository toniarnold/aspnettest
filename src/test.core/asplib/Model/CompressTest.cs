using asplib.Model;
using NUnit.Framework;
using System.IO.Compression;

namespace test.asplib.Model
{
    public class CompressTest
    {
        private byte[] raw = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        [Test]
        public void CompressDecompressTest()
        {
            Assert.That(Compress.IsGzipped(raw), Is.False);
            var compressed = Compress.Gzip(raw, CompressionLevel.Optimal);
            Assert.That(Compress.IsGzipped(compressed), Is.True);

            var decompressed = Compress.Gunzip(compressed);
            Assert.That(Compress.IsGzipped(decompressed), Is.False);
            Assert.That(decompressed, Is.EquivalentTo(raw));
        }

        [Test]
        public void DecompressRawTest()
        {
            var decompressed = Compress.Gunzip(raw);
            Assert.That(decompressed, Is.EquivalentTo(raw));
        }
    }
}