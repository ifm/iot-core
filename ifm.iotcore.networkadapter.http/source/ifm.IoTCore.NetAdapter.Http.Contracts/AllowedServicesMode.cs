namespace ifm.IoTCore.NetAdapter.Http.Contracts
{
    public enum AllowedServicesMode
    {
        /// <summary>
        /// All service calls allowed.
        /// </summary>
        All =  0,

        /// <summary>
        /// Only calls to getidentity service allowed.
        /// </summary>
        GetIdentityOnly = 1
    }
}
