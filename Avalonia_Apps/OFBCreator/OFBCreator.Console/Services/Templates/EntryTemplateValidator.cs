using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OFBCreator.Console.Services.Templates;

/// <summary>
/// Parses and validates the safe, declarative entry-template grammar.
/// </summary>
public static class EntryTemplateValidator
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly IReadOnlyDictionary<string, string[]> AllowedProperties =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["section"] = ["kind", "columns", "blocks"],
            ["paragraph"] = ["kind", "role", "anchor", "content"],
            ["text"] = ["kind", "value"],
            ["field"] = ["kind", "path", "formatter"],
            ["link"] = ["kind", "target", "content"],
            ["if"] = ["kind", "condition", "then"],
            ["forEach"] = ["kind", "items", "as", "template"],
            ["include"] = ["kind", "fragment"]
        };

    private static readonly HashSet<string> Formatters = new(StringComparer.Ordinal)
    {
        "childCount",
        "dateSuffix",
        "gedcomDatePrefix",
        "ordinal",
        "vitalEvents"
    };

    private static readonly HashSet<string> Roles = new(StringComparer.Ordinal)
    {
        "section-heading",
        "family-header",
        "family-data",
        "adult",
        "children-header",
        "child",
        "note"
    };

    public static EntryTemplateDefinition ParseAndValidate(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        using var jsonDocument = JsonDocument.Parse(json);
        ValidateJsonShape(jsonDocument.RootElement, isBlock: false);

        var definition = JsonSerializer.Deserialize<EntryTemplateDefinition>(json, SerializerOptions)
            ?? throw new InvalidDataException("The entry template JSON did not contain an object.");
        ValidateDefinition(definition);
        return definition;
    }

    private static void ValidateJsonShape(JsonElement element, bool isBlock)
    {
        if (element.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException(isBlock
                ? "Every template block must be a JSON object."
                : "The template root must be a JSON object.");

        if (!isBlock)
        {
            var allowedRoot = new HashSet<string>(["schemaVersion", "id", "entryRoot", "fragments", "blocks"], StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
                if (!allowedRoot.Contains(property.Name))
                    throw new InvalidDataException($"Unknown template property '{property.Name}'.");

            if (!element.TryGetProperty("schemaVersion", out var schemaVersion)
                || schemaVersion.ValueKind != JsonValueKind.Number
                || !element.TryGetProperty("id", out var id)
                || id.ValueKind != JsonValueKind.String
                || !element.TryGetProperty("entryRoot", out var entryRoot)
                || entryRoot.ValueKind != JsonValueKind.String
                || !element.TryGetProperty("blocks", out var rootBlocks))
                throw new InvalidDataException("A template requires schemaVersion, id, entryRoot, and blocks.");
            ValidateBlockArray(rootBlocks);
            if (element.TryGetProperty("fragments", out var fragments))
            {
                if (fragments.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException("Template fragments must be a JSON object.");
                foreach (var fragment in fragments.EnumerateObject())
                    ValidateBlockArray(fragment.Value);
            }
            return;
        }

        var kind = element.TryGetProperty("kind", out var kindElement) && kindElement.ValueKind == JsonValueKind.String
            ? kindElement.GetString()
            : null;
        if (kind is null || !AllowedProperties.TryGetValue(kind, out var allowedProperties))
            throw new InvalidDataException($"Unknown or missing template block kind '{kind}'.");
        var allowed = new HashSet<string>(allowedProperties, StringComparer.Ordinal);
        foreach (var property in element.EnumerateObject())
        {
            if (!allowed.Contains(property.Name))
                throw new InvalidDataException($"Property '{property.Name}' is not valid on a '{kind}' block.");
            if (property.Name is "blocks" or "content" or "then" or "template")
                ValidateBlockArray(property.Value);
        }
        var requiredProperties = kind switch
        {
            "section" => new[] { "blocks" },
            "text" => new[] { "value" },
            "field" => new[] { "path" },
            "link" => new[] { "target", "content" },
            "if" => new[] { "condition", "then" },
            "forEach" => new[] { "items", "as", "template" },
            "include" => new[] { "fragment" },
            _ => Array.Empty<string>()
        };
        foreach (var requiredProperty in requiredProperties)
            if (!element.TryGetProperty(requiredProperty, out _))
                throw new InvalidDataException($"A '{kind}' block requires '{requiredProperty}'.");
    }

    private static void ValidateBlockArray(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("Template block collections must be JSON arrays.");

        foreach (var block in element.EnumerateArray())
            ValidateJsonShape(block, isBlock: true);
    }

    private static void ValidateDefinition(EntryTemplateDefinition definition)
    {
        if (definition.SchemaVersion != 1)
            throw new InvalidDataException($"Unsupported entry-template schema version {definition.SchemaVersion}.");
        if (string.IsNullOrWhiteSpace(definition.Id))
            throw new InvalidDataException("An entry template must have a non-empty id.");
        if (definition.EntryRoot is not ("Family" or "Individual"))
            throw new InvalidDataException("entryRoot must be either 'Family' or 'Individual'.");
        if (definition.Blocks is null)
            throw new InvalidDataException("The template must define a blocks array.");
        if (definition.Fragments is null)
            throw new InvalidDataException("The template fragments object cannot be null.");

        foreach (var fragment in definition.Fragments)
        {
            if (string.IsNullOrWhiteSpace(fragment.Key) || fragment.Value is null)
                throw new InvalidDataException("Template fragments must have names and block arrays.");
        }

        var rootVariables = definition.EntryRoot == "Family"
            ? new Dictionary<string, string>(StringComparer.Ordinal) { ["family"] = "family" }
            : new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["individual"] = "person",
                ["person"] = "person"
            };

        ValidateBlocks(definition.Blocks, definition, rootVariables, inSection: false, topLevel: true);
        foreach (var fragment in definition.Fragments.Values)
        {
            var fragmentVariables = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["family"] = "family",
                ["person"] = "person",
                ["child"] = "person",
                ["individual"] = "person",
                ["occupation"] = "occupation"
            };
            ValidateBlocks(fragment, definition, fragmentVariables, inSection: false, topLevel: false);
        }
        if (definition.ExtraProperties is { Count: > 0 } rootExtras)
            throw new InvalidDataException($"Unknown template property '{rootExtras.Keys.First()}'.");
        ValidateIncludeGraph(definition);
    }

    private static void ValidateBlocks(
        IReadOnlyList<EntryTemplateBlock> blocks,
        EntryTemplateDefinition definition,
        IReadOnlyDictionary<string, string> variables,
        bool inSection,
        bool topLevel)
    {
        foreach (var block in blocks)
        {
            if (block is null)
                throw new InvalidDataException("Template block collections cannot contain null values.");
            if (!AllowedProperties.TryGetValue(block.Kind, out var allowed))
                throw new InvalidDataException($"Unknown template block kind '{block.Kind}'.");

            switch (block.Kind)
            {
                case "section":
                    if (!topLevel)
                        throw new InvalidDataException("Template sections are allowed only as top-level blocks.");
                    if (inSection)
                        throw new InvalidDataException("Template sections cannot be nested.");
                    if (block.Columns is < 1 or > 4)
                        throw new InvalidDataException("A section columns value must be between 1 and 4.");
                    ValidateBlocks(block.Blocks, definition, variables, inSection: true, topLevel: false);
                    break;
                case "paragraph":
                    if (block.Role is not null && !Roles.Contains(block.Role))
                        throw new InvalidDataException($"Unknown logical style role '{block.Role}'.");
                    if (block.Anchor is not null)
                        ValidatePath(block.Anchor, variables, requireAnchor: true);
                    ValidateBlocks(block.Content, definition, variables, inSection, topLevel: false);
                    break;
                case "text":
                    if (block.Value is null)
                        throw new InvalidDataException("A text block requires a value.");
                    break;
                case "field":
                    if (string.IsNullOrWhiteSpace(block.Path))
                        throw new InvalidDataException("A field block requires a path.");
                    ValidatePath(block.Path, variables);
                    if (block.Formatter is not null && !Formatters.Contains(block.Formatter))
                        throw new InvalidDataException($"Unknown safe formatter '{block.Formatter}'.");
                    if (block.Formatter is not null && !IsFormatterCompatible(block.Path, block.Formatter))
                        throw new InvalidDataException($"Formatter '{block.Formatter}' cannot be used with path '{block.Path}'.");
                    if (block.Formatter is null && !IsScalarPath(block.Path))
                        throw new InvalidDataException($"Field path '{block.Path}' requires a compatible safe formatter.");
                    break;
                case "link":
                    if (string.IsNullOrWhiteSpace(block.Target))
                        throw new InvalidDataException("A link block requires a target path.");
                    ValidatePath(block.Target, variables, requireAnchor: true);
                    ValidateBlocks(block.Content, definition, variables, inSection, topLevel: false);
                    break;
                case "if":
                    if (block.Condition is null)
                        throw new InvalidDataException("An if block requires a condition.");
                    ValidatePath(block.Condition, variables);
                    if (!block.Condition.EndsWith(".any", StringComparison.Ordinal))
                        throw new InvalidDataException("Conditions must use a supported '.any' collection test.");
                    ValidateBlocks(block.Then, definition, variables, inSection, topLevel: false);
                    break;
                case "forEach":
                    if (string.IsNullOrWhiteSpace(block.Items) || string.IsNullOrWhiteSpace(block.As))
                        throw new InvalidDataException("A forEach block requires items and as values.");
                    ValidatePath(block.Items, variables);
                    if (!IsIdentifier(block.As))
                        throw new InvalidDataException($"Invalid forEach variable name '{block.As}'.");
                    var childVariables = new Dictionary<string, string>(variables, StringComparer.Ordinal)
                    {
                        [block.As] = GetCollectionElementType(block.Items)
                    };
                    ValidateBlocks(block.Template, definition, childVariables, inSection, topLevel: false);
                    break;
                case "include":
                    if (string.IsNullOrWhiteSpace(block.Fragment))
                        throw new InvalidDataException("An include block requires a fragment name.");
                    if (!definition.Fragments.ContainsKey(block.Fragment))
                        throw new InvalidDataException($"Unknown template fragment '{block.Fragment}'.");
                    break;
            }
        }
    }

    private static void ValidatePath(string path, IReadOnlyDictionary<string, string> variables, bool requireAnchor = false)
    {
        var parts = path.Split('.');
        if (!variables.TryGetValue(parts[0], out var type))
            throw new InvalidDataException($"Template path '{path}' does not refer to a value in the typed root model.");

        if (parts.Length == 1)
        {
            if (!requireAnchor && variables[parts[0]] is "person" or "occupation" or "family")
                return;
            throw new InvalidDataException($"Template path '{path}' must select a typed value.");
        }
        if (parts.Length < 2)
            throw new InvalidDataException($"Template path '{path}' must select a typed value.");

        var property = string.Join('.', parts.Skip(1));
        var isValid = type switch
        {
            "family" => property is "number" or "anchor" or "union" or "parents" or "children" or "children.any",
            "person" => property is "nameGc" or "nameAk" or "anchor" or "reference" or "vitalEventsGc" or "vitalEventsAk" or "birth" or "death" or "indexAnchor" or "occupations" or "occupations.any" or "ordinal",
            "occupation" => property is "name" or "date" or "indexAnchor",
            _ => false
        };
        if (!isValid)
            throw new InvalidDataException($"Unknown typed template path '{path}'.");

        if (requireAnchor && property is not ("anchor" or "indexAnchor"))
            throw new InvalidDataException($"Link target '{path}' is not an anchor field.");
    }

    private static string GetCollectionElementType(string path) => path switch
    {
        "family.parents" or "family.children" => "person",
        _ when path.EndsWith(".occupations", StringComparison.Ordinal) => "occupation",
        _ => throw new InvalidDataException($"Path '{path}' is not a supported template collection.")
    };

    private static bool IsFormatterCompatible(string path, string formatter) => formatter switch
    {
        "childCount" => path == "family.children",
        "dateSuffix" => path is "family.union" or "person.birth" or "child.birth" or "individual.birth"
            or "person.death" or "child.death" or "individual.death",
        "gedcomDatePrefix" => path.EndsWith(".date", StringComparison.Ordinal),
        "ordinal" => path.EndsWith(".ordinal", StringComparison.Ordinal),
        "vitalEvents" => path is "person" or "individual" or "child",
        _ => false
    };

    private static bool IsScalarPath(string path) => path switch
    {
        "family.number" or "family.anchor" or "family.union" => true,
        "person.nameGc" or "person.nameAk" or "person.anchor" or "person.reference"
            or "person.vitalEventsGc" or "person.vitalEventsAk" or "person.birth" or "person.death"
            or "person.indexAnchor" or "person.ordinal" => true,
        "child.nameGc" or "child.nameAk" or "child.anchor" or "child.reference"
            or "child.vitalEventsGc" or "child.vitalEventsAk" or "child.birth" or "child.death"
            or "child.indexAnchor" or "child.ordinal" => true,
        "individual.nameGc" or "individual.nameAk" or "individual.anchor" or "individual.reference"
            or "individual.vitalEventsGc" or "individual.vitalEventsAk" or "individual.birth"
            or "individual.death" or "individual.indexAnchor" or "individual.ordinal" => true,
        "occupation.name" or "occupation.date" or "occupation.indexAnchor" => true,
        _ => false
    };

    private static bool IsIdentifier(string name) =>
        name.Length > 0
        && (char.IsLetter(name[0]) || name[0] == '_')
        && name.Skip(1).All(character => char.IsLetterOrDigit(character) || character == '_');

    private static void ValidateIncludeGraph(EntryTemplateDefinition definition)
    {
        var visiting = new HashSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<string>(StringComparer.Ordinal);

        foreach (var fragmentName in definition.Fragments.Keys)
            Visit(fragmentName);

        void Visit(string fragmentName)
        {
            if (visited.Contains(fragmentName))
                return;
            if (!visiting.Add(fragmentName))
                throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");

            foreach (var include in EnumerateIncludes(definition.Fragments[fragmentName]))
                Visit(include);

            visiting.Remove(fragmentName);
            visited.Add(fragmentName);
        }

        IEnumerable<string> EnumerateIncludes(IEnumerable<EntryTemplateBlock> blocks)
        {
            foreach (var block in blocks)
            {
                if (block.Kind == "include")
                    yield return block.Fragment!;
                foreach (var nested in block.Content.Concat(block.Then).Concat(block.Template).Concat(block.Blocks))
                {
                    foreach (var include in EnumerateIncludes([nested]))
                        yield return include;
                }
            }
        }
    }
}
