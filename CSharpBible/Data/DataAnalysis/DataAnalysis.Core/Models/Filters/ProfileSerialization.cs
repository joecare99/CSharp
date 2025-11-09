using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataAnalysis.Core.Models;

public static class ProfileSerialization
{
    private static readonly JsonSerializerOptions s_options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var o = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        // Polymorphie per Type-Discriminator "type"
        o.TypeInfoResolverChain.Insert(0, FilterDefinitionContext.Default);
        return o;
    }

    public static string ToJson(AnalysisAggregateProfile profile)
    => JsonSerializer.Serialize(profile, s_options);

    public static AnalysisAggregateProfile? FromJson(string json)
    => JsonSerializer.Deserialize<AnalysisAggregateProfile>(json, s_options);

    public static void SaveToFile(string path, AnalysisAggregateProfile profile)
    => File.WriteAllText(path, ToJson(profile));

    public static AnalysisAggregateProfile? LoadFromFile(string path)
    => File.Exists(path) ? FromJson(File.ReadAllText(path)) : null;
}

[JsonSerializable(typeof(AnalysisAggregateProfile))]
[JsonSerializable(typeof(AnalysisQuery))]
[JsonSerializable(typeof(FilterDefinition), GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(ValueFilterDefinition))]
[JsonSerializable(typeof(GroupFilterDefinition))]
internal partial class FilterDefinitionContext : JsonSerializerContext { }
