namespace ifmIoTCore.Elements.Formats
{
    using Valuations;

    /// <summary>
    /// Represents the format of a string type data element
    /// </summary>
    public class StringFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="valuation">The format valuation</param>
        /// <param name="encoding">The format encoding</param>
        /// <param name="ns">The format namespace</param>
        public StringFormat(StringValuation valuation, string encoding = Encodings.Utf8, string ns = null) : base(Types.String, encoding, valuation, ns)
        {
        }
    }
}
