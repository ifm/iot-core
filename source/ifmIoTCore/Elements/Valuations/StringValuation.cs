namespace ifmIoTCore.Elements.Valuations
{
    /// <summary>
    /// Represents the valuation parameters for a string type data element
    /// </summary>
    public class StringValuation : Valuation
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="defaultValue">The initial value for the item in the data element</param>
        public StringValuation(string defaultValue) :
            base(null, null, null, null, null, null, null, null, defaultValue, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="minLength">The minimum length for the item in the data element</param>
        /// <param name="maxLength">The maximum length for the item in the data element</param>
        /// <param name="pattern">The evaluation pattern for the item in the data element</param>
        /// <param name="defaultValue">The initial value for the item in the data element</param>
        public StringValuation(int? minLength, int? maxLength, string pattern = null, string defaultValue = null) : 
            base(null, null, null, null, minLength, maxLength, pattern, null, defaultValue, null, null, null)
        {
        }
    }
}