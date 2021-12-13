namespace ifmIoTCore.Utilities
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    public static class CompressionHelper
    {
        private static readonly byte[] ZipBytes1 = { 0x50, 0x4b, 0x03, 0x04 };
        private static readonly byte[] ZipBytes2 = { 0x50, 0x4b, 0x05, 0x06 };
        private static readonly byte[] ZipBytes3 = { 0x50, 0x4b, 0x07, 0x08 };
        private static readonly byte[] GzipBytes = { 0x1f, 0x8b };
        //private static readonly byte[] TarBytes = { 0x1f, 0x9d };
        //private static readonly byte[] LzhBytes = { 0x1f, 0xa0 };
        //private static readonly byte[] Bzip2Bytes = { 0x42, 0x5a, 0x68 };
        //private static readonly byte[] LzipBytes = { 0x4c, 0x5a, 0x49, 0x50 };

        public static bool IsZipFormat(byte[] dataBytes)
        {
            foreach (var headerBytes in new[] { ZipBytes1, ZipBytes2, ZipBytes3, GzipBytes })//, TarBytes, LzhBytes, Bzip2Bytes, LzipBytes })
            {
                if (dataBytes.Length < headerBytes.Length)
                    return false;

                return !headerBytes.Where((t, i) => t != dataBytes[i]).Any();
            }

            return false;
        }

        public static byte[] UnCompress(byte[] compressed)//, CompressionMode mode)
        {
            var decompressedBytes = new byte[compressed.Length];
            var stream = new MemoryStream();
            //using (var unCompressedStream = new GZipStream(new MemoryStream(compressed), mode))
            using (var unCompressedStream = new GZipStream(new MemoryStream(compressed), CompressionMode.Decompress))
            {
                int len;
                while ((len = unCompressedStream.Read(decompressedBytes, 0, compressed.Length)) > 0)
                    stream.Write(decompressedBytes, 0, len);

                unCompressedStream.Close();
                return stream.ToArray();
            }
        }

        public static byte[] Compress(byte[] uncompressed, CompressionLevel level)
        {
            using (var compressedStream = new GZipStream(new MemoryStream(), level))
            {
                compressedStream.Write(uncompressed, 0, uncompressed.Length);
                var response = (MemoryStream)compressedStream.BaseStream;
                compressedStream.Close();
                return response.ToArray();
            }
        }
    }
}