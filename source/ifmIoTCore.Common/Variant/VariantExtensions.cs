namespace ifmIoTCore.Common.Variant
{
    public static class VariantExtensions
    {
        public static VariantObject AsVariantObject(this Variant variant)
        {
            return (VariantObject) variant;
        }

        public static VariantArray AsVariantArray(this Variant variant)
        {
            return (VariantArray)variant;
        }

        public static VariantValue AsVariantValue(this Variant variant)
        {
            return (VariantValue)variant;
        }
    }
}
