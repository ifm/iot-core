namespace ifmIoTCore.Elements.Formats
{
    using Valuations;

    /// <summary>
    /// Represents the format of an array type data element
    /// </summary>
    public class ArrayFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="valuation">The format valuation</param>
        /// <param name="ns">The format namespace</param>
        public ArrayFormat(ArrayValuation valuation, string ns = null) : base(Types.Array, null, valuation, ns)
        {
        }
    }
}
