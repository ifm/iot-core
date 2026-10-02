namespace ifm.IoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.Valuations;
    using ifm.Common.Variant;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_gettree_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T44")]
        public void gettree_expands_const_value_profile_DataElements()
        { // this test assumes basic gettree functionality works
            // Given: iot core instance, 2 dataelements, one of them with const_value profile, with specific value
            var testiotcore = Helpers.CreateIoTCore("ioTCore");
            const int AnswerOfEverything = 42;
            var dataelement_const_value_profile = System.Guid.NewGuid().ToString("N");
            testiotcore.ElementManager.CreateSimpleDataElement(
                testiotcore.Root, 
                dataelement_const_value_profile, 
                value: AnswerOfEverything, 
                profiles: new List<string> {"const_value"});

            var dataelement_const_value_not = System.Guid.NewGuid().ToString("N");
            testiotcore.ElementManager.CreateSimpleDataElement(testiotcore.Root, dataelement_const_value_not, value: AnswerOfEverything + 1, profiles: new List<string> { "const_value_not" });

            // When: gettree service is called
            var gettreeResponse = testiotcore.MessageDispatcher.HandleRequest(0, "/gettree", data: new VariantObject
            {
                {
                    "expand_const_values", new VariantValue(true)
                }
            });

            // Then: gettree json has only one element having 'value' field
            Assert.That(gettreeResponse.Data.ToJToken().SelectTokens("$..value").Count(), Is.EqualTo(1), "expected search to return exactly single value element");
            // Then: the specific dataelement having const_value has the specific value
            var query_elementHavingValue = $"$..[?(@.identifier == '{dataelement_const_value_profile}')]";
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken(query_elementHavingValue)?.Value<int>("value"), Is.EqualTo(AnswerOfEverything));
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_adr_level_together_workAsExpected()
        { // this test assumes basic gettree functionality works
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var nextRoot = ioTCore.Root;
            for (var i = 0; i < 20; i++)
            {
                nextRoot = ioTCore.ElementManager.CreateStructureElement(nextRoot, $"struct{i}");
            }

            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", new VariantObject { { "adr", new VariantValue("/struct0/struct1/struct2/struct3/struct4/struct5/struct6/struct7/struct8/struct9/struct10") }, { "level", new VariantValue(5) }, });
            Assert.That(ResponseCodes.Success, Is.EqualTo(gettreeResponse.Code));
            Assert.That(ResponseCodes.Success, Is.EqualTo(200));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.type")?.ToObject<string>(),Is.EqualTo("structure"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.identifier")?.ToObject<string>(),Is.EqualTo("struct10"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$..[?(@.identifier==" + $"'struct{10 + 5}'" + ")]"), Is.Not.Null);
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$..[?(@.identifier==" + $"'struct{10 + 5 + 1}'" + ")]"), Is.Null);
        }


        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_level_1to100_ListsSubElements()
        { // this test assumes basic gettree functionality works
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var nextRoot = ioTCore.Root;
            for (var i = 1; i <= 100; i++)
            {
                nextRoot = ioTCore.ElementManager.CreateStructureElement(nextRoot, $"struct{i}");
            }
            Assert.Multiple(() =>
            {
                for (var i = 1; i <= 100; i++)
                {
                    var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", 
                        VariantConverter.FromJToken(JToken.Parse("{" + $"'level' : {i}" + "}")));
                    Assert.That(gettreeResponse.Code, Is.EqualTo(ResponseCodes.Success));
                    Assert.That(ResponseCodes.Success, Is.EqualTo(200));
                    Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$..[?(@.identifier==" + $"'struct{i}'" + ")]"), Is.Not.Null);
                    Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$..[?(@.identifier==" +
                                                                            $"'struct{i + 1}'" + ")]"), Is.Null);
                }
            }); 
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        //[Ignore("TODO when query clarified")]
        public void gettree_ExtraParameter_level_negative_Ignored()
        { // this test assumes basic gettree functionality works
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", VariantConverter.FromJToken(JToken.Parse("{'level': 0}")));
            Assert.That(gettreeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.That(ResponseCodes.Success, Is.EqualTo(200));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.identifier")?.ToObject<string>(), Is.EqualTo("ioTCore"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.subs"), Is.Null, "Not expecting subs and its contents");
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        //[Ignore("TODO when query clarified")]
        public void gettree_ExtraParameter_level_null_givesAllLevels()
        { // this test assumes basic gettree functionality works
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var nextRoot = ioTCore.Root;
            for (var i = 1; i <= 100; i++)
            {
                nextRoot = ioTCore.ElementManager.CreateStructureElement(nextRoot, $"struct{i}");
            }
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", VariantConverter.FromJToken(JToken.Parse("{'level': null}")));
            Assert.That(gettreeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.That(ResponseCodes.Success, Is.EqualTo(200));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.type").ToObject<string>(), Is.EqualTo("device"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.identifier").ToObject<string>(), Is.EqualTo("ioTCore"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.subs"), Is.Not.Null, "Expecting subs and its contents");
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$..[?(@.identifier==" + $"'struct{100}'" + ")]"), Is.Not.Null);
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$..[?(@.identifier==" + $"'struct{100 + 1}'" + ")]"), Is.Null);
        }


        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_level_0_NoSubElements()
        { // this test assumes basic gettree functionality works
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", VariantConverter.FromJToken(JToken.Parse("{'level': 0}")));
            Assert.That(gettreeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.That(ResponseCodes.Success, Is.EqualTo(200));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.identifier")?.ToObject<string>(), Is.EqualTo("ioTCore"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.subs"), Is.Null, "Not expecting subs and its contents");
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_adr_valid_startsFrom_specifiedElement()
        { // this test assumes basic gettree functionality works
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var nextRoot = ioTCore.Root;
            for (var i = 0; i < 20; i++)
            {
                nextRoot = ioTCore.ElementManager.CreateStructureElement(nextRoot, $"struct{i}");
            }
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", VariantConverter.FromJToken(JToken.Parse("{'adr':'/struct0/struct1/struct2/struct3/struct4/struct5/struct6/struct7/struct8/struct9/struct10'}")));
            Assert.That(gettreeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.That(ResponseCodes.Success, Is.EqualTo(200));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.type")?.ToObject<string>(),Is.EqualTo( "structure"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.identifier")?.ToObject<string>(), Is.EqualTo("struct10"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T90")]
        public void gettree_ExtraParameter_adr_null_startsFrom_RootDevice()
        { // this test assumes basic gettree functionality works
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", VariantConverter.FromJToken(JToken.Parse("{'adr':null}")));
            Assert.That(ResponseCodes.Success, Is.EqualTo(200));
            Assert.That(gettreeResponse.Code, Is.EqualTo(ResponseCodes.Success));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.type"), Is.Not.Null);
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.type").ToObject<string>(), Is.EqualTo("device"));
            Assert.That(gettreeResponse.Data.ToJToken().SelectToken("$.identifier").ToObject<string>(), Is.EqualTo("ioTCore"));
        }

        readonly List<string> MandatoryParameters = new List<string> { "identifier", "type" };

        [Test, Property("TestCaseKey", "IOTCS-T89")]
        public void gettree_Response_RootDeviceElement_HasMandatoryParameters()
        { // Note: "subs" parameter is implicitly checked
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            Assert.That(ParametersFound_RecurseSubs(VariantConverter.ToJToken(gettreeResponse), MandatoryParameters));
            Assert.That(ParametersFound_RecurseSubs( gettreeResponse.ToJToken().SelectToken("$.subs"), ChildElements_MandatoryParameters));
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
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0,"/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "struct1", "structure");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement,Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
        }

        [Test, Property("TestCaseKey", "IOTCS-T49")]
        public void gettree_Response_ServiceElement_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateActionServiceElement(ioTCore.Root, "myservice", (_, _) => { });
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "myservice", "service");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
        }

        [Test, Property("TestCaseKey", "IOTCS-T49")]
        public void gettree_Response_ServiceElementFull_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateActionServiceElement(ioTCore.Root, "myserviceFull", (_, _) => { }, new StringFormat(), new List<string>(), "uid_myserviceFull_123");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree", new VariantObject {{"expand_service_format", new VariantValue(true)}}).Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "myserviceFull", "service");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
            // Extended checks on specific element type
            Assert.That(SearchElement.SelectToken("$..[?(@.profiles)]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$..[?(@.uid == 'uid_myserviceFull_123')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.format..[?(@.type == 'string')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.format..[?(@.encoding == 'utf-8')]"),Is.Not.Null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T44")]
        public void gettree_Response_DataElement_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement<object>(ioTCore.Root, "mydata_minimal", format: new Int32Format(new Int32Valuation(int.MinValue, int.MaxValue, 0)));
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "mydata_minimal", "data");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
            // Extended checks on specific element type
            Assert.That(SearchElement.SelectToken("$.format..[?(@.type == 'number')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.format..[?(@.encoding == 'int32')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken($"$.format..[?(@.valuation.min == {int.MinValue})]"), Is.Not.Null);
            Assert.That(SearchElement.SelectToken($"$.format..[?(@.valuation.max == {int.MaxValue})]"), Is.Not.Null);
            Assert.That(SearchElement.SelectToken($"$.format..[?(@.valuation.default == {0})]"), Is.Not.Null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T44")]
        public void gettree_Response_DataElementFull_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            var ioTCore = Helpers.CreateIoTCore("ioTCore");

            var intField = new ObjectValuation.Field("intField1", new Int32Format(new Int32Valuation(-100, 100)));
            var floatField = new ObjectValuation.Field("floatField1", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional:true);
            var stringField = new ObjectValuation.Field("stringField1", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var objectDataFormat = new ObjectFormat(new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField }));

            ioTCore.ElementManager.CreateSimpleDataElement<object>(ioTCore.Root, "mydataFull", format: objectDataFormat, profiles: new List<string>(), uid: "uid_mydataFull_123");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "mydataFull", "data");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
            // Extended checks on specific element type
            Assert.That(SearchElement.SelectToken("$..[?(@.profiles)]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$..[?(@.uid == 'uid_mydataFull_123')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.format..[?(@.type == 'object')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.format.encoding"),Is.Null); // object type does not have encoding explicit
            Assert.That(SearchElement.SelectToken("$.format.valuation.fields[0]..[?(@.name == 'intField1')]"),Is.Not.Null); 
            Assert.That(SearchElement.SelectToken("$.format.valuation.fields[1]..[?(@.name == 'floatField1')]"),Is.Not.Null); 
            Assert.That(SearchElement.SelectToken("$.format.valuation.fields[2]..[?(@.name == 'stringField1')]"),Is.Not.Null); 
            Assert.That(SearchElement.SelectToken("$.format.valuation.fields[0]..[?(@.format.encoding == 'int32')]"),Is.Not.Null); 
            Assert.That(SearchElement.SelectToken("$.format.valuation.fields[1]..[?(@.optional == true)]"),Is.Not.Null); 
            Assert.That(SearchElement.SelectToken("$.format.valuation.fields[1]..[?(@.format.valuation.decimalplaces == 3)]"),Is.Not.Null); 
            Assert.That(SearchElement.SelectToken("$.format.valuation.fields[2]..[?(@.format.valuation.pattern == 'dd-mm-yyyy')]"),Is.Not.Null); 
        }

        [Test, Property("TestCaseKey", "IOTCS-T50")]
        public void gettree_Response_EventElement_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent_minimal"); 
            ioTCore.ElementManager.CreateActionServiceElement(myevent, Identifiers.TriggerEvent, (s, cid) => { myevent.Raise(); });

            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "myevent_minimal", "event");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
            // Extended checks on specific element type
            Assert.That(ParametersFound_RecurseSubs(SearchElement, ChildElements_MandatoryParameters));
            Assert.That(SearchElement.SelectToken("$.subs..[?(@.identifier == 'subscribe' && @.adr == 'ioTCore/myevent_minimal/subscribe')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.subs..[?(@.identifier == 'unsubscribe' && @.adr == 'ioTCore/myevent_minimal/unsubscribe')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.subs..[?(@.identifier == 'triggerevent' && @.adr == 'ioTCore/myevent_minimal/triggerevent')]"),Is.Not.Null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T50")]
        public void gettree_Response_EventElementFull_HasRequiredMembers()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myeventFull", format: null,
                profiles: new List<string>(),
                uid: "uid_myeventFull_123");
            ioTCore.ElementManager.CreateActionServiceElement(myevent, Identifiers.TriggerEvent, (s, cid) => { myevent.Raise(); });


            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "myeventFull", "event");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
            // Extended checks on specific element type
            Assert.That(ParametersFound_RecurseSubs(SearchElement, ChildElements_MandatoryParameters));
            Assert.That(SearchElement.SelectToken("$..[?(@.profiles)]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$..[?(@.uid == 'uid_myeventFull_123')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.subs..[?(@.identifier == 'subscribe' && @.adr == 'ioTCore/myeventFull/subscribe')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.subs..[?(@.identifier == 'unsubscribe' && @.adr == 'ioTCore/myeventFull/unsubscribe')]"),Is.Not.Null);
            Assert.That(SearchElement.SelectToken("$.subs..[?(@.identifier == 'triggerevent' && @.adr == 'ioTCore/myeventFull/triggerevent')]"),Is.Not.Null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T91")]
        public void gettree_Response_HiddenElementIsNotShown_ServiceElement()
        { // pre-condition: ensure these tests pass: AddChildElement , ServiceElement creation 
            // like structure element, all child elements of root device have these parameters: identifier, type and adr
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateActionServiceElement(ioTCore.Root, "myservice_hidden", (_,_) => { }, isHidden: true);
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            var identifier_jpath_query = string.Format(
                "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                "myservice_hidden", "service");
            var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
            Assert.That(SearchElement, Is.Null, $"found element unexpectedly {identifier_jpath_query}");
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
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var gettreeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/gettree").Data;
            foreach (var serviceElement in DeviceElementSubs_services)
            { 
                var identifier_jpath_query = string.Format(
                    "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                    serviceElement, "service");
                var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
                Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
            }
            foreach (var eventElement in DeviceElementSubs_events)
            { 
                var identifier_jpath_query = string.Format(
                    "$.subs[?(@.identifier == '{0}' && @.type == '{1}' && @.adr == 'ioTCore/{0}')]",
                    eventElement, "event");
                var SearchElement = gettreeResponse.ToJToken().SelectTokens(identifier_jpath_query).FirstOrDefault();
                Assert.That(SearchElement, Is.Not.Null, $"Failed to find element {identifier_jpath_query}");
            }
            
        }
    }
}
