namespace Sample31
{
    using System;
    using ifmIoTCore.Common.Variant;

    class Program
    {
        static void Main()
        {
            Variant variantValue1 = new VariantValue(1);
            Variant variantValue2 = Variant.FromObject(2);
            Variant variantValue3 = Variant.FromObject(3.0f);
            Variant variantValue4 = Variant.FromObject('c');

            Variant array = new VariantArray
            {
                variantValue1,
                variantValue2,
                variantValue3,
                variantValue4
            };

            Variant fruits = new VariantArray(new string[] { "apple", "banana", "cherry" });

            Variant obj = new VariantObject
            {
                { "items", array },
                { "fruits", fruits }
            };

            WorkWithVariant(array);
            WorkWithVariant(fruits);
            WorkWithVariant(obj);
        }

        static void WorkWithVariant(Variant variant)
        {
            if (variant is VariantObject variantObject)
            {
                var numbers = variantObject["items"];
                var firstItemAsInt = numbers.AsVariantArray()[0].ToObject<int>();
            }
            else if (variant is VariantArray variantArray)
            {
                var firstItem = variantArray[0];

                try
                {
                    int firstItemAsInt = firstItem.ToObject<int>();
                }
                catch (FormatException e)
                {
                    // This exception will be thrown when working with the fruits array, since the string "apple" is not convertible to an integer.
                }

                
                // This will succeed with both the arrays, the results are "apple" and "1".
                string firstItemAsString = firstItem.ToObject<string>();

                // The cast operators are overloaded for VariantValue so casts to simple types also do a conversion.
                string firstItemAsStringCast = (string)(VariantValue)firstItem; 
            }
        }
    }
}