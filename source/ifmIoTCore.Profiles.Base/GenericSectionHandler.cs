namespace ifmIoTCore.Profiles.Base
{
    using System.Configuration;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class GenericSectionHandler<T> : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            using MemoryStream stm = new MemoryStream();
            using StreamWriter stw = new StreamWriter(stm);
            stw.Write(section.OuterXml);
            stw.Flush();
            stm.Position = 0;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            return xmlSerializer.Deserialize(stm);
        }
    }
}
