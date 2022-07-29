namespace ifmIoTCore.Elements.Valuations
{
    /// <summary>
    /// Represents the valuation parameters for an array type data element
    /// </summary>
    public class ArrayValuation : Valuation
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="baseType">The base type for the items in the data element</param>
        /// <param name="format">The format for the items in the data element</param>
        /// <param name="defaultValue">The initial value for the items in the data element</param>
        public ArrayValuation(string baseType, Format format, object defaultValue = null) :
            base(null, null, null, null, null, null, null, null, defaultValue, null, baseType, format)
        {
        }
    }
}
