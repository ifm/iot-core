namespace ifmIoTCore.Elements
{
    using ServiceData.Requests;
    using ServiceData.Responses;

    /// <summary>
    /// Provides functionality to interact with a device element
    /// </summary>
    public interface IDeviceElement : IBaseElement
    {
        /// <summary>
        /// Gets the identity of the IoTCore
        /// </summary>
        /// <returns>The identity response</returns>
        GetIdentityResponseServiceData GetIdentity();

        /// <summary>
        /// Gets the element tree of the IoTCore
        /// </summary>
        /// <param name="data">The tree element root</param>
        /// <returns>The tree response</returns>
        GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data);

        /// <summary>
        /// Queries a list of tree elements which match a specific filter criteria
        /// </summary>
        /// <param name="data">The filter criteria</param>
        /// <returns>The queried tree elements</returns>
        QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data);

        /// <summary>
        /// Gets multiple data elements
        /// </summary>
        /// <param name="data">The data elements to get</param>
        /// <returns>The data elements that were requested</returns>
        GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data);

        /// <summary>
        /// Sets multiple data elements
        /// </summary>
        /// <param name="data">The data elements to set</param>
        void SetDataMulti(SetDataMultiRequestServiceData data);

        /// <summary>
        /// Gets the subscriber list for multiple event elements
        /// </summary>
        /// <param name="data">The event element to get the subscriptions</param>
        /// <returns>The list of subscriptions</returns>
        GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data);
    }
}
