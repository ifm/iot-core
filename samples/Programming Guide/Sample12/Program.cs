namespace Sample12
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

                var intField = new Field("intField1",
                  new IntegerFormat(new IntegerValuation(-100, 100)));
                var floatField = new Field("floatField1",
                  new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)));
                var stringField = new Field("stringField1",
                  new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
                
                var object1 = new DataElement<UserData>("object1",
                    GetObject1, 
                    SetObject1,
                    format: new ObjectFormat(new ObjectValuation(new List<Field>
                  {
                      intField, 
                      floatField, 
                      stringField
                  })));
                struct1.AddChild(object1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static UserData GetObject1(IDataElement element)
        {
            return _object1;
        }

        private static void SetObject1(IDataElement element, UserData value)
        {
            if (_object1.Equals(value)) return;
            _object1 = value;
        }
        private static UserData _object1 = new UserData();
    }

    internal class UserData : IEquatable<UserData>
    {
        public string String1;
        public int Int1;
        public float Float1;

        public UserData()
        {
            String1 = "Hallo";
            Int1 = 10;
            Float1 = 1.2345f;
        }

        public bool Equals(UserData other)
        {
            if (other == null) return false;
            return Int1 == other.Int1 &&
                   Float1.Equals(other.Float1) &&
                   String1 == other.String1;
        }
    }
}