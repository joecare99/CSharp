using GenInterfaces.Interfaces.Genealogic;
using GenInterfaces.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using BaseLib.Helper;
using System.Collections.Generic;
using System;

namespace BaseGenClasses.Helper;

public class GenPolyConverter<I> : JsonConverter<I> where I : class, IGenBase
{
    private Dictionary<EGenType, Type> genTypeDict = new();

    public GenPolyConverter(IEnumerable<(EGenType, Type)> genTypes)
    {
        foreach (var gt in genTypes)
        {
            genTypeDict.Add(gt.Item1, gt.Item2);
        }
    }

    public override I? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Utf8JsonReader discrReader = reader;

        if (discrReader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        if (!discrReader.Read() || discrReader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        while (discrReader.GetString().StartsWith("$"))
        {
            discrReader.Read();
            discrReader.Skip();
            if (!discrReader.Read() || discrReader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
        }

        if (!discrReader.Read() || discrReader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException();
        }

        EGenType genType = discrReader.GetInt32().AsEnum<EGenType>();


        if (!genTypeDict.TryGetValue(genType, out var type) || type == null)
        {
            throw new JsonException();
        }

        var result = (I)JsonSerializer.Deserialize(ref reader, type, options);

        return result;
    }

    public override void Write(Utf8JsonWriter writer, I value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
