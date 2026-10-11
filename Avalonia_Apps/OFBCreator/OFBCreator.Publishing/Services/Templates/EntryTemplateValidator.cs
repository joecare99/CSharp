using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GenInterfaces.Data;
using OFBCreator.Publishing.Models;

namespace OFBCreator.Publishing.Services.Templates;

/// <summary>
/// Parses and validates the safe, declarative entry-template grammar.
/// </summary>
public static class EntryTemplateValidator
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly IReadOnlyDictionary<string, string[]> AllowedProperties =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [EntryTemplateContract.Block.Section] = ["kind", "columns", "blocks"],
            // Keep "indent" explicitly whitelisted for paragraph blocks (no-op guard).
            [EntryTemplateContract.Block.Paragraph] = ["kind", "role", "anchor", "content", "indent", "hangingIndent", "spaceBefore"],
            // Allow inline line breaks so non-vital events can render line-by-line within one paragraph.
            [EntryTemplateContract.Block.Text] = ["kind", "value", "bold", "italic", "underline", "lineBreakBefore"],
            [EntryTemplateContract.Block.Field] = ["kind", "path", "formatter", "bold", "italic", "underline", "lineBreakBefore"],
            [EntryTemplateContract.Block.Link] = ["kind", "target", "content", "bold", "italic", "underline", "lineBreakBefore"],
            [EntryTemplateContract.Block.If] = ["kind", "condition", "then"],
            [EntryTemplateContract.Block.ForEach] = ["kind", "items", "as", "template"],
            [EntryTemplateContract.Block.Include] = ["kind", "fragment", "context"]
        };

    private static readonly HashSet<string> Formatters = new(StringComparer.Ordinal)
    {
        EntryTemplateContract.Formatter.ChildCount,
        EntryTemplateContract.Formatter.DateSuffix,
        EntryTemplateContract.Formatter.GedcomDatePrefix,
        EntryTemplateContract.Formatter.Ordinal,
        EntryTemplateContract.Formatter.VitalEvents
    };

    private static readonly HashSet<string> Roles = new(StringComparer.Ordinal)
    {
        EntryTemplateContract.Role.SectionHeading,
        EntryTemplateContract.Role.FamilyHeader,
        EntryTemplateContract.Role.FamilyData,
        EntryTemplateContract.Role.Adult,
        EntryTemplateContract.Role.ChildrenHeader,
        EntryTemplateContract.Role.Child,
        EntryTemplateContract.Role.Note,
        EntryTemplateContract.Role.FamilyEntry,
        EntryTemplateContract.Role.ParentEntry,
        EntryTemplateContract.Role.ParentLifeEvents,
        EntryTemplateContract.Role.ChildHeader,
        EntryTemplateContract.Role.ChildEntry,
        EntryTemplateContract.Role.ChildLifeEvents,
        EntryTemplateContract.Role.IndexEntry
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
            var allowedRoot = new HashSet<string>(["schemaVersion", "id", "entryRoot", "fragments", "blocks", "legend"], StringComparer.Ordinal);
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
            if (element.TryGetProperty("legend", out var legend))
                ValidateLegendShape(legend);
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
            EntryTemplateContract.Block.Section => new[] { "blocks" },
            EntryTemplateContract.Block.Text => new[] { "value" },
            EntryTemplateContract.Block.Field => new[] { "path" },
            EntryTemplateContract.Block.Link => new[] { "target", "content" },
            EntryTemplateContract.Block.If => new[] { "condition", "then" },
            EntryTemplateContract.Block.ForEach => new[] { "items", "as", "template" },
            EntryTemplateContract.Block.Include => new[] { "fragment" },
            _ => Array.Empty<string>()
        };
        foreach (var requiredProperty in requiredProperties)
            if (!element.TryGetProperty(requiredProperty, out _))
                throw new InvalidDataException($"A '{kind}' block requires '{requiredProperty}'.");
    }

    private static void ValidateLegendShape(JsonElement legend)
    {
        if (legend.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("Template legend must be an array.");

        foreach (var entry in legend.EnumerateArray())
        {
            if (entry.ValueKind != JsonValueKind.Object)
                throw new InvalidDataException("Every legend entry must be an object.");
            foreach (var property in entry.EnumerateObject())
            {
                if (property.Name is not ("symbol" or "meaning" or "event" or "placePreposition"))
                    throw new InvalidDataException($"Unknown legend property '{property.Name}'.");
                if ((property.Name is "symbol" or "meaning" or "placePreposition")
                    && property.Value.ValueKind is not (JsonValueKind.String or JsonValueKind.Null))
                    throw new InvalidDataException($"Legend property '{property.Name}' must be a string.");
                if (property.Name == "event"
                    && property.Value.ValueKind is not (JsonValueKind.String or JsonValueKind.Null))
                    throw new InvalidDataException("A legend event must be an EFactType name or null.");
                if (property.Name == "event" && property.Value.ValueKind == JsonValueKind.String
                    && (!Enum.TryParse<EFactType>(property.Value.GetString(), ignoreCase: false, out var eventType)
                        || !Enum.IsDefined(eventType)
                        || eventType == EFactType.Reference))
                    throw new InvalidDataException($"Unknown legend event '{property.Value.GetString()}'.");
            }
        }
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
        if (definition.EntryRoot is not (EntryTemplateContract.EntryRoot.Family or EntryTemplateContract.EntryRoot.Individual))
            throw new InvalidDataException("entryRoot must be either 'Family' or 'Individual'.");
        if (definition.Blocks is null)
            throw new InvalidDataException("The template must define a blocks array.");
        if (definition.Fragments is null)
            throw new InvalidDataException("The template fragments object cannot be null.");
        if (definition.Legend is null)
            throw new InvalidDataException("The template legend cannot be null.");
        if (definition.Legend.Any(entry => entry is null
            || string.IsNullOrWhiteSpace(entry.Symbol)
            || string.IsNullOrWhiteSpace(entry.Meaning)))
            throw new InvalidDataException("Legend entries require non-empty symbol and meaning values.");
        if (definition.Legend.Any(entry => entry.Event == EFactType.Reference))
            throw new InvalidDataException("Legend event values cannot be reference metadata.");

        foreach (var fragment in definition.Fragments)
        {
            if (string.IsNullOrWhiteSpace(fragment.Key) || fragment.Value is null)
                throw new InvalidDataException("Template fragments must have names and block arrays.");
        }

        var rootVariables = definition.EntryRoot == EntryTemplateContract.EntryRoot.Family
            ? new Dictionary<string, string>(StringComparer.Ordinal) { [EntryTemplateContract.Variable.Family] = EntryTemplateContract.Model.Family }
            : new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [EntryTemplateContract.Variable.Individual] = EntryTemplateContract.Model.Person,
                [EntryTemplateContract.Variable.Person] = EntryTemplateContract.Model.Person
            };

        ValidateBlocks(definition.Blocks, definition, rootVariables, inSection: false, topLevel: true);
        foreach (var fragment in definition.Fragments.Values)
        {
            var fragmentVariables = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [EntryTemplateContract.Variable.Family] = EntryTemplateContract.Model.Family,
                [EntryTemplateContract.Variable.Person] = EntryTemplateContract.Model.Person,
                [EntryTemplateContract.Variable.Child] = EntryTemplateContract.Model.Person,
                [EntryTemplateContract.Variable.Individual] = EntryTemplateContract.Model.Person,
                [EntryTemplateContract.Variable.Occupation] = EntryTemplateContract.Model.Occupation,
                [EntryTemplateContract.Variable.Property] = EntryTemplateContract.Model.Property,
                [EntryTemplateContract.Variable.FamilyReference] = EntryTemplateContract.Model.FamilyReference,
                [EntryTemplateContract.Variable.FamilyReferenceToken] = EntryTemplateContract.Model.FamilyReferenceToken,
                [EntryTemplateContract.Variable.PersonEvent] = EntryTemplateContract.Model.PersonEvent
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
                case EntryTemplateContract.Block.Section:
                    if (!topLevel)
                        throw new InvalidDataException("Template sections are allowed only as top-level blocks.");
                    if (inSection)
                        throw new InvalidDataException("Template sections cannot be nested.");
                    if (block.Columns is < 1 or > 4)
                        throw new InvalidDataException("A section columns value must be between 1 and 4.");
                    ValidateBlocks(block.Blocks, definition, variables, inSection: true, topLevel: false);
                    break;
                case EntryTemplateContract.Block.Paragraph:
                    if (block.Role is not null && !Roles.Contains(block.Role))
                        throw new InvalidDataException($"Unknown logical style role '{block.Role}'.");
                    if (block.Anchor is not null)
                        ValidatePath(block.Anchor, variables, requireAnchor: true);
                    if (block.HangingIndent is < 0 or > 1440)
                        throw new InvalidDataException("A paragraph hangingIndent value must be between 0 and 1440 points.");
                    if (block.Indent is < 0 or > 1440)
                        throw new InvalidDataException("A paragraph indent value must be between 0 and 1440 points.");
                    if (block.SpaceBefore is < 0 or > 1440)
                        throw new InvalidDataException("A paragraph spaceBefore value must be between 0 and 1440 points.");
                    ValidateBlocks(block.Content, definition, variables, inSection, topLevel: false);
                    break;
                case EntryTemplateContract.Block.Text:
                    if (block.Value is null)
                        throw new InvalidDataException("A text block requires a value.");
                    break;
                case EntryTemplateContract.Block.Field:
                    if (string.IsNullOrWhiteSpace(block.Path))
                        throw new InvalidDataException("A field block requires a path.");
                    ValidatePath(block.Path, variables);
                    var canonicalPath = CanonicalizePath(block.Path, variables);
                    if (block.Formatter is not null && !Formatters.Contains(block.Formatter))
                        throw new InvalidDataException($"Unknown safe formatter '{block.Formatter}'.");
                    if (block.Formatter is not null && !IsFormatterCompatible(canonicalPath, block.Formatter))
                        throw new InvalidDataException($"Formatter '{block.Formatter}' cannot be used with path '{block.Path}'.");
                    if (block.Formatter is null && !IsScalarPath(canonicalPath))
                        throw new InvalidDataException($"Field path '{block.Path}' requires a compatible safe formatter.");
                    break;
                case EntryTemplateContract.Block.Link:
                    if (string.IsNullOrWhiteSpace(block.Target))
                        throw new InvalidDataException("A link block requires a target path.");
                    ValidatePath(block.Target, variables, requireAnchor: true);
                    ValidateBlocks(block.Content, definition, variables, inSection, topLevel: false);
                    break;
                case EntryTemplateContract.Block.If:
                    if (block.Condition is null)
                        throw new InvalidDataException("An if block requires a condition.");
                    var conditionPath = block.Condition.StartsWith('!') ? block.Condition[1..] : block.Condition;
                    if (string.IsNullOrWhiteSpace(conditionPath) || conditionPath.StartsWith('!'))
                        throw new InvalidDataException("A condition may have only one leading '!' negation.");
                    ValidatePath(conditionPath, variables);
                    if (!IsSupportedCondition(conditionPath, variables))
                        throw new InvalidDataException("Conditions must use a supported collection test or optional typed value.");
                    ValidateBlocks(block.Then, definition, variables, inSection, topLevel: false);
                    break;
                case EntryTemplateContract.Block.ForEach:
                    if (string.IsNullOrWhiteSpace(block.Items) || string.IsNullOrWhiteSpace(block.As))
                        throw new InvalidDataException("A forEach block requires items and as values.");
                    ValidatePath(block.Items, variables);
                    if (!IsIdentifier(block.As))
                        throw new InvalidDataException($"Invalid forEach variable name '{block.As}'.");
                    var childVariables = new Dictionary<string, string>(variables, StringComparer.Ordinal)
                    {
                        [block.As] = GetCollectionElementType(block.Items, variables)
                    };
                    ValidateBlocks(block.Template, definition, childVariables, inSection, topLevel: false);
                    break;
                case EntryTemplateContract.Block.Include:
                    if (string.IsNullOrWhiteSpace(block.Fragment))
                        throw new InvalidDataException("An include block requires a fragment name.");
                    if (!definition.Fragments.ContainsKey(block.Fragment))
                        throw new InvalidDataException($"Unknown template fragment '{block.Fragment}'.");
                    if (block.Context is not null)
                    {
                        ValidatePath(block.Context, variables);
                        if (CanonicalizePath(block.Context, variables) != EntryTemplateContract.Model.Person)
                            throw new InvalidDataException("An include context must resolve to a person value.");
                    }
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
            if (!requireAnchor && EntryTemplateContract.GetModelType(type) is not null)
                return;
            throw new InvalidDataException($"Template path '{path}' must select a typed value.");
        }
        var property = GetPathProperty(path, variables);
        if (property is null)
            throw new InvalidDataException($"Unknown typed template path '{path}'.");
        if (requireAnchor
            && (property.PropertyType != typeof(string)
                || !property.Name.EndsWith("Anchor", StringComparison.Ordinal)))
            throw new InvalidDataException($"Link target '{path}' is not an anchor field.");
    }

    private static string CanonicalizePath(string path, IReadOnlyDictionary<string, string> variables)
    {
        var parts = path.Split('.');
        if (parts.Length == 0 || !variables.TryGetValue(parts[0], out var type))
            return path;

        return parts.Length == 1
            ? type
            : $"{type}.{string.Join('.', parts.Skip(1))}";
    }

    private static string GetCollectionElementType(string path, IReadOnlyDictionary<string, string> variables)
    {
        var property = GetPathProperty(path, variables);
        if (property is null
            || !EntryTemplateContract.TryGetCollectionElementType(property.PropertyType, out var elementType)
            || elementType is null)
            throw new InvalidDataException($"Path '{path}' is not a supported template collection.");
        return EntryTemplateContract.GetModelName(elementType)
            ?? throw new InvalidDataException($"Path '{path}' does not contain a supported template model.");
    }

    private static bool IsFormatterCompatible(string path, string formatter)
    {
        var models = GetAllModelVariables();
        if (path.Split('.').Length == 1)
            return formatter == EntryTemplateContract.Formatter.VitalEvents
                && models.TryGetValue(path, out var modelName)
                && EntryTemplateContract.GetModelType(modelName) == typeof(PersonEntryTemplateModel);

        var property = GetPathProperty(path, models);
        return formatter switch
        {
            EntryTemplateContract.Formatter.ChildCount => property?.Name == nameof(FamilyEntryTemplateModel.Children),
            EntryTemplateContract.Formatter.DateSuffix => property?.Name is nameof(FamilyEntryTemplateModel.Union)
                or nameof(FamilyEntryTemplateModel.MarriageDate)
                or nameof(PersonEntryTemplateModel.Birth)
                or nameof(PersonEntryTemplateModel.Death),
            EntryTemplateContract.Formatter.GedcomDatePrefix => property?.Name is nameof(PersonEventEntryTemplateModel.Date)
                or nameof(OccupationEntryTemplateModel.Date)
                or nameof(PropertyEntryTemplateModel.Date),
            EntryTemplateContract.Formatter.Ordinal => property?.Name == nameof(PersonEntryTemplateModel.Ordinal),
            _ => false
        };
    }

    private static bool IsScalarPath(string path)
    {
        var property = GetPathProperty(path, GetAllModelVariables());
        if (property is null)
            return false;

        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        return EntryTemplateContract.IsScalar(propertyType)
            || EntryTemplateContract.GetModelName(propertyType) == EntryTemplateContract.Model.FamilyReference;
    }

    private static bool IsSupportedCondition(string path, IReadOnlyDictionary<string, string> variables)
    {
        if (path.EndsWith(".any", StringComparison.Ordinal) || path.EndsWith(".one", StringComparison.Ordinal))
            return IsSupportedAnyConditionPath(path, variables);

        var property = GetPathProperty(path, variables);
        return property is not null
            && !EntryTemplateContract.TryGetCollectionElementType(property.PropertyType, out _)
            && (EntryTemplateContract.IsScalar(property.PropertyType)
                || EntryTemplateContract.GetModelName(Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType) is not null);
    }

    private static bool IsSupportedAnyConditionPath(string path, IReadOnlyDictionary<string, string> variables)
    {
        if (!path.EndsWith(".any", StringComparison.Ordinal) && !path.EndsWith(".one", StringComparison.Ordinal))
            return false;

        var collectionPath = path[..path.LastIndexOf('.')];
        var property = GetPathProperty(collectionPath, variables);
        return property is not null
            && EntryTemplateContract.TryGetCollectionElementType(property.PropertyType, out _);
    }

    private static PropertyInfo? GetPathProperty(string path, IReadOnlyDictionary<string, string> variables)
    {
        var parts = path.Split('.');
        if (parts.Length < 2 || !variables.TryGetValue(parts[0], out var modelName))
            return null;

        var modelType = EntryTemplateContract.GetModelType(modelName);
        PropertyInfo? property = null;
        for (var index = 1; index < parts.Length; index++)
        {
            if (parts[index] is "any" or "one")
                return index == parts.Length - 1 ? property : null;
            if (modelType is null)
                return null;

            property = EntryTemplateContract.GetProperty(modelType, parts[index]);
            if (property is null)
                return null;
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (EntryTemplateContract.TryGetCollectionElementType(propertyType, out var elementType))
                modelType = elementType is not null && EntryTemplateContract.GetModelName(elementType) is not null ? elementType : null;
            else
                modelType = EntryTemplateContract.GetModelName(propertyType) is not null ? propertyType : null;
        }

        return property;
    }

    private static IReadOnlyDictionary<string, string> GetAllModelVariables() =>
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [EntryTemplateContract.Variable.Family] = EntryTemplateContract.Model.Family,
            [EntryTemplateContract.Variable.Person] = EntryTemplateContract.Model.Person,
            [EntryTemplateContract.Variable.Child] = EntryTemplateContract.Model.Person,
            [EntryTemplateContract.Variable.Individual] = EntryTemplateContract.Model.Person,
            [EntryTemplateContract.Variable.Occupation] = EntryTemplateContract.Model.Occupation,
            [EntryTemplateContract.Variable.Property] = EntryTemplateContract.Model.Property,
            [EntryTemplateContract.Variable.FamilyReference] = EntryTemplateContract.Model.FamilyReference,
            [EntryTemplateContract.Variable.FamilyReferenceToken] = EntryTemplateContract.Model.FamilyReferenceToken,
            [EntryTemplateContract.Variable.PersonEvent] = EntryTemplateContract.Model.PersonEvent
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
                if (block.Kind == EntryTemplateContract.Block.Include)
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
