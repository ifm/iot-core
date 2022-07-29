namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.GetTree service call
    /// </summary>
    public class GetTreeRequestServiceData
    {
        /// <summary>
        /// The address of the element that serves as the root element for the requested tree 
        /// </summary>
        [VariantProperty("adr", IgnoredIfNull = true)]
        public string Address { get; set; }

        /// <summary>
        /// The number of tree levels that are requested\n
        /// 0 = no subelements, 1 = 1 level of subelements, 2 = 2 levels of subelements, ...\n
        /// Maximum level = 20
        /// </summary>
        [VariantProperty("level", IgnoredIfNull = true)]
        public int? Level { get; set; }

        /// <summary>
        /// Determines that values of data elements which have a "const_value" profile should be included in the gettree response.
        /// The value will be on the same level as identifier and will have a property name of "value".
        /// </summary>
        [VariantProperty("expand_const_values", IgnoredIfNull = true)]
        public bool ExpandConstValues { get; set; }

        /// <summary>
        /// Expand linked elements
        /// </summary>
        [VariantProperty("expand_links", IgnoredIfNull = true)]
        public bool ExpandLinks { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetTreeRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the element that serves as the root element for the requested tree</param>
        /// <param name="level">The number of tree levels that are requested</param>
        /// <param name="expandConstValues">When true the const values will be included in the tree, otherwise when false then no value will be present.</param>
        /// <param name="expandLinks">If true, expand linked elements</param>
        public GetTreeRequestServiceData(string address, int? level, bool expandConstValues = false, bool expandLinks = false)
        {
            Address = address;
            Level = level;
            ExpandConstValues = expandConstValues;
            ExpandLinks = expandLinks;
        }
    }
}
