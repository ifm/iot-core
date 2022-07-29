namespace Sample11
{
    using System;
    using System.Linq;
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

                var array1 = new DataElement<int[]>("array1",
                    GetArray1, 
                    SetArray1,
                    format: new ArrayFormat(new ArrayValuation(Format.Types.Number, 
                        new IntegerFormat(new IntegerValuation(0, 100)))));
                struct1.AddChild(array1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static int[] GetArray1(IDataElement element)
        {
            return _array1;
        }

        private static void SetArray1(IDataElement element, int[] value)
        {
            if (_array1.SequenceEqual(value)) return;
            _array1 = value;
        }
        private static int[] _array1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }
}
