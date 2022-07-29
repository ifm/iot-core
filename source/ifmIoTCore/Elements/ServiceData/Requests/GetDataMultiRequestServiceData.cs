namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using System.Collections.Generic;
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.GetDataMulti service call
    /// </summary>
    public class GetDataMultiRequestServiceData
    {
        /// <summary>
        /// List of addresses of data elements from which the values are requested
        /// </summary>
        [VariantProperty("datatosend", Required = true)]
        public List<string> DataToSend { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetDataMultiRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="dataToSend">List of addresses of data elements from which the values are requested</param>
        public GetDataMultiRequestServiceData(List<string> dataToSend)
        {
            DataToSend = dataToSend;
        }
    }
}
