namespace ifmIoTCore.Profiles.Base
{
    using System.Xml.Serialization;

    public class ProfileBuilderConfiguration
    {
        [XmlIgnore]
        public IIoTCore IoTCore { get;  set; }

        
        public string Address { get; set; }


        public string SettingsName { get; set; }//Only for launcher configuration nessesary!



        public ProfileBuilderConfiguration()
        {
            
        }
        
        public ProfileBuilderConfiguration(IIoTCore ioTCore, string address)
        {
            Address = address;
            IoTCore = ioTCore;
            
        }

    }




}
