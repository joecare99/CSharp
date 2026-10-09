using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Document.Base.Models.Interfaces;
using Document.Base.Models;
using OFBCreator.Console.Models;

namespace OFBCreator.Console.Services.Templates;

/// <summary>
/// Renders validated entry templates without evaluating executable expressions.
/// </summary>
public sealed class EntryTemplateRenderer
{
    public void RenderFamily(
        IUserDocument document,
        EntryTemplateDefinition template,
        FamilyEntryTemplateModel family)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(family);
        if (!string.Equals(template.EntryRoot, "Family", StringComparison.Ordinal))
            throw new InvalidDataException(
                $"Template '{template.Id}' has root '{template.EntryRoot}', but a Family entry was supplied.");

        RenderRoot(document, template, new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["family"] = family
        });
        EnsurePersonNavigation(document, family.Parents.Concat(family.Children));
    }

    public void RenderIndividual(
        IUserDocument document,
        EntryTemplateDefinition template,
        PersonEntryTemplateModel individual)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(individual);
        if (!string.Equals(template.EntryRoot, "Individual", StringComparison.Ordinal))
            throw new InvalidDataException(
                $"Template '{template.Id}' has root '{template.EntryRoot}', but an Individual entry was supplied.");

        RenderRoot(document, template, new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["individual"] = individual,
            ["person"] = individual
        });
        EnsurePersonNavigation(document, [individual]);
    }

    public void EnsureNavigation(
        IUserDocument document,
        IEnumerable<PersonEntryTemplateModel> people)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(people);
        EnsurePersonNavigation(document, people);
    }

    private static void EnsurePersonNavigation(
        IUserDocument document,
        IEnumerable<PersonEntryTemplateModel> people)
    {
        var spans = document.Enumerate().OfType<IDocSpan>().ToArray();
        var bookmarks = spans
            .Where(span => !string.IsNullOrWhiteSpace(span.Id))
            .Select(span => span.Id!)
            .ToHashSet(StringComparer.Ordinal);
        var linkTargets = spans
            .Where(span => span.IsLink && span.Href?.StartsWith("#", StringComparison.Ordinal) == true)
            .Select(span => span.Href![1..])
            .ToHashSet(StringComparer.Ordinal);

        foreach (var person in people)
        {
            if (!string.IsNullOrWhiteSpace(person.Anchor) && bookmarks.Add(person.Anchor))
            {
                var anchorParagraph = document.AddParagraph("adult");
                anchorParagraph.AddBookmark(person.Anchor, DocFontStyle.Default);
            }

            if (!string.IsNullOrWhiteSpace(person.IndexAnchor) && linkTargets.Add(person.IndexAnchor))
            {
                var indexParagraph = document.AddParagraph("note");
                var link = indexParagraph.AddLink($"#{person.IndexAnchor}", new DocFontStyle { Underline = true });
                link.TextContent = " [Index]";
            }
        }
    }

    private static void RenderRoot(
        IUserDocument document,
        EntryTemplateDefinition template,
        Dictionary<string, object?> variables)
    {
        RenderBlocks(document, template, template.Blocks, variables, new HashSet<string>(StringComparer.Ordinal));
    }

    private static void RenderBlocks(
        IUserDocument document,
        EntryTemplateDefinition template,
        IEnumerable<EntryTemplateBlock> blocks,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        foreach (var block in blocks)
        {
            switch (block.Kind)
            {
                case "section":
                    if (document is not IDocSectionProvider sectionProvider)
                        throw new NotSupportedException("The selected document provider does not support sections.");
                    sectionProvider.AddSection(block.Columns);
                    RenderBlocks(document, template, block.Blocks, variables, includeStack);
                    break;
                case "paragraph":
                    var paragraph = document.AddParagraph(block.Role ?? "family-data");
                    if (block.Indent.HasValue || block.HangingIndent.HasValue)
                    {
                        var indentationBefore = block.Indent ?? block.HangingIndent.GetValueOrDefault();
                        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.IndentationBefore, indentationBefore));
                    }
                    if (block.HangingIndent is int hangingIndent)
                    {
                        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.IndentationHanging, hangingIndent));
                    }
                    if (block.Anchor is not null)
                    {
                        var anchor = GetString(Resolve(block.Anchor, variables));
                        if (!string.IsNullOrWhiteSpace(anchor))
                        {
                            var bookmark = paragraph.AddBookmark(anchor, DocFontStyle.Default);
                            bookmark.TextContent = string.Empty;
                        }
                    }
                    RenderInline(document, template, paragraph, block.Content, variables, includeStack);
                    break;
                case "if":
                    if (IsConditionMet(block.Condition!, variables))
                        RenderBlocks(document, template, block.Then, variables, includeStack);
                    break;
                case "forEach":
                    RenderEach(document, template, block, variables, includeStack);
                    break;
                case "include":
                    RenderInclude(document, template, block.Fragment!, variables, includeStack);
                    break;
                default:
                    throw new InvalidDataException($"Block '{block.Kind}' is not valid at template block level.");
            }
        }
    }

    private static void RenderInline(
        IUserDocument document,
        EntryTemplateDefinition template,
        IDocParagraph paragraph,
        IEnumerable<EntryTemplateBlock> blocks,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        foreach (var block in blocks)
        {
            switch (block.Kind)
            {
                case "text":
                    AddLineBreakIfRequested(paragraph, block);
                    AppendText(paragraph, block.Value!, block);
                    break;
                case "field":
                    AddLineBreakIfRequested(paragraph, block);
                    AppendText(paragraph, Format(Resolve(block.Path!, variables), block.Formatter), block);
                    break;
                case "link":
                    AddLineBreakIfRequested(paragraph, block);
                    var target = GetString(Resolve(block.Target!, variables));
                    var text = RenderInlineToString(template, block.Content, variables, includeStack);
                    if (string.IsNullOrWhiteSpace(target))
                        AppendText(paragraph, text, block);
                    else
                    {
                        var link = paragraph.AddLink($"#{target}", new DocFontStyle { Underline = true });
                        link.SetStyle(CreateFontStyle(block, underline: true));
                        link.TextContent = text;
                    }
                    break;
                case "if":
                    if (IsConditionMet(block.Condition!, variables))
                        RenderInline(document, template, paragraph, block.Then, variables, includeStack);
                    break;
                case "forEach":
                    RenderEachInline(document, template, paragraph, block, variables, includeStack);
                    break;
                case "include":
                    RenderIncludeInline(document, template, paragraph, block.Fragment!, variables, includeStack);
                    break;
                default:
                    throw new InvalidDataException($"Block '{block.Kind}' is not valid inside paragraph content.");
            }
        }
    }

    private static void AddLineBreakIfRequested(IDocParagraph paragraph, EntryTemplateBlock block)
    {
        if (block.LineBreakBefore)
            paragraph.AddLineBreak();
    }

    private static string RenderInlineToString(
        EntryTemplateDefinition template,
        IEnumerable<EntryTemplateBlock> blocks,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        var values = new List<string>();
        foreach (var block in blocks)
        {
            switch (block.Kind)
            {
                case "text":
                    values.Add(block.Value!);
                    break;
                case "field":
                    values.Add(Format(Resolve(block.Path!, variables), block.Formatter));
                    break;
                case "if":
                    if (IsConditionMet(block.Condition!, variables))
                        values.Add(RenderInlineToString(template, block.Then, variables, includeStack));
                    break;
                case "forEach":
                    values.Add(RenderEachInlineToString(template, block, variables, includeStack));
                    break;
                case "include":
                    values.Add(RenderIncludeInlineToString(template, block.Fragment!, variables, includeStack));
                    break;
                default:
                    throw new InvalidDataException($"Block '{block.Kind}' cannot be nested inside a link.");
            }
        }

        return string.Concat(values);
    }

    private static string RenderEachInlineToString(
        EntryTemplateDefinition template,
        EntryTemplateBlock block,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        var values = new List<string>();
        foreach (var item in Enumerate(Resolve(block.Items!, variables)))
        {
            var hadPriorValue = variables.TryGetValue(block.As!, out var priorValue);
            variables[block.As!] = item;
            values.Add(RenderInlineToString(template, block.Template, variables, includeStack));
            RestoreVariable(variables, block.As!, hadPriorValue, priorValue);
        }

        return string.Concat(values);
    }

    private static string RenderIncludeInlineToString(
        EntryTemplateDefinition template,
        string fragmentName,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        if (!includeStack.Add(fragmentName))
            throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");
        try
        {
            return RenderInlineToString(template, template.Fragments[fragmentName], variables, includeStack);
        }
        finally
        {
            includeStack.Remove(fragmentName);
        }
    }

    private static void RenderEach(
        IUserDocument document,
        EntryTemplateDefinition template,
        EntryTemplateBlock block,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        foreach (var item in Enumerate(Resolve(block.Items!, variables)))
        {
            var hadPriorValue = variables.TryGetValue(block.As!, out var priorValue);
            variables[block.As!] = item;
            RenderBlocks(document, template, block.Template, variables, includeStack);
            RestoreVariable(variables, block.As!, hadPriorValue, priorValue);
        }
    }

    private static void RenderEachInline(
        IUserDocument document,
        EntryTemplateDefinition template,
        IDocParagraph paragraph,
        EntryTemplateBlock block,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        foreach (var item in Enumerate(Resolve(block.Items!, variables)))
        {
            var hadPriorValue = variables.TryGetValue(block.As!, out var priorValue);
            variables[block.As!] = item;
            RenderInline(document, template, paragraph, block.Template, variables, includeStack);
            RestoreVariable(variables, block.As!, hadPriorValue, priorValue);
        }
    }

    private static void RenderInclude(
        IUserDocument document,
        EntryTemplateDefinition template,
        string fragmentName,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        if (!includeStack.Add(fragmentName))
            throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");
        try
        {
            RenderBlocks(document, template, template.Fragments[fragmentName], variables, includeStack);
        }
        finally
        {
            includeStack.Remove(fragmentName);
        }
    }

    private static void RenderIncludeInline(
        IUserDocument document,
        EntryTemplateDefinition template,
        IDocParagraph paragraph,
        string fragmentName,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        if (!includeStack.Add(fragmentName))
            throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");
        try
        {
            RenderInline(document, template, paragraph, template.Fragments[fragmentName], variables, includeStack);
        }
        finally
        {
            includeStack.Remove(fragmentName);
        }
    }

    private static object? Resolve(string path, IReadOnlyDictionary<string, object?> variables)
    {
        var parts = path.Split('.');
        if (!variables.TryGetValue(parts[0], out var value))
            throw new InvalidDataException($"Template value '{parts[0]}' is not available in this rendering context.");

        for (var index = 1; index < parts.Length; index++)
        {
            if (parts[index] is "any" or "one")
            {
                value = value switch
                {
                    ICollection collection when parts[index] == "any" => collection.Count > 0,
                    ICollection collection when parts[index] == "one" => collection.Count == 1,
                    IEnumerable enumerable when value is not string && parts[index] == "any"
                        => enumerable.Cast<object?>().Any(),
                    IEnumerable enumerable when value is not string && parts[index] == "one"
                        => enumerable.Cast<object?>().Take(2).Count() == 1,
                    _ => throw new InvalidDataException($"Template value '{path}' is not available in this rendering context.")
                };
                continue;
            }

            value = (value, parts[index]) switch
            {
                (FamilyEntryTemplateModel family, "number") => family.Number,
                (FamilyEntryTemplateModel family, "anchor") => family.Anchor,
                (FamilyEntryTemplateModel family, "union") => family.Union,
                (FamilyEntryTemplateModel family, "marriageMark") => family.MarriageMark,
                (FamilyEntryTemplateModel family, "marriageDate") => family.MarriageDate,
                (FamilyEntryTemplateModel family, "marriagePlace") => family.MarriagePlace,
                (FamilyEntryTemplateModel family, "marriagePlaceShort") => family.MarriagePlaceShort,
                (FamilyEntryTemplateModel family, "marriagePlaceAnchor") => family.MarriagePlaceAnchor,
                (FamilyEntryTemplateModel family, "properties") => family.Properties,
                (FamilyEntryTemplateModel family, "parents") => family.Parents,
                (FamilyEntryTemplateModel family, "children") => family.Children,
                (PersonEntryTemplateModel person, "nameGc") => person.NameGc,
                (PersonEntryTemplateModel person, "nameAk") => person.NameAk,
                (PersonEntryTemplateModel person, "akaNames") => person.AkaNames,
                (PersonEntryTemplateModel person, "anchor") => person.Anchor,
                (PersonEntryTemplateModel person, "reference") => person.Reference,
                (PersonEntryTemplateModel person, "referenceNumber") => person.ReferenceNumber,
                (PersonEntryTemplateModel person, "events") => person.Events,
                (PersonEntryTemplateModel person, "indexLabel") => person.IndexLabel,
                (PersonEntryTemplateModel person, "vitalEventsGc") => person.VitalEventsGc,
                (PersonEntryTemplateModel person, "additionalLifeDataGc") => person.AdditionalLifeDataGc,
                (PersonEntryTemplateModel person, "vitalEventsAk") => person.VitalEventsAk,
                (PersonEntryTemplateModel person, "birth") => person.Birth,
                (PersonEntryTemplateModel person, "death") => person.Death,
                (PersonEntryTemplateModel person, "indexAnchor") => person.IndexAnchor,
                (PersonEntryTemplateModel person, "occupations") => person.Occupations,
                (PersonEntryTemplateModel person, "properties") => person.Properties,
                (PersonEntryTemplateModel person, "residence") => person.Residence,
                (PersonEntryTemplateModel person, "residenceAnchor") => person.ResidenceAnchor,
                (PersonEntryTemplateModel person, "parentFamily") => person.ParentFamily,
                (PersonEntryTemplateModel person, "childFamilies") => person.ChildFamilies,
                (PersonEntryTemplateModel person, "parentFamilies") => person.ParentFamilies,
                (PersonEntryTemplateModel person, "childFamilyTokens") => person.ChildFamilyTokens,
                (PersonEntryTemplateModel person, "parentFamilyTokens") => person.ParentFamilyTokens,
                (FamilyReferenceTokenModel token, "text") => token.Text,
                (FamilyReferenceTokenModel token, "anchor") => token.Anchor,
                (PersonEntryTemplateModel person, "ordinal") => person.Ordinal,
                (FamilyReferenceEntryTemplateModel familyReference, "number") => familyReference.Number,
                (FamilyReferenceEntryTemplateModel familyReference, "anchor") => familyReference.Anchor,
                (OccupationEntryTemplateModel occupation, "name") => occupation.Name,
                (OccupationEntryTemplateModel occupation, "date") => occupation.Date,
                (OccupationEntryTemplateModel occupation, "indexAnchor") => occupation.IndexAnchor,
                (OccupationEntryTemplateModel occupation, "place") => occupation.Place,
                (OccupationEntryTemplateModel occupation, "placeAnchor") => occupation.PlaceAnchor,
                (PropertyEntryTemplateModel property, "name") => property.Name,
                (PropertyEntryTemplateModel property, "date") => property.Date,
                (PropertyEntryTemplateModel property, "place") => property.Place,
                (PropertyEntryTemplateModel property, "placeAnchor") => property.PlaceAnchor,
                (PropertyEntryTemplateModel property, "indexAnchor") => property.IndexAnchor,
                (PersonEventEntryTemplateModel personEvent, "symbol") => personEvent.Symbol,
                (PersonEventEntryTemplateModel personEvent, "isVital") => personEvent.IsVital,
                (PersonEventEntryTemplateModel personEvent, "relatedPerson") => personEvent.HasRelatedPerson,
                (PersonEventEntryTemplateModel personEvent, "relatedFamily") => personEvent.HasRelatedFamily,
                (PersonEventEntryTemplateModel personEvent, "date") => personEvent.Date,
                (PersonEventEntryTemplateModel personEvent, "place") => personEvent.Place,
                (PersonEventEntryTemplateModel personEvent, "placeAnchor") => personEvent.PlaceAnchor,
                (PersonEventEntryTemplateModel personEvent, "additional") => personEvent.Additional,
                (PersonEventEntryTemplateModel personEvent, "relatedPersonName") => personEvent.RelatedPersonName,
                (PersonEventEntryTemplateModel personEvent, "relatedPersonAnchor") => personEvent.RelatedPersonAnchor,
                (PersonEventEntryTemplateModel personEvent, "relatedFamilyNumber") => personEvent.RelatedFamilyNumber,
                (PersonEventEntryTemplateModel personEvent, "relatedFamilyAnchor") => personEvent.RelatedFamilyAnchor,
                _ => throw new InvalidDataException($"Template value '{path}' is not available in this rendering context.")
            };
        }

        return value;
    }

    private static string Format(object? value, string? formatter)
    {
        if (value is null)
            return string.Empty;
        return formatter switch
        {
            null => GetString(value),
            "childCount" when value is IReadOnlyCollection<PersonEntryTemplateModel> children
                => $"{children.Count.ToString(CultureInfo.InvariantCulture)} Kdr:",
            "dateSuffix" when value is string text
                => string.IsNullOrWhiteSpace(text) ? string.Empty : $" {text}",
            "gedcomDatePrefix" when value is string date
                => string.IsNullOrWhiteSpace(date) ? string.Empty : $"{date}: ",
            "ordinal" when value is int ordinal => $"{ordinal.ToString(CultureInfo.InvariantCulture)}.",
            "vitalEvents" when value is PersonEntryTemplateModel person => person.VitalEventsGc,
            _ => throw new InvalidDataException($"Formatter '{formatter}' cannot format this template value.")
        };
    }

    private static IEnumerable<object?> Enumerate(object? value)
    {
        if (value is not IEnumerable items || value is string)
            throw new InvalidDataException("A forEach items path did not resolve to a collection.");
        foreach (var item in items)
            yield return item;
    }

    private static bool IsTruthy(object? value) => value switch
    {
        bool boolean => boolean,
        string text => !string.IsNullOrWhiteSpace(text),
        ICollection collection => collection.Count > 0,
        null => false,
        _ => true
    };

    private static bool IsConditionMet(string condition, IReadOnlyDictionary<string, object?> variables)
    {
        var negate = condition.StartsWith('!');
        var path = negate ? condition[1..] : condition;
        var value = IsTruthy(Resolve(path, variables));
        return negate ? !value : value;
    }

    private static string GetString(object? value) => value switch
    {
        null => string.Empty,
        string text => text,
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
    };

    private static void AppendText(IDocParagraph paragraph, string text, EntryTemplateBlock block)
    {
        if (!string.IsNullOrEmpty(text))
            paragraph.AddSpan(text, CreateFontStyle(block));
    }

    private static void AppendLinkedText(IDocParagraph paragraph, string target, string text, EntryTemplateBlock block)
    {
        var style = CreateFontStyle(block, underline: true);
        var link = paragraph.AddLink($"#{target}", style);
        link.SetStyle(style);
        link.TextContent = text;
    }

    private static IDocFontStyle CreateFontStyle(EntryTemplateBlock block, bool underline = false) => new DocFontStyle
    {
        Bold = block.Bold,
        Italic = block.Italic,
        Underline = block.Underline || underline
    };

    private static void RestoreVariable(
        Dictionary<string, object?> variables,
        string name,
        bool hadPriorValue,
        object? priorValue)
    {
        if (hadPriorValue)
            variables[name] = priorValue;
        else
            variables.Remove(name);
    }
}
