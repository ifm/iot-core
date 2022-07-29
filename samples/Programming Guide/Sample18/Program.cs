namespace Sample18
{
    using System;
    using System.Threading;
    using ifmIoTCore;
    using ifmIoTCore.Elements;

    internal class Program
    {
        static void Main()
        {
            Timer eventTimer = null;
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = new StructureElement("struct1");
                ioTCore.Root.AddChild(struct1);

                var event1 = new EventElement("event1");
                struct1.AddChild(event1);

                event1.Subscribe(HandleEvent1);

                eventTimer = new Timer(RaiseEvent1, event1, 5000, 60000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();

            eventTimer?.Dispose();
        }

        private static void HandleEvent1(object sender)
        {
            Console.WriteLine("HandleEvent1 called");

            Console.WriteLine($"Received event from {((IEventElement)sender).Address}");
        }

        private static void RaiseEvent1(object param)
        {
            Console.WriteLine("RaiseEvent1 called");

            var event1 = (IEventElement)param;
            event1.RaiseEvent();
        }
    }
}