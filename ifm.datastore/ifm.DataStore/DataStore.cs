using System.Text.Json.Serialization;

namespace ifm.DataStore;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using Contracts;

public class DataStore : IDataStore
{
    private const int Timeout = 5000;
    private readonly ReaderWriterLockSlim _lock = new (LockRecursionPolicy.SupportsRecursion);

    private readonly string _fileName;
    private readonly JsonObject _jsonRoot;
    private readonly DataStoreOptions _dataStoreOptions;

    public DataStore(string fileName) : this(fileName, new DataStoreOptions()) 
    {
    }

    public DataStore(string fileName, DataStoreOptions dataStoreOptions)
    {
        _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        _jsonRoot = ReadFromFile();
        _dataStoreOptions = dataStoreOptions ?? throw new ArgumentNullException(nameof(dataStoreOptions));
    }

    public T Get<T>(string section, string key)
    {
        if (!_lock.TryEnterReadLock(Timeout))
        {
            throw new Exception("The file is already in use by another task. Try it later.");
        }

        try
        {
            var configNode = _jsonRoot[section]?[key];
            return configNode != null ? configNode.Deserialize<T>() : default;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Set<T>(string section, string key, T value)
    {
        if (!_lock.TryEnterWriteLock(Timeout))
        {
            throw new Exception("The file is already in use by another task. Try it later.");
        }

        try
        {
            var sectionNode = _jsonRoot[section];
            if (sectionNode == null)
            {
                _jsonRoot[section] = sectionNode = new JsonObject();
            }
            sectionNode[key] = JsonNode.Parse(JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true }));

            WriteToFile();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Delete(string section)
    {
        if (!_lock.TryEnterWriteLock(Timeout))
        {
            throw new Exception("The file is already in use by another task. Try it later.");
        }

        try
        {
            _jsonRoot.Remove(section);
            WriteToFile();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Delete(string section, string key)
    {
        if (!_lock.TryEnterWriteLock(Timeout))
        {
            throw new Exception("The file is already in use by another task. Try it later.");
        }

        try
        {
            ((JsonObject)_jsonRoot[section])?.Remove(key);
            WriteToFile();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private JsonObject ReadFromFile()
    {
        JsonObject ret; 
        try
        {
            var bytes = File.ReadAllBytes(_fileName);
            ret = (JsonObject)JsonNode.Parse(Encoding.UTF8.GetString(bytes), new JsonNodeOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            ret = new JsonObject();
        }
        return ret;
    }

    private void WriteToFile()
    {
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = _dataStoreOptions.WriteNullValues ? JsonIgnoreCondition.Never : JsonIgnoreCondition.WhenWritingNull
        };

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_jsonRoot, serializerOptions));
        var tempFileName = $"{_fileName}.tmp";
        WriteAllBytes(tempFileName, bytes);
        ReplaceFile(tempFileName, _fileName);
    }

    private static void WriteAllBytes(string fileName, byte[] bytes)
    {
        using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        using var bufferedStream = new BufferedStream(fileStream);
        bufferedStream.Write(bytes, 0, bytes.Length);
        bufferedStream.Flush();
        fileStream.Flush(true);
    }

    private static void ReplaceFile(string sourceFileName, string destinationFileName)
    {
        if (!File.Exists(destinationFileName))
        {
            File.Move(sourceFileName, destinationFileName);
        }
        else
        {
            File.Replace(sourceFileName, destinationFileName, null);
        }
    }
}