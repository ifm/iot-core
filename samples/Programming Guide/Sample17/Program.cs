namespace Sample17
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
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

                var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, 
                    "struct1");

                ioTCore.CreateSetterServiceElement<EventServiceData>(struct1, 
                    "service1", 
                    HandleService1);

                var string1 = ioTCore.CreateDataElement<string>(struct1, 
                    "string1", 
                    GetString1, 
                    null, 
                    true, 
                    false,
                    format: new StringFormat(new StringValuation(0, 100)));

                var event1 = ioTCore.CreateEventElement(struct1, 
                    "event1");

                event1.Subscribe(new SubscribeRequestServiceData("/struct1/service1",
                  new List<string> { string1.Address }));

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
            EventServiceData data, 
            int? cid = null)
        {
            Console.WriteLine("HandleService1 called");

            Console.WriteLine($"Event source={data.EventSource}");
            Console.WriteLine($"Event number={data.EventNumber}");
            foreach (var (key, value) in data.Payload)
            {
                Console.WriteLine($"{key}={value}");
            }
        }
    }
}

