namespace ifm.IoTCore.Profile.DeviceInfo;

/// <summary>
/// Provides functionality to interact with a device info instance
/// For all parameters return null, if it is not supported
/// </summary>
public interface IDeviceInfo
{
    string DeviceName { get; }
    string DeviceFamily { get; }
    string SerialNumber { get; }
    string ProductName { get; }
    string ProductCode { get; }
    string OrderNumber { get; }
    string ProductionDate { get; }
    string HwRevision { get; }
    string HwVersion { get; }
    string SwRevision { get; }
    string SwVersion { get; }
    string BootloaderRevision { get; }
    string Vendor { get; }

    FieldbusTypeEnum? FieldbusType { get; }

    public enum FieldbusTypeEnum
    {
        Profinet = 0,
        Reserved = 1,
        EtherNetIp = 2,
        EtherCat = 3,
        ModbusTcp = 4,
        IoT = 5,
        Asi = 6,
        Powerlink = 7


    }
}