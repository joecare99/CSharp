using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Filter.JSON.Model;

/// <summary>
/// Serializable JSON representation of the canonical trace data set.
/// </summary>
[DataContract]
public sealed class JsonTraceModel
{
    /// <summary>
    /// Stable payload marker for deterministic JSON trace cache files.
    /// </summary>
    public const string PayloadFormat = "TraceAnalysis.JsonTrace/1.0";

    /// <summary>
    /// Gets or sets the payload format marker.
    /// </summary>
    [DataMember(Name = "format", Order = 1)]
    public string Format { get; set; } = PayloadFormat;

    /// <summary>
    /// Gets or sets the original source identifier.
    /// </summary>
    [DataMember(Name = "sourceId", Order = 2)]
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized field metadata.
    /// </summary>
    [DataMember(Name = "fields", Order = 3)]
    public List<JsonTraceFieldModel> Fields { get; set; } = new();

    /// <summary>
    /// Gets or sets the serialized records.
    /// </summary>
    [DataMember(Name = "records", Order = 4)]
    public List<JsonTraceRecordModel> Records { get; set; } = new();

    /// <summary>
    /// Gets or sets collected parse errors.
    /// </summary>
    [DataMember(Name = "parseErrors", Order = 5)]
    public List<string> ParseErrors { get; set; } = new();

    /// <summary>
    /// Creates a JSON model from a canonical trace data set.
    /// </summary>
    public static JsonTraceModel FromDataSet(ITraceDataSet dataSet)
    {
        if (dataSet == null)
            throw new ArgumentNullException(nameof(dataSet));

        var model = new JsonTraceModel
        {
            SourceId = dataSet.Metadata.sSourceId
        };

        foreach (var field in dataSet.Metadata.Fields)
        {
            model.Fields.Add(new JsonTraceFieldModel
            {
                Name = field.sName,
                Group = field.sGroup,
                Format = field.sFormat,
                Type = SerializeTypeName(field.FieldType)
            });
        }

        foreach (var record in dataSet.Records)
        {
            var jsonRecord = new JsonTraceRecordModel
            {
                Timestamp = JsonTraceValueModel.FromValue(record.Timestamp)
            };

            foreach (var field in dataSet.Metadata.Fields)
            {
                record.Values.TryGetValue(field.sName, out var value);
                jsonRecord.Values.Add(new JsonTraceValueEntryModel
                {
                    Name = field.sName,
                    Value = JsonTraceValueModel.FromValue(value)
                });
            }

            model.Records.Add(jsonRecord);
        }

        model.ParseErrors.AddRange(dataSet.ParseErrors);
        return model;
    }

    /// <summary>
    /// Converts this JSON model back to the canonical trace data set.
    /// </summary>
    public TraceDataSet ToDataSet()
    {
        var fields = new List<ITraceFieldMetadata>(Fields.Count);
        foreach (var field in Fields)
            fields.Add(new TraceFieldMetadata(field.Name ?? string.Empty, DeserializeType(field.Type), field.Group, field.Format));

        var records = new List<ITraceRecord>(Records.Count);
        foreach (var record in Records)
        {
            var values = new Dictionary<string, object?>();
            foreach (var value in record.Values)
                values[value.Name ?? string.Empty] = value.Value?.ToValue();

            var timestamp = record.Timestamp?.ToValue() ?? string.Empty;
            records.Add(new TraceRecord(timestamp, values));
        }

        return new TraceDataSet(new TraceMetadata(SourceId ?? string.Empty, fields), records, ParseErrors);
    }

    /// <summary>
    /// Reads a JSON trace model from a stream.
    /// </summary>
    public static JsonTraceModel Read(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var serializer = CreateSerializer();
        var model = serializer.ReadObject(stream) as JsonTraceModel;
        if (model == null)
            throw new SerializationException("The JSON payload did not contain a trace model.");

        if (!string.Equals(model.Format, PayloadFormat, StringComparison.Ordinal))
            throw new SerializationException($"Unsupported trace JSON format '{model.Format}'.");

        model.Fields ??= new List<JsonTraceFieldModel>();
        model.Records ??= new List<JsonTraceRecordModel>();
        model.ParseErrors ??= new List<string>();
        foreach (var record in model.Records)
            record.Values ??= new List<JsonTraceValueEntryModel>();

        return model;
    }

    /// <summary>
    /// Writes this JSON trace model to a stream.
    /// </summary>
    public void Write(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var serializer = CreateSerializer();
        serializer.WriteObject(stream, this);
        stream.Flush();
    }

    /// <summary>
    /// Writes this JSON trace model to a formatted JSON string.
    /// </summary>
    public string ToJson()
    {
        using var stream = new MemoryStream();
        Write(stream);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: false);
        return reader.ReadToEnd();
    }

    private static DataContractJsonSerializer CreateSerializer()
    {
        return new DataContractJsonSerializer(typeof(JsonTraceModel), new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = false
        });
    }

    private static string? SerializeTypeName(Type? type)
    {
        return type?.AssemblyQualifiedName;
    }

    private static Type? DeserializeType(string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        return Type.GetType(typeName, throwOnError: false);
    }
}

/// <summary>
/// Serializable field metadata entry.
/// </summary>
[DataContract]
public sealed class JsonTraceFieldModel
{
    [DataMember(Name = "name", Order = 1)]
    public string? Name { get; set; }

    [DataMember(Name = "type", Order = 2, EmitDefaultValue = false)]
    public string? Type { get; set; }

    [DataMember(Name = "group", Order = 3, EmitDefaultValue = false)]
    public string? Group { get; set; }

    [DataMember(Name = "format", Order = 4, EmitDefaultValue = false)]
    public string? Format { get; set; }
}

/// <summary>
/// Serializable record entry.
/// </summary>
[DataContract]
public sealed class JsonTraceRecordModel
{
    [DataMember(Name = "timestamp", Order = 1)]
    public JsonTraceValueModel? Timestamp { get; set; }

    [DataMember(Name = "values", Order = 2)]
    public List<JsonTraceValueEntryModel> Values { get; set; } = new();
}

/// <summary>
/// Serializable name/value entry for a field value.
/// </summary>
[DataContract]
public sealed class JsonTraceValueEntryModel
{
    [DataMember(Name = "name", Order = 1)]
    public string? Name { get; set; }

    [DataMember(Name = "value", Order = 2)]
    public JsonTraceValueModel? Value { get; set; }
}

/// <summary>
/// Serializable primitive value wrapper with explicit type metadata.
/// </summary>
[DataContract]
public sealed class JsonTraceValueModel
{
    [DataMember(Name = "kind", Order = 1)]
    public string Kind { get; set; } = "null";

    [DataMember(Name = "value", Order = 2, EmitDefaultValue = false)]
    public string? Value { get; set; }

    public static JsonTraceValueModel FromValue(object? value)
    {
        if (value == null)
            return new JsonTraceValueModel();

        switch (value)
        {
            case string stringValue:
                return Create("string", stringValue);
            case bool boolValue:
                return Create("bool", boolValue ? "true" : "false");
            case byte byteValue:
                return Create("byte", byteValue.ToString(CultureInfo.InvariantCulture));
            case sbyte sbyteValue:
                return Create("sbyte", sbyteValue.ToString(CultureInfo.InvariantCulture));
            case short shortValue:
                return Create("int16", shortValue.ToString(CultureInfo.InvariantCulture));
            case ushort ushortValue:
                return Create("uint16", ushortValue.ToString(CultureInfo.InvariantCulture));
            case int intValue:
                return Create("int32", intValue.ToString(CultureInfo.InvariantCulture));
            case uint uintValue:
                return Create("uint32", uintValue.ToString(CultureInfo.InvariantCulture));
            case long longValue:
                return Create("int64", longValue.ToString(CultureInfo.InvariantCulture));
            case ulong ulongValue:
                return Create("uint64", ulongValue.ToString(CultureInfo.InvariantCulture));
            case float floatValue:
                return Create("single", floatValue.ToString("R", CultureInfo.InvariantCulture));
            case double doubleValue:
                return Create("double", doubleValue.ToString("R", CultureInfo.InvariantCulture));
            case decimal decimalValue:
                return Create("decimal", decimalValue.ToString(CultureInfo.InvariantCulture));
            case DateTime dateTimeValue:
                return Create("datetime", dateTimeValue.ToString("O", CultureInfo.InvariantCulture));
            case DateTimeOffset dateTimeOffsetValue:
                return Create("datetimeoffset", dateTimeOffsetValue.ToString("O", CultureInfo.InvariantCulture));
            case Guid guidValue:
                return Create("guid", guidValue.ToString("D", CultureInfo.InvariantCulture));
            default:
                return Create("string", Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
        }
    }

    public object? ToValue()
    {
        switch (Kind)
        {
            case "null":
                return null;
            case "string":
                return Value ?? string.Empty;
            case "bool":
                return bool.Parse(Value ?? "false");
            case "byte":
                return byte.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "sbyte":
                return sbyte.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "int16":
                return short.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "uint16":
                return ushort.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "int32":
                return int.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "uint32":
                return uint.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "int64":
                return long.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "uint64":
                return ulong.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "single":
                return float.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "double":
                return double.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "decimal":
                return decimal.Parse(Value ?? "0", CultureInfo.InvariantCulture);
            case "datetime":
                return DateTime.Parse(Value ?? string.Empty, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            case "datetimeoffset":
                return DateTimeOffset.Parse(Value ?? string.Empty, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            case "guid":
                return Guid.Parse(Value ?? Guid.Empty.ToString("D", CultureInfo.InvariantCulture));
            default:
                return Value;
        }
    }

    private static JsonTraceValueModel Create(string kind, string? value)
    {
        return new JsonTraceValueModel
        {
            Kind = kind,
            Value = value
        };
    }
}
