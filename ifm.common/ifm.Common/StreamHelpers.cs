namespace ifm.Common;

using System.IO;

/// <summary>
/// Provides stream helpers.
/// </summary>
public static class StreamHelpers
{
    /// <summary>
    /// Read bytes from a stream
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>The bytes read.</returns>
    public static byte[] ReadBytes(Stream stream, int count)
    {
        var buffer = new byte[count];
        var offset = 0;
        while (count > 0)
        {
            var bytesThisTime = stream.Read(buffer, offset, count);
            offset += bytesThisTime;
            count -= bytesThisTime;
        }
        return buffer;
    }
}