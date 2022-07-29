namespace ifmIoTCore.Contracts.UnitTests
{
    using System;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class TestClass
    {

        [Test]
        public void TestMethod()
        {
            var result = VariantCsSourceWriter.WriteSource(@"{
                                'callback': 'http://127.0.0.1:8050/somepath/', 
                                'datatosend': ['/data1','/data2'], 
                                'subscribeid': 1}");
        }
    }

    public class VariantCsSourceWriter
    {
        public static string WriteSource(string json)
        {
            var writer = new StringWriter();
            var jToken = JToken.Parse(json);
            WriteJToken(writer, jToken);
            return writer.ToString();
        }

        private static void WriteJToken(StringWriter writer, JToken jToken)
        {
            if (jToken is JObject jObject)
            {
                WriteObject(writer, jObject);
            } 
            else if (jToken is JValue jValue)
            {
                WriteJValue(writer, jValue);
            }
            else if (jToken is JArray jArray)
            {
                WriteJArray(writer, jArray);
            }
        }

        private static void WriteJArray(StringWriter writer, JArray jArray)
        {
            writer.Write("new VariantArray () {");

            foreach (var item in jArray)
            {
                WriteJToken(writer, item);
                if (jArray.Last != item)
                {
                    writer.Write(",");
                }
            }

            writer.Write("}");
        }

        private static void WriteJValue(StringWriter writer, JValue jValue)
        {
            if (jValue.Type == JTokenType.Boolean)
            {
                writer.Write($"new VariantValue({(bool)jValue.Value})");
            } else if (jValue.Type == JTokenType.Float)
            {
                writer.Write($"new VariantValue({(float)jValue.Value})");
            } else if (jValue.Type == JTokenType.Integer)
            {
                writer.Write($"new VariantValue({(long)Convert.ToInt64(jValue.Value)})");
            } else if (jValue.Type == JTokenType.String)
            {
                writer.Write($"new VariantValue(\"{(string)jValue.Value}\")");
            }
            else if (jValue.Type == JTokenType.Null)
            {
                writer.Write($"new VariantValue()");
            }
            else
            {
                throw new ArgumentOutOfRangeException($"The tokentype {jValue.Type} is unhandled.");
            }
        }

        private static void WriteObject(StringWriter writer, JObject jObject)
        {
            writer.Write("new VariantObject() {");

            foreach (var item in jObject)
            {
                writer.Write("{");

                writer.Write($"\"{item.Key}\",");

                WriteJToken(writer, item.Value);

                writer.Write("}");

                
                writer.Write(",");
                
            }

            writer.Write("}");
        }
    }
}
