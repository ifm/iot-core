namespace ifm.IoTCore.UnitTests
{
    using ifm.Common.Variant;
    using NUnit.Framework;
    using ElementManager.Contracts.Elements.ServiceData.Responses;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_getidentity_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T54")]
        public void getIdentity_Response_Has_iot_device_security()
        { // assumes command interface tests pass
            var ioTCore = Helpers.CreateIoTCore("myiotcore");
            var msg = ioTCore.MessageDispatcher.HandleRequest(0, "/getidentity");
            var getidentityResponse = Variant.ToObject<GetIdentityResponseServiceData>(msg.Data);
            // using XPath-like JPath expressions for powerful queries in string instead of script
            var iot = getidentityResponse.IoT;
            Assert.That(iot,Is.Not.Null);
            //var device = getidentityResponse.Device;
            //Assert.That(device,Is.Not.Null);
            // TODO enable security when available
            //var security = getidentityResponse.Security;
            //Assert.That(security,Is.Not.Null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T54")]
        public void getIdentity_Response_HasMember_iot_Has_RequiredMembers()
        { // assumes command interface tests pass
            var myiotcore = Helpers.CreateIoTCore("myiotcore");
            var getIdentityResponse = Variant.ToObject<GetIdentityResponseServiceData>(myiotcore.MessageDispatcher.HandleRequest(0, "/getidentity").Data);

            Assert.Multiple(() => { 
                Assert.That(getIdentityResponse.IoT.Name,Is.Not.Null);
                Assert.That(getIdentityResponse.IoT.Version,Is.Not.Null);

                // Ignore, because buggy in iolinkmaster and optional anyway
                // As discussed w/ Matthieu on 11-4-2021
                //Assert.That(getIdentityResponse.SelectToken("$.iot.serverlist"),Is.Not.Null);

                //var interfaces = getIdentityResponse.SelectToken("$.iot.serverlist");
                //foreach (var iface in interfaces)
                //{
                //    Assert.That(iface.SelectToken("$.type"),Is.Not.Null);
                //    Assert.That(iface.SelectToken("$.uri"),Is.Not.Null);
                //    Assert.That(iface.SelectToken("$.formats"),Is.Not.Null);
                //}
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T54")]
        [Ignore("TODO implement when 'device' member enabled. currently device value is null (19.Nov.2021)")]
        public void getIdentity_Response_HasMember_device_Has_RequiredMembers()
        { // assumes command interface tests pass
        }

        [Test, Property("TestCaseKey", "IOTCS-T54")]
        [Ignore("The security item is defined as optional. The security item is being under discussion at this time.(25.10.2021).")]
        public void getIdentity_Response_HasMember_security_Has_RequiredMembers()
        { // assumes command interface tests pass
            var myiotcore = Helpers.CreateIoTCore("myiotcore");
            // start server, take data, stop server
            var getIdentityResponse = Variant.ToObject<GetIdentityResponseServiceData>(myiotcore.MessageDispatcher.HandleRequest(0, "/getidentity").Data);

            // Assert
            Assert.Multiple(() => { 
                Assert.That(getIdentityResponse.Security.Mode,Is.Not.Null);
                Assert.That(getIdentityResponse.Security.AuthenticationScheme,Is.Not.Null);
            });
        }

    }
}
