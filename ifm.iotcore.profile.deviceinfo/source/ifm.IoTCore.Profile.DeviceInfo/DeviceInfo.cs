namespace ifm.IoTCore.Profile.DeviceInfo;

internal class DeviceInfo(string deviceName,
    string deviceFamily,
    string serialNumber,
    string productName,
    string productCode,
    string orderNumber,
    string productionDate,
    string hwRevision,
    string hwVersion,
    string swRevision,
    string swVersion,
    string bootloaderRevision,
    string vendor,
    IDeviceInfo.FieldbusTypeEnum? fieldbusType) : IDeviceInfo
{
    public string DeviceName { get; } = deviceName;
    public string DeviceFamily { get; } = deviceFamily;
    public string SerialNumber { get; } = serialNumber;
    public string ProductName { get; } = productName;
    public string ProductCode { get; } = productCode;
    public string OrderNumber { get; } = orderNumber;
    public string ProductionDate { get; } = productionDate;
    public string HwRevision { get; } = hwRevision;
    public string HwVersion { get; } = hwVersion;
    public string SwRevision { get; } = swRevision;
    public string SwVersion { get; } = swVersion;
    public string BootloaderRevision { get; } = bootloaderRevision;
    public string Vendor { get; } = vendor;
    public IDeviceInfo.FieldbusTypeEnum? FieldbusType { get; } = fieldbusType;
}