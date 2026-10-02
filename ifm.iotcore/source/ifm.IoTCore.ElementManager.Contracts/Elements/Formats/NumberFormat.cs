namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

/// <summary>
/// Represents the format of a number type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="type">The format type.</param>
/// <param name="encoding">The format encoding.</param>
/// <param name="ns">The format namespace.</param>
public abstract class NumberFormat(string type, string encoding, string ns) : ValueFormat(type, encoding, ns);