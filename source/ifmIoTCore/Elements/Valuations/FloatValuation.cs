namespace ifmIoTCore.Elements.Valuations
{
    /// <summary>
    /// Represents the valuation parameters for an float type data element
    /// </summary>
    public class FloatValuation : Valuation
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="defaultValue">The initial value for the item in the data element</param>
        public FloatValuation(float defaultValue) :
            base(null, null, null, null, null, null, null, null, defaultValue, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="min">The minimum value for the item in the data element</param>
        /// <param name="max">The maximum value for the item in the data element</param>
        /// <param name="decimalPlaces">The number of decimal places</param>
        /// <param name="defaultValue">The initial value for the item in the data element</param>
        public FloatValuation(float? min, float? max, int? decimalPlaces, float? defaultValue = null) : 
            base(min, max, null, decimalPlaces, null, null, null, null, defaultValue, null, null, null)
        {
        }
    }
}