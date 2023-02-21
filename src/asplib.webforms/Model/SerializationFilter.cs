using System;
using System.Configuration;
using System.IO.Compression;

namespace asplib.Model
{
    /// <summary>
    /// The compression filter is used both in asplib.Model.Main and IStorageControl,
    /// if it is to be combined with another filter the composition is
    /// 1.compress->2.filter and 1 filter->2.decompress
    /// </summary>
    internal class SerializationFilter
    {
        /// <summary>
        /// Return the Gzip compression filter if configured, otherwise null.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static Func<byte[], byte[]> CompressFilter(Func<byte[], byte[]> filter = null)
        {
            Func<byte[], byte[]> compressFilter = null;
            var compressionLevel = GetViewStateCompressionLevel();
            if (GetViewStateCompressionLevel() != CompressionLevel.NoCompression)
            {
                compressFilter = x => Compress.Gzip(x, compressionLevel);
            }
            return Serialization.ComposeFilters(compressFilter, filter);
        }

        /// <summary>
        /// Return the Gunzip compression filter if configured, otherwise null.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static Func<byte[], byte[]> DecompressFilter(Func<byte[], byte[]> filter = null)
        {
            Func<byte[], byte[]> decompressFilter = null;
            if (GetViewStateCompressionLevel() != CompressionLevel.NoCompression)
            {
                decompressFilter = x => Compress.Gunzip(x);
            }
            return Serialization.ComposeFilters(filter, decompressFilter);
        }

        /// <summary>
        /// Return the configured ViewStateCompressionLevel or
        /// CompressionLevel.NoCompression if not configured.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static CompressionLevel GetViewStateCompressionLevel()
        {
            if (Enum.TryParse<CompressionLevel>(ConfigurationManager.AppSettings["ViewStateCompressionLevel"], out var level))
            {
                return level;
            }
            else
            {
                return CompressionLevel.NoCompression;
            }
        }
    }
}