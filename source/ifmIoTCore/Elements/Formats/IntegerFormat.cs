namespace ifmIoTCore.Elements.Formats
{
    using Valuations;

    /// <summary>
    /// Represents the format of an integer type data element
    /// </summary>
    public class IntegerFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="valuation">The format valuation</param>
        /// <param name="ns">The format namespace</param>
        public IntegerFormat(IntegerValuation valuation, string ns = null) : base(Types.Number, Encodings.Integer, valuation, ns)
        {
        }
    }
}
