namespace Sample05
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = new StructureElement("struct1");
                ioTCore.Root.AddChild(struct1);

                var bool1 = new DataElement<bool>("bool1",
                    GetBool1, 
                    SetBool1,
                    format: new BooleanFormat());
                struct1.AddChild(bool1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static bool GetBool1(IDataElement element)
        {
            return _bool1;
        }

        private static void SetBool1(IDataElement element, bool value)
        {
            _bool1 = value;
        }
        private static bool _bool1 = true;
    }
}