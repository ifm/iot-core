namespace Sample18
{
    using System;
    using System.Threading;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Utilities;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, 
                    "struct1");

                var event1 = ioTCore.CreateEventElement(struct1, 
                    "event1");

                event1.Subscribe(HandleNotificationEvent);

                var eventTimer = new Timer(TriggerEvent1, event1, 5000, 60000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static void HandleNotificationEvent(IEventElement element)
        {
            Console.WriteLine("HandleNotificationEvent called");

            Console.WriteLine($"Received event from {element.Address}");
        }

        private static void TriggerEvent1(object param)
        {
            Console.WriteLine("TriggerEvent1 called");

            var event1 = (IEventElement)param;
            event1.RaiseEvent();
        }
    }
}