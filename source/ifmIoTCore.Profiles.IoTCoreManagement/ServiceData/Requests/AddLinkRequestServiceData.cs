namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using Newtonsoft.Json;

    public class AddLinkRequestServiceData
    {
        [JsonProperty("identifier", Required = Required.Always)]
        public readonly string Identifier;

        [JsonProperty("adr", Required = Required.Always)]
        public readonly string SourceAddress;

        [JsonProperty("target_adr", Required = Required.Always)]
        public readonly string TargetAddress;

        [JsonProperty("persist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool Persist;

        public AddLinkRequestServiceData(string identifier, string sourceAddress, string targetAddress, bool persist = false)
        {
            Identifier = identifier;
            SourceAddress = sourceAddress;
            TargetAddress = targetAddress;
            Persist = persist;
        }
    }
}
