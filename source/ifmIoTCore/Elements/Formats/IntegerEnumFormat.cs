namespace ifmIoTCore.Elements.Formats
{
    using Valuations;

    /// <summary>
    /// Represents the format of an integer enumeration type data element
    /// </summary>
    public class IntegerEnumFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="valuation">The format valuation</param>
        /// <param name="ns">The format namespace</param>
        public IntegerEnumFormat(IntegerEnumValuation valuation, string ns = null) : base(Types.Enum, Encodings.Integer, valuation, ns)
        {
        }
    }
}
