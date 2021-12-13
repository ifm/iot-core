namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using Newtonsoft.Json;

    public class RemoveLinkRequestServiceData
    {
        [JsonProperty("adr", Required = Required.Always)]
        public readonly string SourceAddress;

        [JsonProperty("target_adr", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string TargetAddress;


        public RemoveLinkRequestServiceData(string sourceAddress, string targetAddress = null)
        {
            SourceAddress = sourceAddress;
            TargetAddress = targetAddress;
        }
    }
}
