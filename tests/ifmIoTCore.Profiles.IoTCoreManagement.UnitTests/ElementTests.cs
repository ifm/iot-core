namespace ifmIoTCore.Profiles.Element.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Converter.Json;
    using Elements;
    using Elements.Formats;
    using Elements.ServiceData;
    using Messages;
    using NUnit.Framework;
    using IoTCoreManagement;
    using IoTCoreManagement.ServiceData.Requests;
    using IoTCoreManagement.ServiceData.Responses;
    using Utilities;

    [TestFixture]
    public class ElementTests
    {
        private IIoTCore _iotCore;
        private IoTCoreManagementProfileBuilder _profileBuilder;
        private readonly ifmIoTCore.Converter.Json.JsonConverter _converter = new JsonConverter();

        [SetUp]
        public void Setup()
        {
            this._iotCore = IoTCoreFactory.Create("id0", new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));
            this._profileBuilder = new IoTCoreManagementProfileBuilder(this._iotCore);
            this._profileBuilder.Build();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void AddElement_Message_Success()
        {
            // Act
            var response = _iotCore.HandleRequest(new RequestMessage(1, $"/iotcore_management/addelement",
                Helpers.ToJson(new AddElementRequestServiceData("/", Identifiers.Structure, "struct1",
                    new Format("f1", null, null, null), "uid", new List<string> { "profile1" }))));

            // Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            var element = _iotCore.Root.GetElementByAddress("/struct1");
            Assert.True(element != null);
            Assert.True(element.Type == Identifiers.Structure);
            Assert.True(element.Identifier == "struct1");
            Assert.True(element.Format != null);
            Assert.True(element.Format.Type == "f1");
            Assert.True(element.UId == "uid");
            Assert.True(element.Profiles != null);
            Assert.That(element.HasProfile("profile1"));
            Assert.Contains("profile1", element.Profiles.ToList());
        }

        [Test]
        public void RemoveElement_Message_Success()
        {
            // Arrange
            _iotCore.CreateStructureElement(_iotCore.Root, "struct1");

            // Act
            var response = _iotCore.HandleRequest(new RequestMessage(1, $"/iotcore_management/removeelement",
                Helpers.ToJson(new RemoveElementRequestServiceData("/struct1"))));

            // Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            var element = _iotCore.Root.GetElementByAddress("/struct1");
            Assert.True(element == null);
        }

        [Test]
        public void RemoveElement_Message_AsString_Success()
        {
            // Arrange
            _iotCore.CreateStructureElement(_iotCore.Root, "struct1");
            
            var serializedMessage = "{\"code\":10,\"cid\":1,\"adr\":\"/iotcore_management/removeelement\",\"data\":{\"adr\":\"/struct1\",\"persist\":false}}";
            var deserializedMessage = this._converter.Deserialize<RequestMessage>(serializedMessage);

            // Act
            var response = _iotCore.HandleRequest(deserializedMessage);

            //// Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            var element = _iotCore.Root.GetElementByAddress("/struct1");
            Assert.True(element == null);
        }

        [Test]
        public void AddProfile_Message_Success()
        {
            // Arrange
            var element = _iotCore.CreateStructureElement(_iotCore.Root, "struct1");

            var element2 = _iotCore.CreateStructureElement(_iotCore.Root, "struct2");

            element2.AddProfile("profile3");

            // Act
            var response = _iotCore.HandleRequest(new RequestMessage(1, $"/iotcore_management/addprofile",
                Helpers.ToJson(new AddProfileRequestServiceData(new List<string> { "/struct1", element2.Address,  "/Not/Existing/Element" }, new List<string> { "profile1", "profile2", "profile3" }))));

            var addprofileResponse = Helpers.FromJson<AddProfileResponseServiceData>(response.Data);

            Assert.That(addprofileResponse.TryGetValue("/struct1", out var list));

            Assert.That(list.Any(x=>x.Profile.Equals("profile1") && x.Code == ProfileAddCode.Ok));
            Assert.That(list.Any(x=>x.Profile.Equals("profile2") && x.Code == ProfileAddCode.Ok));
            Assert.That(list.Any(x=>x.Profile.Equals("profile3") && x.Code == ProfileAddCode.Ok));
            Assert.That(!list.Any(x=>x.Profile.Equals("profile3") && x.Code == ProfileAddCode.ProfileAlreadyExistsOnElement));

            // Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            Assert.That(element.HasProfile("profile1"));
            Assert.Contains("profile1", element.Profiles.ToList());
        }

        [Test]
        public void RemoveProfile_Message_Success()
        {
            // Arrange
            var element = _iotCore.CreateStructureElement(_iotCore.Root, "struct1", profiles: new List<string> { "profile1", "profile2" });

            var element2 = _iotCore.CreateStructureElement(_iotCore.Root, "struct2", profiles: new List<string> { "profile3", "profile4" });

            // Act
            var response = _iotCore.HandleRequest(new RequestMessage(1, $"/iotcore_management/removeprofile",
                Helpers.ToJson(new RemoveProfileRequestServiceData(new List<string> { element.Address, element2.Address , "/Not/Existing/Element"}, new List<string> { "profile1", "Profile2", "Profile3" }))));

            var responseData = Helpers.FromJson<RemoveProfileResponseServiceData>(response.Data);

            Assert.That(responseData.TryGetValue(element.Address, out var list));
            
            Assert.That(list.Any(x => x.Code == ProfileRemoveCode.Ok && x.Profile.Equals("profile1")));
             
            // Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            var profiles = element.Profiles.ToList();
            Assert.False(profiles.Contains("profile1"));
            Assert.True(profiles.Contains("profile2"));

            Assert.That(!element.HasProfile("profile1"));
            Assert.That(element.HasProfile("profile2"));
        }



        [Test]
        public void RemoveProfile_Message_AsString_Success()
        {
            // Arrange
            var element = _iotCore.CreateStructureElement(_iotCore.Root, "struct1", profiles: new List<string> { "profile1", "profile2" });

            var message = new RequestMessage(1, $"/iotcore_management/removeprofile",
                Helpers.ToJson(new RemoveProfileRequestServiceData(new List<string> {element.Address}, new List<string> {"profile1"})));

            var serializedMessage = "{\"code\":10,\"cid\":1,\"adr\":\"/iotcore_management/removeprofile\",\"data\":{\"adrlist\":[\"/struct1\"],\"profiles\":[\"profile1\"],\"persist\":false}}";
            var deserializedMessage = this._converter.Deserialize<RequestMessage>(serializedMessage);

            // Act
            var response = _iotCore.HandleRequest(deserializedMessage);

            //// Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            var profiles = element.Profiles.ToList();
            Assert.False(profiles.Contains("profile1"));
            Assert.True(profiles.Contains("profile2"));
        }

        [Test]
        public void AddElement_Message_AsString_Success()
        {
            var message = new RequestMessage(1, $"/iotcore_management/addelement",
                Helpers.ToJson(new AddElementRequestServiceData("/", Identifiers.Structure, "struct1",
                new Format("f1", null, null, null), "uid", new List<string> {"profile1"})));
            
            var serializedMessage = _converter.Serialize(message);
            var deserializedMessage = _converter.Deserialize<RequestMessage>(serializedMessage);

            var response = _iotCore.HandleRequest(deserializedMessage);

            // Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            var element = _iotCore.Root.GetElementByAddress("/struct1");
            Assert.True(element != null);
            Assert.True(element.Type == Identifiers.Structure);
            Assert.True(element.Identifier == "struct1");
            Assert.True(element.Format != null);
            Assert.True(element.Format.Type == "f1");
            Assert.True(element.UId == "uid");
            Assert.True(element.Profiles != null);
            Assert.Contains("profile1", element.Profiles.ToList());
            Assert.That(element.HasProfile("profile1"));
        }

        [Test]
        public void AddElement_Cascaded_Message_AsString_Success()
        {
            var serializedMessage = "{\"code\":10,\"cid\":1,\"adr\":\"/iotcore_management/addelement\",\"data\":{\"adr\":\"/\",\"type\":\"structure\",\"identifier\":\"struct1\",\"format\":{\"type\":\"f1\",\"namespace\":\"json\"},\"uid\":\"uid\",\"profiles\":[\"profile1\"]}}";
            var deserializedMessage = _converter.Deserialize<RequestMessage>(serializedMessage);

            var response = _iotCore.HandleRequest(deserializedMessage);

            // Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            var element = _iotCore.Root.GetElementByAddress("/struct1");
            Assert.True(element != null);
            Assert.True(element.Type == Identifiers.Structure);
            Assert.True(element.Identifier == "struct1");
            Assert.True(element.Format != null);
            Assert.True(element.Format.Type == "f1");
            Assert.True(element.UId == "uid");
            Assert.True(element.Profiles != null);
            Assert.Contains("profile1", element.Profiles.ToList());
            Assert.That(element.HasProfile("profile1"));

            var serializedMessage2 = "{\"code\":10,\"cid\":1,\"adr\":\"/iotcore_management/addelement\",\"data\":{\"adr\":\"/struct1\",\"type\":\"structure\",\"identifier\":\"struct2\",\"format\":{\"type\":\"f2\",\"namespace\":\"json\"},\"uid\":\"uid\",\"profiles\":[\"profile2\"]}}";
            var deserializedMessage2 = _converter.Deserialize<RequestMessage>(serializedMessage2);

            var response2 = _iotCore.HandleRequest(deserializedMessage2);

            // Assert
            Assert.AreEqual(response2.Code, ResponseCodes.Success);
            var element2 = _iotCore.Root.GetElementByAddress("/struct1/struct2");
            Assert.True(element2 != null);
            Assert.True(element2.Type == Identifiers.Structure);
            Assert.True(element2.Identifier == "struct2");
            Assert.True(element2.Format != null);
            Assert.True(element2.Format.Type == "f2");
            Assert.True(element2.UId == "uid");
            Assert.True(element2.Profiles != null);
            Assert.Contains("profile2", element2.Profiles.ToList());
            Assert.That(element2.HasProfile("profile2"));
        }

        [Test]
        public void AddProfile_Message_AsString_Success()
        {
            // Arrange
            var element = _iotCore.CreateStructureElement(_iotCore.Root, "struct1");

            var converter = new ifmIoTCore.Converter.Json.JsonConverter();
            
            var serializedMessage = "{\"code\":10,\"cid\":1,\"adr\":\"/iotcore_management/addprofile\",\"data\":{\"adrlist\":[\"/struct1\"],\"profiles\":[\"profile1\"],\"persist\":false}}";
            var deserializedMessage = converter.Deserialize<RequestMessage>(serializedMessage);

            // Act
            var response = _iotCore.HandleRequest(deserializedMessage);

            //// Assert
            Assert.AreEqual(response.Code, ResponseCodes.Success);
            Assert.Contains("profile1", element.Profiles.ToList());
            Assert.That(element.HasProfile("profile1"));
        }

    }
}