using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements.ServiceData.Responses;

namespace ifmIoTCore.UnitTests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_getidentity_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T54")]
        public void getIdentity_Response_Has_iot_device_security()
        { // assumes command interface tests pass
            using var ioTCore = IoTCoreFactory.Create("myiotcore");
            var msg = ioTCore.HandleRequest(0, "/getidentity");
            var getidentityResponse = Variant.ToObject<GetIdentityResponseServiceData>(msg.Data);
            // using XPath-like JPath expressions for powerful queries in string instead of script
            var iot = getidentityResponse.IoT;
            Assert.NotNull(iot);
            var device = getidentityResponse.Device;
            Assert.NotNull(device);
            // TODO enable security when available
            //var security = getidentityResponse.Security;
            //Assert.NotNull(security);
        }

        [Test, Property("TestCaseKey", "IOTCS-T54")]
        public void getIdentity_Response_HasMember_iot_Has_RequiredMembers()
        { // assumes command interface tests pass
            var myiotcore = IoTCoreFactory.Create("myiotcore");
            var getIdentityResponse = Variant.ToObject<GetIdentityResponseServiceData>(myiotcore.HandleRequest(0, "/getidentity").Data);

            Assert.Multiple(() => { 
                Assert.NotNull(getIdentityResponse.IoT.Name);
                Assert.NotNull(getIdentityResponse.IoT.Version);

                // Ignore, because buggy in iolinkmaster and optional anyway
                // As discussed w/ Matthieu on 11-4-2021
                //Assert.NotNull(getIdentityResponse.SelectToken("$.iot.serverlist"));

                //var interfaces = getIdentityResponse.SelectToken("$.iot.serverlist");
                //foreach (var iface in interfaces)
                //{
                //    Assert.NotNull(iface.SelectToken("$.type"));
                //    Assert.NotNull(iface.SelectToken("$.uri"));
                //    Assert.NotNull(iface.SelectToken("$.formats"));
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
            var myiotcore = IoTCoreFactory.Create("myiotcore");
            // start server, take data, stop server
            var getIdentityResponse = Variant.ToObject<GetIdentityResponseServiceData>(myiotcore.HandleRequest(0, "/getidentity").Data);

            // Assert
            Assert.Multiple(() => { 
                Assert.NotNull(getIdentityResponse.Security.Mode);
                Assert.NotNull(getIdentityResponse.Security.AuthenticationScheme);
                Assert.NotNull(getIdentityResponse.Security.IsPasswordSet);
            });
        }

    }
}
