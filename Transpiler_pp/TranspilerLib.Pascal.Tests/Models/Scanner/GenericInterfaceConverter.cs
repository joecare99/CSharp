using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

#pragma warning disable IDE0130
namespace TranspilerLib.Pascal.Models.Scanner.Tests;

// Allgemein gehaltener Converter für Interface -> konkrete Implementierung (hier ICodeBlock -> CodeBlock-Abkömmling).
// Verwendet Reflection, optional Property-Whitelist. Standard: Name, Code, Type, SubBlocks.
public class GenericInterfaceConverter<TInterface, TConcrete> : JsonConverter<TInterface>
    where TInterface : class
    where TConcrete : class, TInterface, new()
{
    private readonly Dictionary<string, PropertyInfo> _props;
    private readonly bool _isCodeBlockLike;
    private readonly PropertyInfo? _subBlocksProp;
    private readonly PropertyInfo? _typeProp;

    public GenericInterfaceConverter(params string[]? propertyWhitelist)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public;
        var allProps = typeof(TConcrete).GetProperties(flags)
            .Where(p => p.CanRead && p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        if (propertyWhitelist is { Length: > 0 })
            _props = propertyWhitelist
                .Where(n => allProps.ContainsKey(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(n => n, n => allProps[n], StringComparer.OrdinalIgnoreCase);
        else
            _props = allProps;

        _isCodeBlockLike = typeof(ICodeBlock).IsAssignableFrom(typeof(TInterface))
                           && typeof(CodeBlock).IsAssignableFrom(typeof(TConcrete));

        // Spezielle Behandlung für ICodeBlock-ähnliche Typen
        if (_isCodeBlockLike)
        {
            _subBlocksProp = _props.Values.FirstOrDefault(p =>
                typeof(IList<ICodeBlock>).IsAssignableFrom(p.PropertyType));
            _typeProp = _props.Values.FirstOrDefault(p => p.PropertyType == typeof(CodeBlockType));

            // Sicherstellen, dass Standardfelder vorhanden sind
            AddIfMissing(allProps, "Name");
            AddIfMissing(allProps, "Code");
            AddIfMissing(allProps, "Type");
            AddIfMissing(allProps, "SubBlocks");
        }

        void AddIfMissing(Dictionary<string, PropertyInfo> source, string name)
        {
            if (!_props.ContainsKey(name) && source.ContainsKey(name))
                _props[name] = source[name];
        }
    }

    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var instance = new TConcrete();

        foreach (var jsonProp in root.EnumerateObject())
        {
            var name = jsonProp.Name;
            if (!_props.TryGetValue(name, out var pInfo))
                continue;

            try
            {
                if (_isCodeBlockLike && _subBlocksProp != null && pInfo == _subBlocksProp &&
                    jsonProp.Value.ValueKind == JsonValueKind.Array)
                {
                    var list = (IList<ICodeBlock>?)_subBlocksProp.GetValue(instance)
                               ?? new List<ICodeBlock>();
                    foreach (var el in jsonProp.Value.EnumerateArray())
                    {
                        var child = JsonSerializer.Deserialize<ICodeBlock>(el.GetRawText(), options);
                        if (child != null) list.Add(child);
                    }
                    _subBlocksProp.SetValue(instance, list);
                    continue;
                }

                object? value = null;
                switch (jsonProp.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        if (pInfo.PropertyType == typeof(string))
                            value = jsonProp.Value.GetString();
                        else if (pInfo.PropertyType.IsEnum)
                        {
                            var s = jsonProp.Value.GetString();
                            if (Enum.TryParse(pInfo.PropertyType, s, true, out var enumVal))
                                value = enumVal;
                        }
                        break;

                    case JsonValueKind.Number:
                        if (pInfo.PropertyType == typeof(int) && jsonProp.Value.TryGetInt32(out var i))
                            value = i;
                        else if (pInfo.PropertyType == typeof(long) && jsonProp.Value.TryGetInt64(out var l))
                            value = l;
                        else if (pInfo.PropertyType == typeof(double) && jsonProp.Value.TryGetDouble(out var d))
                            value = d;
                        else if (pInfo.PropertyType == typeof(float) && jsonProp.Value.TryGetDouble(out var f))
                            value = (float)f;
                        break;

                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        if (pInfo.PropertyType == typeof(bool))
                            value = jsonProp.Value.GetBoolean();
                        break;

                    case JsonValueKind.Array:
                        // Generische einfache List<T> (nur für primitive oder String)
                        if (pInfo.PropertyType.IsGenericType &&
                            typeof(IList<>).IsAssignableFrom(pInfo.PropertyType.GetGenericTypeDefinition()))
                        {
                            var elemType = pInfo.PropertyType.GetGenericArguments()[0];
                            if (elemType == typeof(string))
                            {
                                var arr = (IList<string>)Activator.CreateInstance(pInfo.PropertyType)!;
                                foreach (var el in jsonProp.Value.EnumerateArray())
                                    if (el.ValueKind == JsonValueKind.String)
                                        arr.Add(el.GetString()!);
                                value = arr;
                            }
                        }
                        break;

                    case JsonValueKind.Object:
                        // Rekursive Deserialisierung komplexer verschachtelter Objekte
                        value = JsonSerializer.Deserialize(jsonProp.Value.GetRawText(), pInfo.PropertyType, options);
                        break;
                }

                if (value != null)
                    pInfo.SetValue(instance, value);
            }
            catch
            {
                // Fehler bei Einzelproperty ignorieren (robust)
            }
        }

        return instance;
    }

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kv in _props)
        {
            var pInfo = kv.Value;
            var propVal = pInfo.GetValue(value);
            if (propVal == null) continue;

            if (_isCodeBlockLike && _subBlocksProp != null && pInfo == _subBlocksProp &&
                propVal is IList<ICodeBlock> list && list.Count > 0)
            {
                writer.WritePropertyName("SubBlocks");
                writer.WriteStartArray();
                foreach (var sb in list)
                    JsonSerializer.Serialize(writer, sb, options);
                writer.WriteEndArray();
                continue;
            }

            if (pInfo.PropertyType == typeof(string))
            {
                writer.WriteString(pInfo.Name, (string)propVal);
            }
            else if (pInfo.PropertyType.IsEnum)
            {
                writer.WriteString(pInfo.Name, propVal.ToString());
            }
            else if (propVal is int i)
            {
                writer.WriteNumber(pInfo.Name, i);
            }
            else if (propVal is bool b)
            {
                writer.WriteBoolean(pInfo.Name, b);
            }
            else if (propVal is long l)
            {
                writer.WriteNumber(pInfo.Name, l);
            }
            else if (propVal is double d)
            {
                writer.WriteNumber(pInfo.Name, d);
            }
            else if (propVal is float f)
            {
                writer.WriteNumber(pInfo.Name, f);
            }
            else if (propVal is IEnumerable<string> strEnum)
            {
                writer.WritePropertyName(pInfo.Name);
                writer.WriteStartArray();
                foreach (var s in strEnum)
                    writer.WriteStringValue(s);
                writer.WriteEndArray();
            }
            else
            {
                // Fallback für komplexe Typen
                writer.WritePropertyName(pInfo.Name);
                JsonSerializer.Serialize(writer, propVal, propVal.GetType(), options);
            }
        }

        writer.WriteEndObject();
    }
}
