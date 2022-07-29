namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using System.Collections.Generic;
    using Common.Variant;
    using Elements;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.AddElement service call
    /// </summary>
    public class AddElementRequestServiceData
    {
        /// <summary>
        /// The address of the parent element to which the element is added
        /// </summary>
        [VariantProperty("adr", Required = true)]
        public string Address { get; set; }
        
        /// <summary>
        /// The type of the element
        /// </summary>
        [VariantProperty("type", Required = true)]
        public string Type { get; set; }

        /// <summary>
        /// The identifier of the element
        /// </summary>
        [VariantProperty("identifier", Required = true)]
        public string Identifier { get; set; }

        /// <summary>
        /// The format of the element
        /// </summary>
        [VariantProperty("format", Required = false)]
        public Format Format { get; set; }

        /// <summary>
        /// The unique identifier of the element
        /// </summary>
        [VariantProperty("uid", Required = false)]
        public string UId { get; set; }

        /// <summary>
        /// The list of profiles the element belongs to
        /// </summary>
        [VariantProperty("profiles", Required = false)]
        public List<string> Profiles { get; set; }

        /// <summary>
        /// Adds a datachanged event element if true and the created element is a data element; otherwise not
        /// </summary>
        [VariantProperty("adddatachanged", Required = false)]
        public bool AddDataChanged { get; set; }

        [VariantConstructor]
        public AddElementRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the parent element to which the element is added</param>
        /// <param name="type">The type of the element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="format">The format of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="profiles">The list of profiles the element belongs to</param>
        public AddElementRequestServiceData(string address, string type, string identifier, Format format = null, string uid = null, List<string> profiles = null, bool addDataChanged = false)
        {
            Address = address;
            Type = type;
            Identifier = identifier;
            Format = format;
            UId = uid;
            Profiles = profiles;
            AddDataChanged = addDataChanged;
        }
    }
}
