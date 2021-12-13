namespace ifmIoTCore.Elements.Valuations
{
    /// <summary>
    /// Represents the valuation parameters for an integer type data element
    /// </summary>
    public class IntegerValuation : Valuation
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="defaultValue">The initial value for the item in the data element</param>
        public IntegerValuation(int defaultValue) :
            base(null, null, null, null, null, null, null, null, defaultValue, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="min">The minimum value for the item in the data element</param>
        /// <param name="max">The maximum value for the item in the data element</param>
        /// <param name="off">The value to turn off the item in the data element</param>
        /// <param name="defaultValue">The initial value for the item in the data element</param>
        public IntegerValuation(int? min, int? max, int? off = null, int? defaultValue = null) :
            base(min, max, off, null, null, null, null, null, defaultValue, null, null, null)
        {
        }
    }
}