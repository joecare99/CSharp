using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OFBCreator.Console.Services.Templates;

/// <summary>
/// A validated declarative entry template.
/// </summary>
public sealed class EntryTemplateDefinition
{
    public int SchemaVersion { get; set; }

    public string Id { get; set; } = string.Empty;

    public string EntryRoot { get; set; } = string.Empty;

    public Dictionary<string, List<EntryTemplateBlock>> Fragments { get; set; } = new();

    public List<EntryTemplateBlock> Blocks { get; set; } = new();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtraProperties { get; set; }
}

/// <summary>
/// One instruction in the declarative entry-template grammar.
/// </summary>
public sealed class EntryTemplateBlock
{
    public string Kind { get; set; } = string.Empty;

    public string? Role { get; set; }

    public string? Anchor { get; set; }

    public string? Value { get; set; }

    public string? Path { get; set; }

    public string? Formatter { get; set; }

    public string? Target { get; set; }

    public string? Condition { get; set; }

    public string? Items { get; set; }

    public string? As { get; set; }

    public string? Fragment { get; set; }

    public int? Columns { get; set; }

    public int? HangingIndent { get; set; }

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underline { get; set; }

    public List<EntryTemplateBlock> Content { get; set; } = new();

    public List<EntryTemplateBlock> Then { get; set; } = new();

    public List<EntryTemplateBlock> Template { get; set; } = new();

    public List<EntryTemplateBlock> Blocks { get; set; } = new();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtraProperties { get; set; }
}
