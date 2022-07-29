namespace Sample08
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
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

                var string1 = new DataElement<string>("string1",
                    GetString1, 
                    SetString1,
                    format: new StringFormat(new StringValuation(0, 100)));
                struct1.AddChild(string1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static string GetString1(IDataElement element)
        {
            return _string1;
        }

        private static void SetString1(IDataElement element, string value)
        {
            _string1 = value;
        }
        private static string _string1 = "Hallo";
    }
}