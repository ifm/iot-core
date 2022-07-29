namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;

    public class StructureElement : BaseElement, IStructureElement
    {
        public StructureElement(string identifier,
            Format format = null, 
            List<string> profiles = null, 
            string uid = null,
            bool isHidden = false) : 
            base(Identifiers.Structure, identifier, format, profiles, uid, isHidden)
        {
        }
    }
}