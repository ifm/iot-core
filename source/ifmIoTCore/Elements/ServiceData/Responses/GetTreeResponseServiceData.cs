namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Variant;

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.GetTree service call
    /// </summary>
    public class GetTreeResponseServiceData
    {
        /// <summary>
        /// Represents a tree element for a IDeviceElement.GetTree service call
        /// </summary>
        public class Element
        {
            /// <summary>
            /// The type of the element
            /// </summary>
            [VariantProperty("type", Required = true)]
            public string Type { get; set; }

            /// <summary>
            /// The identifier of the element
            /// </summary>
            [VariantProperty("identifier", Required = true)]
            public string Identifier { get; set; }

            /// <summary>
            /// The address of the element
            /// </summary>
            [VariantProperty("adr", IgnoredIfNull = true)]
            public string Address { get; set; }

            /// <summary>
            /// If the element is a linked element the original address of elements
            /// </summary>
            [VariantProperty("link", IgnoredIfNull = true)]
            public string Link { get; set; }

            /// <summary>
            /// The format of the element
            /// </summary>
            [VariantProperty("format", IgnoredIfNull = true)]
            public Format Format { get; set; }

            /// <summary>
            /// The unique identifier of the element
            /// </summary>
            [VariantProperty("uid", IgnoredIfNull = true)]
            public string UId { get; set; }

            /// <summary>
            /// The list of profiles the element belongs to
            /// </summary>
            [VariantProperty("profiles", IgnoredIfNull = true)]
            public List<string> Profiles { get; set; }

            /// <summary>
            /// The list of child elements of the element
            /// </summary>
            [VariantProperty("subs", IgnoredIfNull = true)]
            public List<Element> Subs { get; set; }

            /// <summary>
            /// The value, if the element is of type data and has the profile "const_value"
            /// </summary>
            [VariantProperty("value", IgnoredIfNull = true)]
            public Variant Value { get; set; }

            /// <summary>
            /// The parameterless constructor for the variant converter
            /// </summary>
            [VariantConstructor]
            public Element()
            {
            }
        }

        /// <summary>
        /// The type of the root element in the tree
        /// </summary>
        [VariantProperty("type", Required = true)]
        public string Type { get; set; }

        /// <summary>
        /// The identifier of the root element in the tree
        /// </summary>
        [VariantProperty("identifier", Required = true)]
        public string Identifier { get; set; }

        /// <summary>
        /// The address of the root element in the tree
        /// </summary>
        [VariantProperty("adr", IgnoredIfNull = true)]
        public string Address { get; set; }

        /// <summary>
        /// The format of the element
        /// </summary>
        [VariantProperty("format", IgnoredIfNull = true)]
        public Format Format { get; set; }

        /// <summary>
        /// The list of profiles of the root element
        /// </summary>
        [VariantProperty("profiles", IgnoredIfNull = true)]
        public List<string> Profiles { get; set; }

        /// <summary>
        /// The unique identifier of the element
        /// </summary>
        [VariantProperty("uid", IgnoredIfNull = true)]
        public string UId { get; set; }

        /// <summary>
        /// The value, if the element is of type data and has the profile "const_value"
        /// </summary>
        [VariantProperty("value", IgnoredIfNull = true)]
        public Variant Value { get; set; }

        /// <summary>
        /// The list of child elements in the tree
        /// </summary>
        [VariantProperty("subs", IgnoredIfNull = true)]
        public List<Element> Subs { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetTreeResponseServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="type">The type of the top element in the tree</param>
        /// <param name="identifier">The identifier of the top element in the tree</param>
        /// <param name="address">The address of the top element in the tree</param>
        /// <param name="format">The format of the top element in the tree</param>
        /// <param name="profiles">The list of profiles of the top element</param>
        /// <param name="subs">The list of child elements in the tree</param>
        /// <param name="maxLevel">The number of levels to get from the tree. </param>
        /// <param name="expandConstValues">If true, the data elements with profile "const_value" should show their values in "value" property.</param>
        /// <param name="value">The value to use if expand const values is set to true</param>
        /// <param name="expandLinks">The value to use if expand const values is set to true</param>
        public GetTreeResponseServiceData(string type, string identifier, string address, Format format, IEnumerable<string> profiles, string uid, IEnumerable<IElementReference> subs, int? maxLevel = null, bool expandConstValues = false, bool expandLinks = false, Variant value = null)
        {
            Type = type;
            Identifier = identifier;
            Address = address;
            Format = format;
            Profiles = profiles?.ToList();
            UId = uid;
            Value = value;

            if (subs == null) return;
            if (maxLevel == null || maxLevel > 0)
            {
                Subs = new List<Element>();
                foreach (var item in subs)
                {
                    if (item.TargetElement.IsHidden) continue;
                    Subs.Add(CreateElement(item, 1, maxLevel, expandConstValues, expandLinks));
                }
            }
        }

        private static Variant GetDataElementValue(IBaseElement element)
        {
            Variant result = null;

            if (element is IDataElement dataElement)
            {
                result = dataElement.Value;
            }

            return result;
        }

        private static Element CreateElement(IElementReference elementReference, int currentLevel, int? maxLevel, bool expandConstValues, bool expandLinks)
        {
            if (!elementReference.IsForward) return null;

            Variant value = null;
            if (expandConstValues && elementReference.TargetElement is IDataElement && elementReference.TargetElement.HasProfile("const_value"))
            {
                try
                {
                    value = GetDataElementValue(elementReference.TargetElement);
                }
                catch
                {
                    value = null;
                }
            }

            var responseElement = new Element
            {
                Type = elementReference.TargetElement.Type,
                Identifier = elementReference.Identifier,
                Address = elementReference.TargetElement.Address,
                UId = elementReference.TargetElement.UId,
                Profiles = elementReference.TargetElement.Profiles?.ToList(),
                Format = elementReference.TargetElement.Format,
                Value = value

            };

            string address = "";

            if (elementReference.Type == ReferenceType.Link)
            {
                responseElement.Link = responseElement.Address;
                address = elementReference.SourceElement.Address + "/" + elementReference.Identifier;
                responseElement.Address = address;

                //if (responseElement.Profiles != null)
                //{
                //    responseElement.Profiles.Add(Identifiers.Link);
                //}
                //else
                //{
                //    responseElement.Profiles = new List<string> {Identifiers.Link};
                //}
            }

            if ((maxLevel == null || currentLevel < maxLevel) && elementReference.TargetElement.ForwardReferences != null)
            {
                responseElement.Subs = new List<Element>();
                foreach (var item in elementReference.TargetElement.ForwardReferences)
                {
                    if (item.TargetElement.IsHidden) continue;

                    if (elementReference.Type == ReferenceType.Link && expandLinks)
                    {
                        responseElement.Subs.Add(CreateLinkElement(item, currentLevel + 1, maxLevel, expandConstValues, address));
                    }
                    else if(elementReference.Type != ReferenceType.Link)
                    {
                        responseElement.Subs.Add(CreateElement(item, currentLevel + 1, maxLevel, expandConstValues,
                            expandLinks));
                    }
                   
                        
                }
            }


            return responseElement;
        }

        private static Element CreateLinkElement(IElementReference elementReference, int currentLevel, int? maxLevel, bool expandConstValues, string address)
        {
            if (!elementReference.IsForward) return null;

            Variant value = null;
            if (expandConstValues && elementReference.TargetElement is IDataElement && elementReference.TargetElement.HasProfile("const_value"))
            {
                try
                {
                    value = GetDataElementValue(elementReference.TargetElement);
                }
                catch
                {
                    value = null;
                }
            }

            address = address + "/" + elementReference.Identifier;
            var responseElement = new Element
            {
                Type = elementReference.TargetElement.Type,
                Identifier = elementReference.Identifier,
                Address = address,
                Link = elementReference.TargetElement.Address,
                UId = elementReference.TargetElement.UId,
                Profiles = elementReference.TargetElement.Profiles?.ToList(),
                Format = elementReference.TargetElement.Format,
                Value = value

            };


            if ((maxLevel == null || currentLevel < maxLevel) && elementReference.TargetElement.ForwardReferences != null)
            {
                responseElement.Subs = new List<Element>();
                foreach (var item in elementReference.TargetElement.ForwardReferences)
                {
                    if (item.TargetElement.IsHidden) continue;
                    responseElement.Subs.Add(CreateLinkElement(item, currentLevel + 1, maxLevel, expandConstValues, address));
                }
            }

            return responseElement;
        }

    }
}
