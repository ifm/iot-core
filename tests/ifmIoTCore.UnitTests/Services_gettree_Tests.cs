namespace ifmIoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;

    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_gettree_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_adr_level_together_workAsExpected()
        { // this test assumes basic gettree functionality works
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            IBaseElement nextRoot = ioTCore.Root;
            for (var i = 0; i < 20; i++)
            {
                var child = ioTCore.CreateStructureElement(nextRoot, string.Format("struct{0}", i));
                nextRoot = child;
            }
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree", JToken.Parse("{'adr':'/struct0/struct1/struct2/struct3/struct4/struct5/struct6/struct7/struct8/struct9/struct10', 'level':5}"));
            Assert.AreEqual(ResponseCodes.Success, gettreeResponse.Code); Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.type")?.ToObject<string>(), "structure");
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.identifier")?.ToObject<string>(), "struct10");
            Assert.IsNotNull(gettreeResponse.Data.SelectToken("$..[?(@.identifier==" + string.Format("'struct{0}'",10+5) + ")]"));
            Assert.IsNull(gettreeResponse.Data.SelectToken("$..[?(@.identifier==" + string.Format("'struct{0}'",10+5+1) + ")]"));
        }


        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_level_1to100_ListsSubElements()
        { // this test assumes basic gettree functionality works
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            IBaseElement nextRoot = ioTCore.Root;
            for (var i = 1; i <= 100; i++)
            {
                var child = ioTCore.CreateStructureElement(nextRoot, string.Format("struct{0}", i));
                nextRoot = child;
            }
            Assert.Multiple(() =>
            {
                for (var i = 1; i <= 100; i++)
                {
                    var gettreeResponse = ioTCore.HandleRequest(0, "/gettree", 
                        JToken.Parse("{" + string.Format("'level' : {0}",i) + "}"));
                    Assert.AreEqual(ResponseCodes.Success, gettreeResponse.Code); Assert.AreEqual(200, ResponseCodes.Success);
                    Assert.IsNotNull(gettreeResponse.Data.SelectToken("$..[?(@.identifier==" + string.Format("'struct{0}'",i) + ")]"));
                    Assert.IsNull(gettreeResponse.Data.SelectToken("$..[?(@.identifier==" + string.Format("'struct{0}'",i+1) + ")]"));
                }
            }); 
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        //[Ignore("TODO when query clarified")]
        public void gettree_ExtraParameter_level_negative_Ignored()
        { // this test assumes basic gettree functionality works
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree", JToken.Parse("{'level': -1}"));
            Assert.AreEqual(ResponseCodes.Success, gettreeResponse.Code); Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.identifier")?.ToObject<string>(), "ioTCore");
            Assert.IsNull(gettreeResponse.Data.SelectToken("$.subs"), "Not expecting subs and its contents");
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        //[Ignore("TODO when query clarified")]
        public void gettree_ExtraParameter_level_null_givesAllLevels()
        { // this test assumes basic gettree functionality works
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            IBaseElement nextRoot = ioTCore.Root;
            for (var i = 1; i <= 100; i++)
            {
                var child = ioTCore.CreateStructureElement(nextRoot, string.Format("struct{0}", i));
                nextRoot = child;
            }
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree", JToken.Parse("{'level': null}"));
            Assert.AreEqual(ResponseCodes.Success, gettreeResponse.Code); Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.type").ToObject<string>(), "device");
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.identifier").ToObject<string>(), "ioTCore");
            Assert.IsNotNull(gettreeResponse.Data.SelectToken("$.subs"), "Expecting subs and its contents");
            Assert.IsNotNull(gettreeResponse.Data.SelectToken("$..[?(@.identifier==" + string.Format("'struct{0}'",100) + ")]"));
            Assert.IsNull(gettreeResponse.Data.SelectToken("$..[?(@.identifier==" + string.Format("'struct{0}'",100+1) + ")]"));
        }


        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_level_0_NoSubElements()
        { // this test assumes basic gettree functionality works
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree", JToken.Parse("{'level': 0}"));
            Assert.AreEqual(ResponseCodes.Success, gettreeResponse.Code); Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.identifier")?.ToObject<string>(), "ioTCore");
            Assert.IsNull(gettreeResponse.Data.SelectToken("$.subs"), "Not expecting subs and its contents");
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_adr_valid_startsFrom_specifiedElement()
        { // this test assumes basic gettree functionality works
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            IBaseElement nextRoot = ioTCore.Root;
            for (var i = 0; i < 20; i++)
            {
                var child = ioTCore.CreateStructureElement(nextRoot, string.Format("struct{0}", i));
                nextRoot = child;
            }
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree", JToken.Parse("{'adr':'/struct0/struct1/struct2/struct3/struct4/struct5/struct6/struct7/struct8/struct9/struct10'}"));
            Assert.AreEqual(ResponseCodes.Success, gettreeResponse.Code); Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.type")?.ToObject<string>(), "structure");
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.identifier")?.ToObject<string>(), "struct10");
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_adr_null_startsFrom_RootDevice()
        { // this test assumes basic gettree functionality works
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree", JToken.Parse("{'adr':null}"));
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, gettreeResponse.Code);
            Assert.IsNotNull(gettreeResponse.Data.SelectToken("$.type"));
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.type").ToObject<string>(), "device");
            Assert.AreEqual(gettreeResponse.Data.SelectToken("$.identifier").ToObject<string>(), "ioTCore");
        }

        readonly List<string> MandatoryParameters = new List<string> { "identifier", "type" };

        [Test, Property("TestCaseKey", "IOTCS-T89")]
        public void gettree_Response_RootDeviceElement_HasMandatoryParameters()
        { // Note: "subs" parameter is implicitly checked
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            Assert.IsTrue(ParametersFound_RecurseSubs(gettreeResponse, MandatoryParameters));
            Assert.IsTrue(ParametersFound_RecurseSubs( gettreeResponse.SelectToken("$.subs"), ChildElements_MandatoryParameters));
        }

        internal bool ParametersFound_RecurseSubs(JToken elementToken, List<string> childnames )
        { // Note: "subs" element is recursed if available
            if (elementToken is JArray)
            {
                foreach (var childelementToken in elementToken)
                    if (!ParametersFound_RecurseSubs(childelementToken, childnames))
                        return false;
            }
            else if (elementToken is JToken)
            {
                foreach (var name in childnames)
                    if (elementToken.SelectToken("$." + name) == null)
                        return false;
            }
            // recurse to subs element if found
            var subs = elementToken.SelectToken("$.subs");
            if (subs != null)
                foreach (var childToken in subs)
                    if (!ParametersFound_RecurseSubs(childToken, childnames))
                        return false;
            return true;
        }

        static List<string> ChildElements_MandatoryParameters = new List<string> { 
                "identifier","type", "adr"};

        [Test, Property("TestCaseKey", "IOTCS-T51")]
        public void gettree_Response_StructureElement_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , StructureElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var gettreeResponse = ioTCore.HandleRequest(0,"/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "struct1", "structure");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
        }

        [Test, Property("TestCaseKey", "IOTCS-T49")]
        public void gettree_Response_ServiceElement_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateActionServiceElement(ioTCore.Root, "myservice", null);
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "myservice", "service");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
        }

        [Test, Property("TestCaseKey", "IOTCS-T49")]
        public void gettree_Response_ServiceElementFull_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateActionServiceElement(ioTCore.Root, "myserviceFull", null, 
                    new Format(type:"serviceFormat",encoding:"utf-8", valuation:null, ns: null),
                    new List<string>(),
                    "uid_myserviceFull_123");
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "myserviceFull", "service");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
            // Extended checks on specific element type
            Assert.NotNull(SearchElement.SelectToken("$..[?(@.profiles)]"));
            Assert.NotNull(SearchElement.SelectToken("$..[?(@.uid == 'uid_myserviceFull_123')]"));
            Assert.NotNull(SearchElement.SelectToken("$.format..[?(@.type == 'serviceFormat')]"));
            Assert.NotNull(SearchElement.SelectToken("$.format..[?(@.encoding == 'utf-8')]"));
            Assert.NotNull(SearchElement.SelectToken("$.format..[?(@.namespace == 'json')]")); // ns, namespace, defaults to json
        }

        [Test, Property("TestCaseKey", "IOTCS-T44")]
        public void gettree_Response_DataElement_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<object>(ioTCore.Root,
                "mydata_minimal", 
                    format: new IntegerFormat(new IntegerValuation(int.MinValue, int.MaxValue, null, 0)));
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "mydata_minimal", "data");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
            // Extended checks on specific element type
            Assert.NotNull(SearchElement.SelectToken("$.format..[?(@.type == 'number')]"));
            Assert.NotNull(SearchElement.SelectToken("$.format..[?(@.encoding == 'integer')]"));
            Assert.NotNull(SearchElement.SelectToken(string.Format("$.format..[?(@.valuation.min == {0})]",int.MinValue)));
            Assert.NotNull(SearchElement.SelectToken(string.Format("$.format..[?(@.valuation.max == {0})]",int.MaxValue)));
            Assert.NotNull(SearchElement.SelectToken(string.Format("$.format..[?(@.valuation.default == {0})]",0)));
        }

        [Test, Property("TestCaseKey", "IOTCS-T44")]
        public void gettree_Response_DataElementFull_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);

            var intField = new Field("intField1", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField1", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional:true);
            var stringField = new Field("stringField1", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var objectDataFormat = new ObjectFormat(new ObjectValuation(new List<Field> { intField, floatField, stringField }));

            ioTCore.CreateDataElement<object>(ioTCore.Root,
                "mydataFull", 
                    format: objectDataFormat,
                    profiles: new List<string>(),
                    uid: "uid_mydataFull_123");
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "mydataFull", "data");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
            // Extended checks on specific element type
            Assert.NotNull(SearchElement.SelectToken("$..[?(@.profiles)]"));
            Assert.NotNull(SearchElement.SelectToken("$..[?(@.uid == 'uid_mydataFull_123')]"));
            Assert.NotNull(SearchElement.SelectToken("$.format..[?(@.type == 'object')]"));
            Assert.Null(SearchElement.SelectToken("$.format.encoding")); // object type does not have encoding explicit
            Assert.NotNull(SearchElement.SelectToken("$.format.valuation.fields[0]..[?(@.name == 'intField1')]")); 
            Assert.NotNull(SearchElement.SelectToken("$.format.valuation.fields[1]..[?(@.name == 'floatField1')]")); 
            Assert.NotNull(SearchElement.SelectToken("$.format.valuation.fields[2]..[?(@.name == 'stringField1')]")); 
            Assert.NotNull(SearchElement.SelectToken("$.format.valuation.fields[0]..[?(@.format.encoding == 'integer')]")); 
            Assert.NotNull(SearchElement.SelectToken("$.format.valuation.fields[1]..[?(@.optional == true)]")); 
            Assert.NotNull(SearchElement.SelectToken("$.format.valuation.fields[1]..[?(@.format.valuation.decimalplaces == 3)]")); 
            Assert.NotNull(SearchElement.SelectToken("$.format.valuation.fields[2]..[?(@.format.valuation.pattern == 'dd-mm-yyyy')]")); 
        }

        [Test, Property("TestCaseKey", "IOTCS-T50")]
        public void gettree_Response_EventElement_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var myevent = ioTCore.CreateEventElement(ioTCore.Root, "myevent_minimal");
            ioTCore.CreateActionServiceElement(myevent, 
                Identifiers.TriggerEvent,
                (s, cid) => { myevent.RaiseEvent(); });

            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "myevent_minimal", "event");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
            // Extended checks on specific element type
            Assert.IsTrue(ParametersFound_RecurseSubs(SearchElement, ChildElements_MandatoryParameters));
            Assert.NotNull(SearchElement.SelectToken("$.subs..[?(@.identifier == 'subscribe' && @.adr == '/myevent_minimal/subscribe')]"));
            Assert.NotNull(SearchElement.SelectToken("$.subs..[?(@.identifier == 'unsubscribe' && @.adr == '/myevent_minimal/unsubscribe')]"));
            Assert.NotNull(SearchElement.SelectToken("$.subs..[?(@.identifier == 'triggerevent' && @.adr == '/myevent_minimal/triggerevent')]"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T50")]
        public void gettree_Response_EventElementFull_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var myevent = ioTCore.CreateEventElement(ioTCore.Root,
                "myeventFull",
                format: null,
                profiles: new List<string>(),
                uid: "uid_myeventFull_123");
            ioTCore.CreateActionServiceElement(myevent, 
                Identifiers.TriggerEvent,
                (s, cid) => { myevent.RaiseEvent(); });


            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "myeventFull", "event");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
            // Extended checks on specific element type
            Assert.IsTrue(ParametersFound_RecurseSubs(SearchElement, ChildElements_MandatoryParameters));
            Assert.NotNull(SearchElement.SelectToken("$..[?(@.profiles)]"));
            Assert.NotNull(SearchElement.SelectToken("$..[?(@.uid == 'uid_myeventFull_123')]"));
            Assert.NotNull(SearchElement.SelectToken("$.subs..[?(@.identifier == 'subscribe' && @.adr == '/myeventFull/subscribe')]"));
            Assert.NotNull(SearchElement.SelectToken("$.subs..[?(@.identifier == 'unsubscribe' && @.adr == '/myeventFull/unsubscribe')]"));
            Assert.NotNull(SearchElement.SelectToken("$.subs..[?(@.identifier == 'triggerevent' && @.adr == '/myeventFull/triggerevent')]"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T91")]
        public void gettree_Response_HiddenElementIsNotShown_ServiceElement()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateActionServiceElement(ioTCore.Root, "myservice_hidden", null, isHidden: true);
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                "myservice_hidden", "service");
            var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.Null(SearchElement, string.Format("found element unexpectedly {0}", identifier_jpath_query));
        }


        static List<string> DeviceElementSubs_services = new List<string>{ 
            "getidentity",
            "gettree", 
            "querytree",
            "getdatamulti", 
            "setdatamulti",
            "getsubscriberlist",
            //"addelement", "removeelement",
            //"getelementinfo", "setelementinfo",
            //"addprofile", "removeprofile",
            //"mirror", "unmirror",
            };

        static List<string> DeviceElementSubs_events = new List<string>{ 
            "treechanged",
            };

        [Test, Property("TestCaseKey", "IOTCS-T53")]
        public void gettree_Response_DeviceElementSubs_HasStandardElements()
        { // pre-condition: ensure these tests pass: AddChildElement , DeviceElement, IoTCore creation 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var gettreeResponse = ioTCore.HandleRequest(0, "/gettree").Data;
            foreach (var serviceElement in DeviceElementSubs_services)
            { 
                var identifier_jpath_query = string.Format(
                    "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                    serviceElement, "service");
                var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
                Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
            }
            foreach (var eventElement in DeviceElementSubs_events)
            { 
                var identifier_jpath_query = string.Format(
                    "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == '/{0}')]",
                    eventElement, "event");
                var SearchElement = gettreeResponse.SelectTokens(identifier_jpath_query).FirstOrDefault();
                Assert.NotNull(SearchElement, string.Format("Failed to find element {0}", identifier_jpath_query));
            }
            
        }
    }
}
