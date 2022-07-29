namespace ifmIoTCore.Elements
{
    public static class Identifiers
    {
        public const string Root = "/";

        public const string Remote = "remote";

        // Standard element type identifiers
        public const string Device = "device";
        public const string SubDevice = "subdevice";
        public const string Structure = "structure";
        public const string Data = "data";
        public const string Service = "service";
        public const string Event = "event";
        public const string Link = "link";

        // Standard service identifiers
        public const string GetIdentity = "getidentity";
        public const string GetTree = "gettree";
        public const string QueryTree = "querytree";
        public const string GetDataMulti = "getdatamulti";
        public const string SetDataMulti = "setdatamulti";
        public const string GetBlobData = "getblobdata";
        public const string SetBlobData = "setblobdata";
        public const string GetData = "getdata";
        public const string SetData = "setdata";
        public const string Subscribe = "subscribe";
        public const string Unsubscribe = "unsubscribe";
        public const string TriggerEvent = "triggerevent";
        public const string CallBackTrigger = "___callback_trigger___";
        public const string GetSubscriberList = "getsubscriberlist";
        public const string IolReadAcyclic = "iolreadacyclic";
        public const string IolWriteAcyclic = "iolwriteacyclic";
        public const string GetLinks = "getlinks";

        // Standard event identifiers
        public const string TreeChanged = "treechanged";
        public const string DataChanged = "datachanged";

        // Standard service parameters
        public const string Value = "value";
        public const string NewValue = "newvalue";
        public const string Callback = "callback";
        public const string DataToSend = "datatosend";
    }
}