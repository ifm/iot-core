namespace Sample09
{
    using System;
    using System.Collections.Generic;
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

                var enum1 = new DataElement<int>("enum1",
                    GetEnum1, 
                    SetEnum1,
                    format: new IntegerEnumFormat(new IntegerEnumValuation(
                        new Dictionary<string, string>
                        {
                            {"0", "null"}, 
                            {"1", "one"}, 
                            {"2", "two"}, 
                            {"3", "three"}
                        }, _enum1)));
                struct1.AddChild(enum1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static int GetEnum1(IDataElement element)
        {
            return _enum1;
        }

        private static void SetEnum1(IDataElement element, int value)
        {
            _enum1 = value;
        }
        private static int _enum1 = 1;
    }
}