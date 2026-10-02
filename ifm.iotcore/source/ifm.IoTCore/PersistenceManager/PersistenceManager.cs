namespace ifm.IoTCore.PersistenceManager;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Logger.Contracts;
using ifm.Common;
using ifm.Common.Variant;
using ElementManager.Contracts;
using ElementManager.Contracts.Elements;
using Contracts;

public class PersistenceManager : IPersistenceManager
{
    private readonly SafeFileWriter _persistenceFile;
    private readonly ILogger _logger;
    private readonly IElementManager _elementManager;

    private readonly LinkedList<IPersistenceManager.ServiceCallInfo> _items = [];
    private bool _isRestoring;

    public PersistenceManager(string fileName, IElementManager elementManager, ILogger logger)
    {
        _elementManager = elementManager ?? throw new ArgumentNullException(nameof(elementManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        string path;
        if (string.IsNullOrEmpty(fileName))
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ifm", "iotcore");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, "persist.txt");
        }
        else
        {
            path = fileName;
        }
        _persistenceFile = new SafeFileWriter(path);

        try
        {
            ReadFromFile();
        }
        catch (Exception e)
        {
            _logger.Error($"Persistence file is corrupted and is not loaded. Error: {e.Message}");
        }
    }

    public void Lock()
    {
        Monitor.Enter(_items);
    }

    public void Unlock()
    {
        Monitor.Exit(_items);
    }

    public IReadOnlyList<IPersistenceManager.ServiceCallInfo> PersistedItems
    {
        get
        {
            lock (_items)
            {
                return [.. _items];
            }
        }
    }

    public void Persist(string serviceAddress, Variant serviceData, bool flush)
    {
        lock (_items)
        {
            if (_isRestoring) return;
            try
            {
                // Identical command already exists
                if (_items.Any(x => x.ServiceAddress == serviceAddress && x.ServiceData.Equals(serviceData))) return;

                var item = new IPersistenceManager.ServiceCallInfo(serviceAddress, serviceData);
                _items.AddLast(item);
                if (flush) WriteToFile();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }

    public void Remove(IPersistenceManager.ServiceCallInfo item, bool flush)
    {
        lock (_items)
        {
            if (_isRestoring) return;
            try
            {
                _items.Remove(item);
                if (flush) WriteToFile();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }

    public void Flush()
    {
        WriteToFile();
    }

    public void Restore()
    {
        lock (_items)
        {
            _isRestoring = true;
            foreach (var item in _items)
            {
                try
                {
                    var element = _elementManager.GetElementByAddress(item.ServiceAddress);
                    if (element == null)
                    {
                        throw new Exception($"Restore persisted service {item.ServiceAddress} is not found");
                    }
                    if (element is not IServiceElement serviceElement)
                    {
                        throw new Exception($"Restore persisted service {item.ServiceAddress} is not a service");
                    }
                    serviceElement.Invoke(item.ServiceData);
                }
                catch (Exception e)
                {
                    _logger.Error($"Restore persisted service {item.ServiceAddress} failed: Error: {e.Message}");
                }
            }
            _isRestoring = false;
        }
    }

    private void WriteToFile()
    {
        var items = new List<Tuple<string, string>>();
        foreach (var item in _items)
        {
            items.Add(new Tuple<string, string>(item.ServiceAddress, Variant.ToJsonElement(item.ServiceData).ToString()));
        }

        _persistenceFile.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true })));
    }

    private void ReadFromFile()
    {
        if (_persistenceFile.Exists)
        {
            var items = JsonSerializer.Deserialize<List<Tuple<string, string>>>(Encoding.UTF8.GetString(_persistenceFile.Read()));

            if (items == null) return;
            foreach (var item in items)
            {
                _items.AddLast(new IPersistenceManager.ServiceCallInfo(item.Item1, Variant.FromJsonElement(JsonDocument.Parse(item.Item2).RootElement, false)));
            }
        }
    }
}