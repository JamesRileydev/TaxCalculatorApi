using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaxCalculator.Api.Converters
{
    public class ExceptionConverter : JsonConverter<Exception>
    {
        public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Reading exceptions from JSON is not supported");
        }

        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            WriteJsonException(writer, value);
        }

        private void WriteJsonException(Utf8JsonWriter writer, Exception value)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            writer.WriteString("type", value.GetType().Name);
            writer.WriteString("message", value.Message);
            writer.WriteString("stackTrace", value.StackTrace);
            writer.WritePropertyName("innerException");
            WriteJsonException(writer, value.InnerException);

            writer.WriteEndObject();
        }
    }
}
