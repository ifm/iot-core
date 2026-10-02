namespace ifm.IoTCore.UnitTests.Elements;

using System;
using System.Diagnostics.CodeAnalysis;
using ElementManager.Contracts.Elements.ServiceData.Responses;
using ifm.Common.Variant;
using NUnit.Framework;

[ExcludeFromCodeCoverage]
[TestFixture]
[Parallelizable(ParallelScope.None)]
public class GetSetDataTests
{
    [Test]
    public void LocalGetDataTest()
    {
        try
        {
            var ioTCore1 = Helpers.CreateIoTCore("id0");
                
            var dataElement = ioTCore1.ElementManager.CreateSimpleDataElement(ioTCore1.Root, "data0", "data123"); 

            var getDataResponse = ioTCore1.MessageDispatcher.HandleRequest(0, "/data0/getdata");
            Assert.That(getDataResponse,Is.Not.Null);
            Assert.That(getDataResponse.Code, Is.EqualTo(200));

            var data = Variant.ToObject<GetDataResponseServiceData>(getDataResponse.Data);

            Assert.That(string.Equals("data123", (string)(VariantValue)data.Value));
        }
        catch (Exception exception)
        {
            Assert.Fail($"An exeption occured. The message was: {exception.Message}");
        }
    }

    [Test]
    public void LocalSetDataTest()
    {
        var iotCore = Helpers.CreateIoTCore("id0");

        string data = null;
            
        var dataElement = iotCore.ElementManager.CreateDataElement(iotCore.Root, "data0",
            (b, i) => data,
            (b, o, i) => { data = o;});

        dataElement.Value = "asdf";
        var result = dataElement.Value;

        Assert.That(string.Equals("asdf", result));
    }


    [Test]
    public void GetDataTimeStampTest()
    {
        try
        {
            var ioTCore1 = Helpers.CreateIoTCore("id0");

            // Test simple data element
            var simpleDataElement = ioTCore1.ElementManager.CreateSimpleDataElement(ioTCore1.Root, "data0", "data123");

            // Test programming API
            var d = simpleDataElement.GetData();
            Assert.That(d.TimeStamp > 0);

            // Test network API
            var getDataResponse = ioTCore1.MessageDispatcher.HandleRequest(0, "/data0/getdata");
            d = Variant.ToObject<GetDataResponseServiceData>(getDataResponse.Data);
            Assert.That(d.TimeStamp > 0);

            var dataElement = ioTCore1.ElementManager.CreateReadOnlyDataElement(ioTCore1.Root, "data1", (s, i) => "data123");

            // Test programming API
            d = dataElement.GetData();
            Assert.That(d.TimeStamp > 0);

            // Test network API
            getDataResponse = ioTCore1.MessageDispatcher.HandleRequest(0, "/data1/getdata");
            d = Variant.ToObject<GetDataResponseServiceData>(getDataResponse.Data);
            Assert.That(d.TimeStamp > 0);
        }
        catch (Exception exception)
        {
            Assert.Fail($"An exeption occured. The message was: {exception.Message}");
        }
    }
}
