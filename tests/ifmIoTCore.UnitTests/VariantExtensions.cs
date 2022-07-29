using ifmIoTCore.MessageConverter.Json.Newtonsoft;
using Newtonsoft.Json.Linq;

namespace ifmIoTCore.UnitTests
{
    internal static class VariantExtensions
    {
        internal static JToken ToJToken(this ifmIoTCore.Common.Variant.Variant variant)
        {
            return VariantConverter.ToJToken(variant);
        }
    }
}
