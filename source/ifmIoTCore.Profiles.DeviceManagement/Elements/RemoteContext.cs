namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using System;
    using Messages;

    public class RemoteContext
    {
        public Uri Uri { get; }
        public string Address { get; }
        public AuthenticationInfo AuthenticationInfo { get; }
        public string Callback { get; }


        public RemoteContext(Uri uri, string address, AuthenticationInfo authenticationInfo, string callback)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (address == null) throw new ArgumentNullException(nameof(address));

            Uri = uri;
            Address = address;
            AuthenticationInfo = authenticationInfo;
            Callback = callback;
        }
    }
}
