using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integrity.Banking.Api
{
    public class DecimalConverter : JsonConverter<decimal>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(decimal);
        }

        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.TryGetDecimal(out var readValue);
            return readValue;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteRawValue($"{value:0.00}");
        }
    }
}
