namespace Sample07
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.Common;
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

                var float1 = new DataElement<float>("float1",
                    GetFloat1, 
                    SetFloat1,
                    format: new FloatFormat(new FloatValuation(-9.9f, 9.9f, 3)));
                struct1.AddChild(float1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static float GetFloat1(IDataElement element)
        {
            return _float1;
        }

        private static void SetFloat1(IDataElement element, float value)
        {
            if (_float1.EqualsWithPrecision(value)) return;
            _float1 = value;
        }
        private static float _float1 = 1.2345f;
    }
}