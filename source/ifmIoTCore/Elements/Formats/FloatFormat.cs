namespace ifmIoTCore.Elements.Formats
{
    using Valuations;

    /// <summary>
    /// Represents the format of a float type data element
    /// </summary>
    public class FloatFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="valuation">The format valuation</param>
        /// <param name="ns">The format namespace</param>
        public FloatFormat(FloatValuation valuation, string ns = null) : base(Types.Number, Encodings.Float, valuation, ns)
        {
        }
    }
}
