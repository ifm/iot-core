namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Formats;
    using Newtonsoft.Json.Linq;

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
            [JsonProperty("type", Required = Required.Always)]
            public string Type { get; set; }

            /// <summary>
            /// The identifier of the element
            /// </summary>
            [JsonProperty("identifier", Required = Required.Always)]
            public string Identifier { get; set; }

            /// <summary>
            /// The address of the element
            /// </summary>
            [JsonProperty("adr", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Address { get; set; }

            /// <summary>
            /// The format of the element
            /// </summary>
            [JsonProperty("format", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public Format Format { get; set; }

            /// <summary>
            /// The unique identifier of the element
            /// </summary>
            [JsonProperty("uid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string UId { get; set; }

            /// <summary>
            /// The list of profiles the element belongs to
            /// </summary>
            [JsonProperty("profiles", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Profiles { get; set; }

            /// <summary>
            /// The list of child elements of the element
            /// </summary>
            [JsonProperty("subs", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public List<Element> Subs { get; set; }

            /// <summary>
            /// The value, if the element is of type data and has the profile "const_value"
            /// </summary>
            [JsonProperty("value", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public JToken Value { get; set; }

            /// <summary>
            /// If the element is a linked element the original address of elements
            /// </summary>
            [JsonProperty("link", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Link { get; set; }

            /// <summary>
            /// Returns a string that represents the current object
            /// </summary>
            /// <returns>A string that represents the current object</returns>
            public override string ToString()
            {
                return $"'{Type}' : '{Identifier}'";
            }
        }

        /// <summary>
        /// The type of the root element in the tree
        /// </summary>
        [JsonProperty("type", Required = Required.Always, Order=1)]
        public readonly string Type;

        /// <summary>
        /// The identifier of the root element in the tree
        /// </summary>
        [JsonProperty("identifier", Required = Required.Always, Order = 2)]
        public readonly string Identifier;

        /// <summary>
        /// The address of the root element in the tree
        /// </summary>
        [JsonProperty("adr", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string Address { get; set; }

        /// <summary>
        /// The list of profiles of the root element
        /// </summary>
        [JsonProperty("profiles", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore, Order = 4)]
        public readonly List<string> Profiles;

        /// <summary>
        /// The value, if the element is of type data and has the profile "const_value"
        /// </summary>
        [JsonProperty("value", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore, Order = 5)]
        public JToken Value { get; set; }

        /// <summary>
        /// The list of child elements in the tree
        /// </summary>
        [JsonProperty("subs", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public readonly List<Element> Subs;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public GetTreeResponseServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="type">The type of the root element in the tree</param>
        /// <param name="identifier">The identifier of the root element in the tree</param>
        /// <param name="address">The address of the root element in the tree</param>
        /// <param name="refs">The list of child elements in the tree</param>
        /// <param name="profiles">The list of profiles of the root element.</param>
        /// <param name="maxLevel">The number of levels to get from the tree. </param>
        /// <param name="expandConstValues">If true, the data elements with profile "const_value" should show their values in "value" property.</param>
        /// <param name="value">The value to use if expand const values is set to true</param>
        public GetTreeResponseServiceData(string type, string identifier, string address, IEnumerable<ElementReference> refs, IEnumerable<string> profiles, int? maxLevel = null, bool expandConstValues = false, JToken value = null)
        {
            Type = type;
            Identifier = identifier;
            Address = address;
            Profiles = profiles?.ToList();
            Value = value;

            if (refs == null) return;
            if (maxLevel == null || maxLevel > 0)
            {
                Subs = new List<Element>();
                foreach (var item in refs)
                {
                    if (!item.TargetElement.IsHidden)
                    {
                        Subs.Add(CreateElement(item, 1, maxLevel, expandConstValues));
                    }
                }
            }
        }

        private static Element CreateElement(ElementReference elementReference, int currentLevel, int? maxLevel, bool expandConstValues)
        {
            if (!elementReference.IsForward) return null;

            JToken value = null;
            if (expandConstValues && elementReference.TargetElement is IDataElement dataElement && elementReference.TargetElement.HasProfile("const_value"))
            {
                try
                {
                    value = dataElement.Value;
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

            if (elementReference.Type == ReferenceType.Link)
            {
                if (responseElement.Profiles != null)
                {
                    responseElement.Profiles.Add(Identifiers.Link);
                }
                else
                {
                    responseElement.Profiles = new List<string> {Identifiers.Link};
                }
            }

            if (maxLevel == null || currentLevel < maxLevel)
            {
                if (elementReference.TargetElement.ForwardReferences != null)
                {
                    responseElement.Subs = new List<Element>();
                    foreach (var item in elementReference.TargetElement.ForwardReferences)
                    {
                        responseElement.Subs.Add(CreateElement(item, currentLevel + 1, maxLevel, expandConstValues));
                    }
                }
            }
            return responseElement;
        }
    }
}
