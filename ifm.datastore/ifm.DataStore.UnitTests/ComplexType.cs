namespace ifm.DataStore.UnitTests;

public class ComplexType
{
    public int MyInteger { get; set; } = -652000;
    public uint MyUnsignedInteger { get; set; } = 652000;
    public bool MyBoolean { get; set; } = true;
    public byte MyByte { get; set; } = 255;
    public double MyDouble { get; set; } = 65.200;
    public ComplexType MyComplexType { get; set; } = null;
}