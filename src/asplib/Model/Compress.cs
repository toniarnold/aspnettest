using System.IO;
using System.IO.Compression;

namespace asplib.Model
{
    /// <summary>
    /// Encapsulates the gzip compression the ViewState or the [Main].[main] storage column
    /// </summary>
    public class Compress
    {
        internal static readonly byte[] GZIP_MAGIC_NUMNER = { 0x1f, 0x8b };

        /// <summary>
        /// Gzip the raw byte array
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static byte[] Gzip(byte[] raw, CompressionLevel level)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var rawStream = new MemoryStream(raw))
                using (var compressor = new GZipStream(compressedStream, level))
                {
                    rawStream.CopyTo(compressor);
                }
                return compressedStream.ToArray();
            }
        }

        /// <summary>
        /// Gunzip the raw byte array. Ruboust: if it doesn't start with the
        /// gzip magic number, assume that it was stored uncompressed and return
        /// it unchanged.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static byte[] Gunzip(byte[] compressed)
        {
            if (IsGzipped(compressed))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    using (var compressedStream = new MemoryStream(compressed))
                    using (var decompressor = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        decompressor.CopyTo(decompressedStream);
                    }
                    return decompressedStream.ToArray();
                }
            }
            else { return compressed; }
        }

        internal static bool IsGzipped(byte[] compressd)
        {
            return compressd != null &&
                   compressd.Length >= 2 &&
                   compressd[0] == GZIP_MAGIC_NUMNER[0] &&
                   compressd[1] == GZIP_MAGIC_NUMNER[1];
        }
    }
}