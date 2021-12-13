namespace ifmIoTCore.Persistence
{
    using System;
    using Newtonsoft.Json.Linq;

    public class ServiceInvocation : IEquatable<ServiceInvocation>
    {
        public string ServiceAddress { get; }
        public JToken ServiceData { get; }
        
        public ServiceInvocation(string serviceAddress, JToken serviceData)
        {
            ServiceAddress = serviceAddress;
            ServiceData = serviceData;
        }

        public bool Equals(ServiceInvocation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ServiceAddress == other.ServiceAddress && Equals(ServiceData, other.ServiceData);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceInvocation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ServiceAddress != null ? ServiceAddress.GetHashCode() : 0) * 397) ^ (ServiceData != null ? ServiceData.GetHashCode() : 0);
            }
        }
    }
}