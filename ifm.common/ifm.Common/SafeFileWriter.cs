namespace ifm.Common;

using System.IO;

/// <summary>
/// Provides a safe and atomic file writer. 
/// Initializes an instance of the class.
/// </summary>
/// <param name="filePath">The file path.</param>
/// <param name="backup">If true a backup is created from the old version of the file; otherwise not.</param>
public class SafeFileWriter(string filePath, bool backup = false)
{
    private readonly string _tempFilePath = $"{filePath}.tmp";
    private readonly string _backupFilePath = backup ? $"{filePath}.bak" : null;

    /// <summary>
    /// Gets the file path.
    /// </summary>
    public string FilePath => filePath;

    /// <summary>
    /// Checks if the file exists.
    /// </summary>
    public bool Exists => File.Exists(filePath);

    /// <summary>
    /// Gets the file content as byte array in binary mode.
    /// </summary>
    /// <returns>The file content as string.</returns>
    public byte[] Read() => File.ReadAllBytes(filePath);

    /// <summary>
    /// Writes the byte array to the file.
    /// If the file already exists, it is overwritten.
    /// </summary>
    /// <param name="bytes">The bytes to write.</param>
    public void Write(byte[] bytes)
    {
        WriteTempFile(new MemoryStream(bytes));
        ReplaceTempFile();
    }

    /// <summary>
    /// Writes the stream to the file.
    /// If the file already exists, it is overwritten.
    /// </summary>
    /// <param name="stream">The stream to write.</param>
    public void Write(Stream stream)
    {
        WriteTempFile(stream);
        ReplaceTempFile();
    }

    private void WriteTempFile(Stream stream)
    {
        using var targetStream = new FileStream(_tempFilePath, FileMode.Create, FileAccess.Write);
        using var bufferedStream = new BufferedStream(targetStream);
        stream.CopyTo(bufferedStream);
        bufferedStream.Flush();
        targetStream.Flush(true);
    }

    private void ReplaceTempFile()
    {
        if (!File.Exists(filePath))
        {
            File.Move(_tempFilePath, filePath);
        }
        else
        {
            File.Replace(_tempFilePath, filePath, _backupFilePath);
        }
    }

    /// <summary>
    /// Writes the stream to the file.
    /// If the file already exists, it is overwritten.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="stream">The stream to write.</param>
    public static void Write(string filePath, Stream stream)
    {
        var safeFileWriter = new SafeFileWriter(filePath);
        safeFileWriter.Write(stream);
    }
}