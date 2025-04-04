using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Helper;

public class GenConverter<T, I> : JsonConverter<I> where T : I
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(I) || base.CanConvert(typeToConvert);
    }
    public override I? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, I value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
