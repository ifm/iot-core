namespace ifm.Common;

using System.IO;

/// <summary>
/// Provides file and directory helpers.
/// </summary>
public static class FileHelpers
{
    /// <summary>
    /// Clear directory.
    /// </summary>
    /// <param name="path">The path to the directory.</param>
    public static void ClearDirectory(string path)
    {
        var di = new DirectoryInfo(path);
        foreach (var item in di.GetFiles())
        {
            item.Delete();
        }
        foreach (var item in di.GetDirectories())
        {
            item.Delete(true);
        }
    }
}