namespace Sample17
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Common.Variant;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.ServiceData.Events;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.Valuations;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = new StructureElement("struct1");
                ioTCore.Root.AddChild(struct1);

                var service1 = new SetterServiceElement("service1", HandleService1);
                struct1.AddChild(service1);

                var string1 = new ReadOnlyDataElement<string>("string1", 
                    GetString1, 
                    format: new StringFormat(new StringValuation(0, 100)));
                struct1.AddChild(string1);

                var event1 = new EventElement("event1");
                struct1.AddChild(event1);

                var data = Variant.FromObject(
                    new SubscribeRequestServiceData("/struct1/service1", 
                        new List<string> { string1.Address }));
                event1.SubscribeServiceElement.Invoke(data);

                event1.RaiseEvent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static string GetString1(IBaseElement element)
        {
            return _string1;
        }
        private static string _string1 = "Hallo";

        private static void HandleService1(IBaseElement element,
            Variant data, 
            int? cid = null)
        {
            Console.WriteLine("HandleService1 called");

            var eventServiceData = Variant.ToObject<EventServiceData>(data);

            Console.WriteLine($"Event source={eventServiceData.EventAddress}");
            Console.WriteLine($"Event number={eventServiceData.EventNumber}");
            foreach (var (key, value) in eventServiceData.Payload)
            {
                Console.WriteLine($"{key}={value.Code},{value.Data}");
            }
        }
    }
}