namespace ifmIoTCore.Elements.Valuations
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the valuation parameters for an integer enumeration type data element
    /// </summary>
    public class IntegerEnumValuation : Valuation
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="values">The list of values for the item in the data element</param>
        /// <param name="defaultValue">The initial value for the item in the data element</param>
        public IntegerEnumValuation(Dictionary<string, string> values, int? defaultValue = null) : 
            base(null, null, null, null, null, null, null, values, defaultValue, null, null, null)
        {
        }
    }
}