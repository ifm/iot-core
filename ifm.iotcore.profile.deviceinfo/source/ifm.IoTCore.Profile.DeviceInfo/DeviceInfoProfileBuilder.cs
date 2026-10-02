namespace ifm.IoTCore.Profile.DeviceInfo;

using System;
using System.Linq;
using System.Collections.Generic;
using ElementManager.Contracts;
using ElementManager.Contracts.Elements;
using ElementManager.Contracts.Elements.Formats;
using ElementManager.Contracts.Elements.Valuations;

/// <summary>
/// Builds the deviceinfo profile.
/// </summary>
public class DeviceInfoProfileBuilder
{
    public const string DeviceNameIdentifier = "devicename";
    public const string HwVersionIdentifier = "hwversion";
    public const string VendorIdentifier = "vendor";
    public const string BootLoaderRevisionIdentifier = "bootloaderrevision";
    public const string SwVersionIdentifier = "swversion";
    public const string SwRevisionIdentifier = "swrevision";
    public const string HwRevisionIdentifier = "hwrevision";
    public const string ProductionDateIdentifier = "productiondate";
    public const string OrderNumberIdentifier = "ordernumber";
    public const string ProductCodeIdentifier = "productcode";
    public const string ProductNameIdentifier = "productname";
    public const string SerialNumberIdentifier = "serialnumber";
    public const string DeviceFamilyIdentifier = "devicefamily";
    public const string FieldBusTypeIdentifier = "fieldbustype";

    private readonly IElementManager _elementManager;
    private readonly IDeviceInfo _deviceInfo;
    private readonly IBaseElement _targetElement;

    private const string ProfileName = "deviceinfo";
    private readonly List<string> _parameterProfile = new() { "parameter" };

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceInfoProfileBuilder"/> class.
    /// </summary>
    /// <param name="elementManager">The elementmanager instance.</param>
    /// <param name="targetElement">The element into which the profile will be built.</param>
    /// <param name="deviceInfo">The device information provider.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DeviceInfoProfileBuilder(IElementManager elementManager,
        IBaseElement targetElement,
        IDeviceInfo deviceInfo)
    {
        _elementManager = elementManager ?? throw new ArgumentNullException(nameof(elementManager));
        _targetElement = targetElement ?? throw new ArgumentNullException(nameof(targetElement));
        _deviceInfo = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
    }

    /// <summary>
    /// Builds the profile into the target element.
    /// </summary>
    /// <param name="timeout">The cache timeout for the created data elements.</param>
    public void Build(TimeSpan? timeout = null)
    {
        var deviceInfoElement = _targetElement.GetElementByIdentifier(ProfileName);

        if (deviceInfoElement == null)
        {
            deviceInfoElement = _elementManager.CreateStructureElement(
                _targetElement,
                ProfileName,
                profiles: new List<string> { ProfileName }, acquireLock: false);
        }

        if (_deviceInfo.DeviceName != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                DeviceNameIdentifier,
                _ => _deviceInfo.DeviceName,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.DeviceFamily != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                DeviceFamilyIdentifier,
                _ => _deviceInfo.DeviceFamily,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.SerialNumber != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                SerialNumberIdentifier,
                _ => _deviceInfo.SerialNumber,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.ProductName != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                ProductNameIdentifier,
                _ => _deviceInfo.ProductName,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.ProductCode != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                ProductCodeIdentifier,
                _ => _deviceInfo.ProductCode,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.OrderNumber != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                OrderNumberIdentifier,
                _ => _deviceInfo.OrderNumber,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.ProductionDate != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                ProductionDateIdentifier,
                _ => _deviceInfo.ProductionDate,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.HwRevision != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                HwRevisionIdentifier,
                _ => _deviceInfo.HwRevision,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.SwRevision != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                SwRevisionIdentifier,
                _ => _deviceInfo.SwRevision,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.SwVersion != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                SwVersionIdentifier,
                _ => _deviceInfo.SwVersion,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.BootloaderRevision != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                BootLoaderRevisionIdentifier,
                _ => _deviceInfo.BootloaderRevision,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.Vendor != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                VendorIdentifier,
                _ => _deviceInfo.Vendor,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.HwVersion != null)
        {
            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                HwVersionIdentifier,
                _ => _deviceInfo.HwVersion,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false);
        }

        if (_deviceInfo.FieldbusType != null)
        {
            var valueList = new Dictionary<string, string>
            {
                {"0" , "Profinet"},
                {"2" , "EtherNet/IP" },
                {"3" , "EtherCAT" },
                {"4" , "Modbus-TCP"},
                {"5" , "Internet of Things"},
                {"6" , "ASi"},
                {"7" , "POWERLINK"}
            };

            _elementManager.CreateReadOnlyDataElement(
                deviceInfoElement,
                FieldBusTypeIdentifier,
                _ => _deviceInfo.FieldbusType,
                profiles: _parameterProfile,
                cacheTimeout: timeout, acquireLock: false,
                format: new IntegerEnumFormat(new IntegerEnumValuation(valueList)));
        }
    }

    /// <summary>
    /// Creates a device info instance from the given base element.
    /// </summary>
    /// <param name="baseElement">The element with the profile <seealso cref="ProfileName"/>.</param>
    /// <returns>A DeviceInfo instance or null.</returns>
    public static IDeviceInfo FromElement(IBaseElement baseElement)
    {
        if (baseElement.Profiles != null && baseElement.Profiles.Contains(ProfileName))
        {
            var deviceNameElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == DeviceNameIdentifier, false, false);
            var deviceFamilyElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == DeviceFamilyIdentifier, false, false);
            var serialNumberElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == SerialNumberIdentifier, false, false);
            var productNameElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == ProductNameIdentifier, false, false);
            var productCodeElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == ProductCodeIdentifier, false, false);
            var orderNumberElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == OrderNumberIdentifier, false, false);
            var productionDateElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == ProductionDateIdentifier, false, false);
            var hwRevisionElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == HwRevisionIdentifier, false, false);
            var hwVersionElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == HwVersionIdentifier, false, false);
            var swRevisionElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == SwRevisionIdentifier, false, false);
            var swVersionElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == SwVersionIdentifier, false, false);
            var bootloaderRevisionElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == BootLoaderRevisionIdentifier, false, false);
            var vendorElement = (IReadDataElement<string>)baseElement.GetElementByPredicate(x => x.Identifier == VendorIdentifier, false, false);
            var fieldbusTypeElement = (IReadDataElement<IDeviceInfo.FieldbusTypeEnum?>)baseElement.GetElementByPredicate(x => x.Identifier == FieldBusTypeIdentifier, false, false);

            return new DeviceInfo(
                deviceNameElement?.Value,
                deviceFamilyElement?.Value,
                serialNumberElement?.Value,
                productNameElement?.Value,
                productCodeElement?.Value,
                orderNumberElement?.Value,
                productionDateElement?.Value,
                hwRevisionElement?.Value,
                hwVersionElement?.Value,
                swRevisionElement?.Value,
                swVersionElement?.Value,
                bootloaderRevisionElement?.Value,
                vendorElement?.Value,
                fieldbusTypeElement?.Value
            );
        }

        return null;
    }
}