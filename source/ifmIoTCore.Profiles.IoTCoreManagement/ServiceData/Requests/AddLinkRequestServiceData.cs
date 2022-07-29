namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using Common.Variant;

    public class AddLinkRequestServiceData
    {
        [VariantProperty("adr", Required = true)]
        public string SourceAddress { get; set; }

        [VariantProperty("target_adr", Required = true)]
        public string TargetAddress { get; set; }

        [VariantProperty("identifier", Required = false)]
        public string Identifier { get; set; }

        [VariantProperty("persist", Required = false)]
        public bool Persist { get; set; }

        [VariantConstructor]
        public AddLinkRequestServiceData()
        {
        }

        public AddLinkRequestServiceData(string identifier, string sourceAddress, string targetAddress, bool persist = false)
        {
            Identifier = identifier;
            SourceAddress = sourceAddress;
            TargetAddress = targetAddress;
            Persist = persist;
        }
    }
}
