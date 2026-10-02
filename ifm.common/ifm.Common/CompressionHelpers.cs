namespace ifm.Common;

using System;
using System.IO;
using System.IO.Compression;

/// <summary>
/// Provides methods for compressing and decompressing data.
/// </summary>
public static class CompressionHelpers
{
    private const uint ZipHeader = 0x04034b50;
    private const ushort GZipHeader = 0x8b1f;

    // Some other possible but not implemented compression formats.
    //private const ushort TarHeader = 0x9d1f;
    //private const ushort LzhHeader = 0xa01f;
    //private const uint Bzip2Header = 0xff685a42;
    //private const uint LZipHeader = 0x50495a4c;
    
    /// <summary>
    /// Checks if data is a zip file stream.
    /// </summary>
    /// <param name="data">The data to check.</param>
    /// <returns>true if it is a Zip format; otherwise false.</returns>
    public static bool IsZipFormat(byte[] data)
    {
        return BitConverter.ToUInt32(data, 0) == ZipHeader;
    }

    /// <summary>
    /// Checks if data is a gzip file stream.
    /// </summary>
    /// <param name="data">The data to check.</param>
    /// <returns>true if it is a GZip format; otherwise false.</returns>
    public static bool IsGZipFormat(byte[] data)
    {
        return BitConverter.ToUInt16(data, 0) == GZipHeader;
    }

    /// <summary>
    /// Compresses the provided data.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <param name="level">The compression level.</param>
    /// <returns>The compressed data.</returns>
    public static byte[] GZipCompress(byte[] data, CompressionLevel level = CompressionLevel.Optimal)
    {
        var gZipStream = new GZipStream(new MemoryStream(), level);
        gZipStream.Write(data, 0, data.Length);
        var stream = (MemoryStream)gZipStream.BaseStream;
        gZipStream.Dispose();
        return stream.ToArray();
    }

    /// <summary>
    /// Decompresses the provided data.
    /// </summary>
    /// <param name="data">The data to decompress.</param>
    /// <returns>The decompressed data.</returns>
    public static byte[] GZipDecompress(byte[] data)
    {
        var gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
        var decompressedBytes = new byte[data.Length];
        var stream = new MemoryStream();
        int len;
        while ((len = gZipStream.Read(decompressedBytes, 0, data.Length)) > 0)
        {
            stream.Write(decompressedBytes, 0, len);
        }
        gZipStream.Dispose();
        return stream.ToArray();
    }

    /// <summary>
    /// Unpacks a zip archive
    /// </summary>
    /// <param name="archivePath">The path to the archive file.</param>
    /// <param name="directoryPath">The path to the directory where the archive is unpacked.</param>
    public static void UnpackArchive(string archivePath, string directoryPath)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        foreach (var entry in archive.Entries)
        {
            var path = Path.Combine(directoryPath, entry.FullName);
            if (entry.Name == string.Empty)
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                SafeFileWriter.Write(path, entry.Open());
            }
        }
    }

}