namespace ifmIoTCore.Elements.Formats
{
    /// <summary>
    /// Represents the format of a boolean type data element
    /// </summary>
    public class BooleanFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="ns">The format namespace</param>
        public BooleanFormat(string ns = null) : base(Types.Boolean, null, null, ns)
        {
        }
    }
}
