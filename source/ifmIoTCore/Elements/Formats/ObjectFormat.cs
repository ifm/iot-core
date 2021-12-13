namespace ifmIoTCore.Elements.Formats
{
    using Valuations;

    /// <summary>
    /// Represents the format of an object type data element
    /// </summary>
    public class ObjectFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="valuation">The format valuation</param>
        /// <param name="ns">The format namespace</param>
        public ObjectFormat(ObjectValuation valuation, string ns = null) : base(Types.Object, null, valuation, ns)
        {
        }
    }
}
