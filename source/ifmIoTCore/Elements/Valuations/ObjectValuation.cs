namespace ifmIoTCore.Elements.Valuations
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the valuation parameters for an object type data element
    /// </summary>
    public class ObjectValuation : Valuation
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="fields">The list of fields in the object type data element</param>
        public ObjectValuation(List<Field> fields) : 
            base(null, null, null, null, null, null, null, null, null, fields, null, null)
        {
        }
    }
}
