namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;
    using Formats;

    internal class StructureElement : BaseElement, IStructureElement
    {
        public StructureElement(IBaseElement parent,
            string identifier,
            Format format = null, 
            List<string> profiles = null, 
            string uid = null,
            bool isHidden = false,
            object context = null) : 
            base(parent, Identifiers.Structure, identifier, format, profiles, uid, isHidden, context)
        {
        }
    }
}