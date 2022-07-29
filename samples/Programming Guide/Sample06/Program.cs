namespace Sample06
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

                var int1 = new DataElement<int>("int1",
                    GetInt1, 
                    SetInt1,
                    format: new IntegerFormat(new IntegerValuation(0, 100)));

                struct1.AddChild(int1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static int GetInt1(IDataElement element)
        {
            return _int1;
        }

        private static void SetInt1(IDataElement element, int value)
        {
            _int1 = value;
        }
        private static int _int1 = 10;
    }
}
