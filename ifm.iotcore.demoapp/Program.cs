namespace ifm.IoTCore.DemoApp;

using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

using ifm.Common;
using ifm.Common.Tree;
using ifm.Common.Variant;
using DataStore;
using DataStore.Contracts;
using Logger;
using Logger.Contracts;
using Common;
using Common.Exceptions;
using ElementManager.Contracts;
using ElementManager.Contracts.Elements;
using ElementManager.Contracts.Elements.Formats;
using ElementManager.Contracts.Elements.Valuations;
using MessageConverter.Json;
using NetAdapter.Http.Client;
using NetAdapter.Http.Server;
using NetAdapter.Http.Profile.HttpServer;
using Profile.DeviceInfo;
using Profile.DeviceTag;

internal class Program
{
    private static string _workingDirectory;
    private static IDataStore _dataStore;
    private static ILogger _logger;

    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<CommandLineParameters>(args).WithParsed(o => { Run(o.Id, new Uri(o.HttpUri)); });
    }

    private static void Run(string id, Uri httpUri)
    {
        _workingDirectory =
            Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings.Get("WorkingDirectory") ??
                                                   @"%ProgramData%\ifm\iodds");
        if (!Directory.Exists(_workingDirectory)) Directory.CreateDirectory(_workingDirectory);
        _dataStore = new DataStore(Path.Combine(_workingDirectory, "DataStore.txt"));
        _logger = new Log4NetLogger("DemoApp");

        try
        {
            _logger.Info("Application starting up");

            _logger.Info($"Create IoTCore '{id}'");
            var ioTCore = IoTCoreFactory.Create(id, true, "user", "password", true, _dataStore, _logger);

            _logger.Info($"IoTCore version: '{ioTCore.ApiVersion}'");

            _logger.Info("Register tree_changed event handler");
            ioTCore.Root.TreeChanged += TreeChangedEventHandler;

            _logger.Info("Create a JSON message converter for use in network adapters");
            var messageConverter = new MessageConverter();

            _logger.Info("Register http client factory");
            ioTCore.ClientNetAdapterManager.RegisterClientNetAdapterFactory(new HttpClientNetAdapterFactory(messageConverter));

            _logger.Info("Adding profile 'deviceinfo'");
            var deviceInfoProfileBuilder = new DeviceInfoProfileBuilder(ioTCore.ElementManager, ioTCore.ElementManager.Root, new DeviceInfoProvider());
            deviceInfoProfileBuilder.Build();

            _logger.Info("Adding profile 'devicetag'");
            var deviceTagProfileBuilder = new DeviceTagProfile(ioTCore.ElementManager, ioTCore.ElementManager.Root, _dataStore);
            deviceTagProfileBuilder.Build();

            _logger.Info("Adding profile 'httpserver'");
            var httpServerProfileBuilder = new HttpServerProfileBuilder(ioTCore.ElementManager, ioTCore.ElementManager.Root, new HttpServerProfileBuilderOptions
            {
                Port = httpUri.Port,
                ListenAddress = httpUri.Host
            });
            httpServerProfileBuilder.Build();

            _logger.Info("Create some elements");
            CreateElements(ioTCore.ElementManager, ioTCore.Root);

            _logger.Info($"Starting http server on {httpUri}");
            var httpServer = new HttpServerNetAdapter(ioTCore.MessageDispatcher, httpUri, messageConverter,
                new HttpServerNetAdapterConfiguration
                {
                    VisualizerCompactMiddlewareEnabled = true,
                    VisualizerCompactMiddlewareOptions = new VisualizerCompactMiddlewareOptions()
                },
                _logger, null, null);
            ioTCore.ServerNetAdapterManager.RegisterServerNetAdapter(httpServer);
            httpServer.Start();

            _logger.Info("Application running. Press any key to exit.");
            Console.ReadLine();
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
        }

    }

    private static void CreateElements(IElementManager elementManager, IBaseElement parentElement)
    {
        var struct1 = elementManager.CreateStructureElement(parentElement, "struct1", profiles: new List<string> { "demo" });

        var bool1 = elementManager.CreateDataElement(struct1,
            "bool1",
            GetBool1,
            SetBool1,
            true,
            _bool1,
            TimeSpan.FromMilliseconds(100),
            new BooleanFormat(new BooleanValuation(true)),
            null,
            "uid1");
        ((IEventElement)bool1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var int1 = elementManager.CreateDataElement(struct1,
            "int1",
            GetInt1,
            SetInt1,
            true,
            _int1,
            TimeSpan.FromMilliseconds(100),
            new Int32Format(new Int32Valuation(0, 100)),
            null,
            "uid2");
        ((IEventElement)int1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var float1 = elementManager.CreateDataElement(struct1,
            "float1",
            GetFloat1,
            SetFloat1,
            true,
            _float1,
            TimeSpan.FromMilliseconds(100),
            new FloatFormat(new FloatValuation(-9.999f, 9.999f, 3)),
            null,
            "uid5");
        ((IEventElement)float1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var string1 = elementManager.CreateDataElement(struct1,
            "string1",
            GetString1,
            SetString1,
            true,
            _string1,
            TimeSpan.FromMilliseconds(100),
            new StringFormat(new StringValuation(0, 10, "", "")),
            null,
            "uid6");
        ((IEventElement)string1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var intArray1 = elementManager.CreateDataElement(struct1,
            "intArray1",
            GetIntArray1,
            SetIntArray1,
            true,
            _intArray1,
            TimeSpan.FromMilliseconds(100),
            new ArrayFormat(new ArrayValuation(new Int32Format(new Int32Valuation(0, 100)), _intArray1.Length)),
            null,
            "uid9");
        ((IEventElement)intArray1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var floatArray1 = elementManager.CreateDataElement(struct1,
            "floatArray1",
            GetFloatArray1,
            SetFloatArray1,
            true,
            _floatArray1,
            TimeSpan.FromMilliseconds(100),
            new ArrayFormat(new ArrayValuation(new FloatFormat(new FloatValuation(-9.999f, 9.999f, 3)), _floatArray1.Length)),
            null,
            "uid10");
        ((IEventElement)floatArray1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var stringArray1 = elementManager.CreateDataElement(struct1,
            "stringArray1",
            GetStringArray1,
            SetStringArray1,
            true,
            _stringArray1,
            TimeSpan.FromMilliseconds(100),
            new ArrayFormat(new ArrayValuation(new StringFormat(new StringValuation(0, 3, "")), _stringArray1.Length)),
            null,
            "uid11");
        ((IEventElement)stringArray1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var intField = new ObjectValuation.Field("int1", new Int32Format(new Int32Valuation(-100, 100)), true);
        var floatField = new ObjectValuation.Field("float1", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), true);
        var stringField = new ObjectValuation.Field("string1", new StringFormat(new StringValuation(10, 10, "")), true);
        var stringArrayField = new ObjectValuation.Field("stringArray1", new ArrayFormat(new ArrayValuation(new StringFormat(new StringValuation(10, 10, "")))), true);

        var object1 = elementManager.CreateDataElement(struct1,
            "object1",
            GetObject1,
            SetObject1,
            true,
            _object1,
            TimeSpan.FromMilliseconds(100),
            new ObjectFormat(new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField, stringArrayField })),
            null,
            "uid12");
        ((IEventElement)object1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        var objectArray1 = elementManager.CreateDataElement(struct1,
            "objectArray1",
            GetObjectArray1,
            SetObjectArray1,
            true,
            _objectArray1,
            TimeSpan.FromMilliseconds(100),
            new ArrayFormat(new ArrayValuation(new ObjectFormat(new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField })), _stringArray1.Length)),
            null,
            "uid11");
        ((IEventElement)objectArray1.GetElementByIdentifier(Identifiers.DataChanged)).EventRaised += DataChangedEventHandler;

        elementManager.CreateActionServiceElement(struct1, "clear_screen", ClearScreenServiceHandler);

        elementManager.CreateSetterServiceElement<EventServiceData>(struct1, "event_service", EventServiceHandler);

    }

    private static bool GetBool1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _bool1;
    }

    private static void SetBool1(IBaseElement element, bool value)
    {
        _logger.Info($"Set {element.Address} called");

        _bool1 = value;
    }
    private static bool _bool1 = true;

    private static int GetInt1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _int1;
    }

    private static void SetInt1(IBaseElement element, int value)
    {
        _logger.Info($"Set {element.Address} called");

        var format = (Int32Format)element.Format;
        if (value < format.Valuation.Min || value > format.Valuation.Max)
        {
            throw new DataInvalidException($"Value must be in range {format.Valuation.Min} - {format.Valuation.Max}");
        }
        _int1 = value;
    }
    private static int _int1 = 10;

    private static float GetFloat1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _float1;
    }

    private static void SetFloat1(IBaseElement element, float value)
    {
        _logger.Info($"Set {element.Address} called");

        var format = (FloatFormat)element.Format;
        if (value < format.Valuation.Min || value > format.Valuation.Max)
        {
            throw new DataInvalidException($"Value must be in range {format.Valuation.Min} - {format.Valuation.Max}");
        }
        _float1 = value;
    }
    private static float _float1 = 1.2345f;

    private static string GetString1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _string1;
    }

    private static void SetString1(IBaseElement element, string value)
    {
        _logger.Info($"Set {element.Address} called");

        var format = (StringFormat)element.Format;
        var length = value.Length;
        if (length < format.Valuation.MinLength || length > format.Valuation.MaxLength)
        {
            throw new DataInvalidException($"Value length must be in range {format.Valuation.MinLength} - {format.Valuation.MaxLength}");
        }
        _string1 = value;
    }
    private static string _string1 = "Hallo";

    private static int[] GetIntArray1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _intArray1;
    }

    private static void SetIntArray1(IBaseElement element, int[] value)
    {
        _logger.Info($"Set {element.Address} called");

        var format = (ArrayFormat)element.Format;
        if (format.Valuation.Length != value.Length)
        {
            throw new DataInvalidException($"Value length must be {format.Valuation.Length}");
        }
        var itemFormat = (Int32Format)format.Valuation.Format;
        for (var idx = 0; idx < _intArray1.Length; idx++)
        {
            if (value[idx] < itemFormat.Valuation.Min || value[idx] > itemFormat.Valuation.Max)
            {
                throw new DataInvalidException($"Value must be in range {itemFormat.Valuation.Min} - {itemFormat.Valuation.Max}");
            }
        }
        _intArray1 = value;
    }
    private static int[] _intArray1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    private static float[] GetFloatArray1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _floatArray1;
    }

    private static void SetFloatArray1(IBaseElement element, float[] value)
    {
        _logger.Info($"Set {element.Address} called");

        var format = (ArrayFormat)element.Format;
        if (format.Valuation.Length != value.Length)
        {
            throw new DataInvalidException($"Value length must be {format.Valuation.Length}");
        }
        var itemFormat = (FloatFormat)format.Valuation.Format;
        for (var idx = 0; idx < _intArray1.Length; idx++)
        {
            if (value[idx] < itemFormat.Valuation.Min || value[idx] > itemFormat.Valuation.Max)
            {
                throw new DataInvalidException($"Value must be in range {itemFormat.Valuation.Min} - {itemFormat.Valuation.Max}");
            }
        }
        _floatArray1 = value;
    }
    private static float[] _floatArray1 = { -9.9f, -8.8f, -7.7f, -6.6f, -5.5f, 0, 6.6f, 7.7f, 8.8f, 9.9f };

    private static string[] GetStringArray1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _stringArray1;
    }

    private static void SetStringArray1(IBaseElement element, string[] value)
    {
        _logger.Info($"Set {element.Address} called");

        var format = (ArrayFormat)element.Format;
        if (format.Valuation.Length != value.Length)
        {
            throw new DataInvalidException($"Value length must be {format.Valuation.Length}");
        }
        var itemFormat = (StringFormat)format.Valuation.Format;
        for (var idx = 0; idx < _intArray1.Length; idx++)
        {
            var length = value[idx].Length;
            if (length < itemFormat.Valuation.MinLength || length > itemFormat.Valuation.MaxLength)
            {
                throw new DataInvalidException($"Value length must be in range {itemFormat.Valuation.MinLength} - {itemFormat.Valuation.MaxLength}");
            }
        }
        _stringArray1 = value;
    }
    private static string[] _stringArray1 = { "s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10" };

    private static TestClass GetObject1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _object1;
    }

    private static void SetObject1(IBaseElement element, TestClass value)
    {
        _logger.Info($"Set {element.Address} called");

        var format = (ObjectFormat)element.Format;
        var itemField = format.Valuation.Fields.First(x => x.Name == "int1");
        var int1Format = (Int32Format)itemField.Format;
        if (value.Int1 < int1Format.Valuation.Min || value.Int1 > int1Format.Valuation.Max)
        {
            throw new DataInvalidException($"Field 'int1' must be in range {int1Format.Valuation.Min} - {int1Format.Valuation.Max}");
        }
        itemField = format.Valuation.Fields.First(x => x.Name == "float1");
        var float1Format = (FloatFormat)itemField.Format;
        if (value.Float1 < float1Format.Valuation.Min || value.Float1 > float1Format.Valuation.Max)
        {
            throw new DataInvalidException($"Field 'float1' must be in range {float1Format.Valuation.Min} - {float1Format.Valuation.Max}");
        }
        itemField = format.Valuation.Fields.First(x => x.Name == "string1");
        var string1Format = (StringFormat)itemField.Format;
        var length = value.String1.Length;
        if (length < string1Format.Valuation.MinLength || length > string1Format.Valuation.MaxLength)
        {
            throw new DataInvalidException($"Field 'string1' length must be in range {string1Format.Valuation.MinLength} - {string1Format.Valuation.MaxLength}");
        }
        _object1 = value;
    }
    private static TestClass _object1 = new();

    private static TestClass[] GetObjectArray1(IBaseElement element)
    {
        _logger.Info($"Get {element.Address} called");

        return _objectArray1;
    }

    private static void SetObjectArray1(IBaseElement element, TestClass[] value)
    {
        _objectArray1 = value;
    }

    private static TestClass[] _objectArray1 = [new TestClass(), new TestClass(), new TestClass()];

    private static void ClearScreenServiceHandler(IBaseElement element)
    {
        _logger.Info($"{element.Address} called");

        Console.Clear();
    }

    private static void EventServiceHandler(IBaseElement element, EventServiceData data)
    {
        _logger.Info($"{element.Address} called");

        _logger.Info($"Event number={data.Number}");
        _logger.Info($"Event source={data.Sender}");
        _logger.Info($"Subscription Id={data.SubscribeId}");
        if (data.Payload != null)
        {
            foreach (var (key, value) in data.Payload)
            {
                _logger.Info($"{key}: timestamp={value.TimeStamp} value={value.Data} (code={value.Code})");
            }
        }
        if (data.DataPoints != null)
        {
            foreach (var (key, values) in data.DataPoints)
            {
                foreach (var value in values)

                    _logger.Info($"{key}: timestamp={value.TimeStamp} value={value.Data} (code={value.Code})");
            }
        }
    }

    private static void DataChangedEventHandler(object sender, EventArgs args)
    {
        var element = (IEventElement)sender;
        _logger.Info($"{element.Address} called");

        if ((IReadDataElement)element.Parent is { } dataElement)
        {
            _logger.Info($"New value={dataElement.Value}");
        }
    }
	
    private static void TreeChangedEventHandler(object sender, TreeChangedEventArgs<IBaseElement> args)
    {
        _logger.Info($"Tree changed: Parent={args.SourceNode} Child={args.TargetNode} Action={args.Action}");
    }

}

internal class EventServiceData
{
    [VariantProperty("eventno", IgnoredIfNull = true)]
    public int Number { get; set; }

    [VariantProperty("srcurl", IgnoredIfNull = true)]
    public string Sender { get; set; }

    [VariantProperty("subscribeid", IgnoredIfNull = true)]
    public int? SubscribeId { get; set; }

    [VariantProperty("payload", IgnoredIfNull = true)]
    public Dictionary<string, CodeDataPair> Payload { get; set; }

    [VariantProperty("data_points", IgnoredIfNull = true)]
    public Dictionary<string, List<CodeDataPair>> DataPoints { get; set; }
}

public class TestClass : IEquatable<TestClass>
{
    [VariantProperty("int1", Required = true)] public int Int1 { get; set; }
    [VariantProperty("float1", Required = true)] public float Float1 { get; set; }
    [VariantProperty("string1", Required = true)] public string String1 { get; set; }
    [VariantProperty("stringArray1", Required = true)] public string[] StringArray1 { get; set; }


    public TestClass()
    {
        String1 = "Hallo";
        Int1 = 10;
        Float1 = 1.2345f;
        StringArray1 = ["s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10"];
    }

    public bool Equals(TestClass other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Int1 == other.Int1 &&
               Float1.EqualsWithPrecision(other.Float1) &&
               String1 == other.String1 &&
               StringArray1.SequenceEqual(other.StringArray1);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;

        return Equals((TestClass)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Int1, Float1, String1);
    }
}

public class DeviceInfoProvider : IDeviceInfo
{
    public string DeviceName => "DeviceName";
    public string DeviceClass => "DeviceClass";
    public string ProductInstanceUri => "ProductInstanceUri";
    public string DeviceFamily => "DeviceFamily";
    public string DeviceVariant => "DeviceVariant";
    public byte[] DeviceSymbol => null;
    public byte[] DeviceIcon => null;
    public string DeviceManual => "DeviceManual";
    public string SerialNumber => "serialNumber";
    public string ProductId => "ProductId";
    public string ProductName => "ProductName";
    public string ProductCode => "productCode";
    public string ProductText => "ProductText";
    public string OrderNumber => "OrderNumber";
    public string ProductionDate => "ProductionDate";
    public string ProductionCode => "ProductionCode";
    public string DeviceRevision => "DeviceRevision";
    public string RevisionCounter => "RevisionCounter";
    public string HwRevision => "hwRevision";
    public string HwVersion => "HwVersion";
    public string SwRevision => "SwRevision";
    public string SwVersion => "SwVersion";
    public string BootloaderRevision => "BootloaderRevision";
    public string Vendor => "Vendor";
    public IDeviceInfo.FieldbusTypeEnum? FieldbusType => IDeviceInfo.FieldbusTypeEnum.IoT;
    public string VendorText => "VendorText";
    public string VendorUrl => "VendorUrl";
    public byte[] VendorLogo => null;
    public string ProductWebsite => "ProductWebsite";
    public string SupportContact => "SupportContact";
    public byte[] Icon => null;
    public byte[] Image => null;
    public string Standards => "Standards";
    public string ApplicationSpecificTag { get; set; } = "***";
}

internal class CommandLineParameters
{
    [Option('i', "id", Required = true, HelpText = "The id of the iotcore", Default = "id0")]
    public string Id { get; set; }

    [Option('h', "http-uri", Required = true, HelpText = "Uri to start the http server")]
    public string HttpUri { get; set; }
}
