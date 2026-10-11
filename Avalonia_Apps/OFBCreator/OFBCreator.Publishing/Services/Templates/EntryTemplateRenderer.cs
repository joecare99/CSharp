using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Document.Base.Models.Interfaces;
using Document.Base.Models;
using OFBCreator.Publishing.Models;
using OFBCreator.Publishing.Services;
using OFBCreator.Projects.Models;

namespace OFBCreator.Publishing.Services.Templates;

/// <summary>
/// Renders validated entry templates without evaluating executable expressions.
/// </summary>
public sealed class EntryTemplateRenderer
{
    private readonly OFBProjectLayout _layout;

    private sealed record InlineTextSegment(
        string Text,
        bool Bold,
        bool Italic,
        bool Underline,
        bool LineBreakBefore);

    public EntryTemplateRenderer(OFBProjectLayout? layout = null)
    {
        _layout = layout ?? new OFBProjectLayout();
    }

    public void RenderFamily(
        IUserDocument document,
        EntryTemplateDefinition template,
        FamilyEntryTemplateModel family)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(family);
        if (!string.Equals(template.EntryRoot, EntryTemplateContract.EntryRoot.Family, StringComparison.Ordinal))
            throw new InvalidDataException(
                $"Template '{template.Id}' has root '{template.EntryRoot}', but a Family entry was supplied.");

        RenderRoot(document, template, new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            [EntryTemplateContract.Variable.Family] = family
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
        if (!string.Equals(template.EntryRoot, EntryTemplateContract.EntryRoot.Individual, StringComparison.Ordinal))
            throw new InvalidDataException(
                $"Template '{template.Id}' has root '{template.EntryRoot}', but an Individual entry was supplied.");

        RenderRoot(document, template, new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            [EntryTemplateContract.Variable.Individual] = individual,
            [EntryTemplateContract.Variable.Person] = individual
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

    private void RenderRoot(
        IUserDocument document,
        EntryTemplateDefinition template,
        Dictionary<string, object?> variables)
    {
        RenderBlocks(document, template, template.Blocks, variables, new HashSet<string>(StringComparer.Ordinal));
    }

    private void RenderBlocks(
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
                case EntryTemplateContract.Block.Section:
                    if (document is not IDocSectionProvider sectionProvider)
                        throw new NotSupportedException("The selected document provider does not support sections.");
                    sectionProvider.AddSection(block.Columns);
                    RenderBlocks(document, template, block.Blocks, variables, includeStack);
                    break;
                case EntryTemplateContract.Block.Paragraph:
                    var paragraph = document.AddParagraph(block.Role ?? EntryTemplateContract.Role.FamilyData);
                    if (block.Indent is int indentationBefore)
                        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.IndentationBefore, indentationBefore));
                    if (block.HangingIndent is int hangingIndent)
                    {
                        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.IndentationHanging, hangingIndent));
                    }
                    if (block.SpaceBefore is int spaceBefore)
                    {
                        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.SpacingBeforePt, spaceBefore));
                    }
                    ApplyLayoutStyle(paragraph, block.Role, variables);
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
                case EntryTemplateContract.Block.If:
                    if (IsConditionMet(block.Condition!, variables))
                        RenderBlocks(document, template, block.Then, variables, includeStack);
                    break;
                case EntryTemplateContract.Block.ForEach:
                    RenderEach(document, template, block, variables, includeStack);
                    break;
                case EntryTemplateContract.Block.Include:
                    RenderInclude(document, template, block, variables, includeStack);
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
                case EntryTemplateContract.Block.Text:
                    AddLineBreakIfRequested(paragraph, block);
                    AppendText(paragraph, block.Value!, block);
                    break;
                case EntryTemplateContract.Block.Field:
                    AddLineBreakIfRequested(paragraph, block);
                    AppendText(paragraph, Format(Resolve(block.Path!, variables), block.Formatter), block);
                    break;
                case EntryTemplateContract.Block.Link:
                    AddLineBreakIfRequested(paragraph, block);
                    var target = GetString(Resolve(block.Target!, variables));
                    var segments = new List<InlineTextSegment>();
                    CollectInlineTextSegments(
                        template,
                        block.Content,
                        variables,
                        includeStack,
                        block.Bold,
                        block.Italic,
                        block.Underline,
                        segments);
                    foreach (var segment in segments)
                    {
                        if (segment.LineBreakBefore)
                            paragraph.AddLineBreak();
                        if (string.IsNullOrWhiteSpace(target))
                        {
                            paragraph.AddSpan(segment.Text, new DocFontStyle
                            {
                                Bold = segment.Bold,
                                Italic = segment.Italic,
                                Underline = segment.Underline
                            });
                            continue;
                        }

                        var style = new DocFontStyle
                        {
                            Bold = segment.Bold,
                            Italic = segment.Italic,
                            Underline = true
                        };
                        var link = paragraph.AddLink($"#{target}", style);
                        link.SetStyle(style);
                        link.TextContent = segment.Text;
                    }
                    break;
                case EntryTemplateContract.Block.If:
                    if (IsConditionMet(block.Condition!, variables))
                        RenderInline(document, template, paragraph, block.Then, variables, includeStack);
                    break;
                case EntryTemplateContract.Block.ForEach:
                    RenderEachInline(document, template, paragraph, block, variables, includeStack);
                    break;
                case EntryTemplateContract.Block.Include:
                    RenderIncludeInline(document, template, paragraph, block, variables, includeStack);
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
                case EntryTemplateContract.Block.Text:
                    values.Add(block.Value!);
                    break;
                case EntryTemplateContract.Block.Field:
                    values.Add(Format(Resolve(block.Path!, variables), block.Formatter));
                    break;
                case EntryTemplateContract.Block.If:
                    if (IsConditionMet(block.Condition!, variables))
                        values.Add(RenderInlineToString(template, block.Then, variables, includeStack));
                    break;
                case EntryTemplateContract.Block.ForEach:
                    values.Add(RenderEachInlineToString(template, block, variables, includeStack));
                    break;
                case EntryTemplateContract.Block.Include:
                    values.Add(RenderIncludeInlineToString(template, block, variables, includeStack));
                    break;
                default:
                    throw new InvalidDataException($"Block '{block.Kind}' cannot be nested inside a link.");
            }
        }

        return string.Concat(values);
    }

    private static void CollectInlineTextSegments(
        EntryTemplateDefinition template,
        IEnumerable<EntryTemplateBlock> blocks,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack,
        bool inheritedBold,
        bool inheritedItalic,
        bool inheritedUnderline,
        ICollection<InlineTextSegment> segments)
    {
        foreach (var block in blocks)
        {
            var bold = inheritedBold || block.Bold;
            var italic = inheritedItalic || block.Italic;
            var underline = inheritedUnderline || block.Underline;
            switch (block.Kind)
            {
                case EntryTemplateContract.Block.Text:
                    AddSegment(block.Value!, block, bold, italic, underline);
                    break;
                case EntryTemplateContract.Block.Field:
                    AddSegment(Format(Resolve(block.Path!, variables), block.Formatter), block, bold, italic, underline);
                    break;
                case EntryTemplateContract.Block.If:
                    if (IsConditionMet(block.Condition!, variables))
                        CollectInlineTextSegments(template, block.Then, variables, includeStack, bold, italic, underline, segments);
                    break;
                case EntryTemplateContract.Block.ForEach:
                    foreach (var item in Enumerate(Resolve(block.Items!, variables)))
                    {
                        var hadPriorValue = variables.TryGetValue(block.As!, out var priorValue);
                        variables[block.As!] = item;
                        CollectInlineTextSegments(template, block.Template, variables, includeStack, bold, italic, underline, segments);
                        RestoreVariable(variables, block.As!, hadPriorValue, priorValue);
                    }
                    break;
                case EntryTemplateContract.Block.Include:
                    var fragmentName = block.Fragment!;
                    if (!includeStack.Add(fragmentName))
                        throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");
                    try
                    {
                        CollectInlineTextSegments(
                            template,
                            template.Fragments[fragmentName],
                            CreateIncludeVariables(block, variables),
                            includeStack,
                            bold,
                            italic,
                            underline,
                            segments);
                    }
                    finally
                    {
                        includeStack.Remove(fragmentName);
                    }
                    break;
                default:
                    throw new InvalidDataException($"Block '{block.Kind}' cannot be nested inside a link.");
            }

            void AddSegment(string text, EntryTemplateBlock source, bool segmentBold, bool segmentItalic, bool segmentUnderline)
            {
                if (text.Length > 0)
                    segments.Add(new InlineTextSegment(text, segmentBold, segmentItalic, segmentUnderline, source.LineBreakBefore));
            }
        }
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
        EntryTemplateBlock block,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        var fragmentName = block.Fragment!;
        if (!includeStack.Add(fragmentName))
            throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");
        try
        {
            return RenderInlineToString(template, template.Fragments[fragmentName], CreateIncludeVariables(block, variables), includeStack);
        }
        finally
        {
            includeStack.Remove(fragmentName);
        }
    }

    private void RenderEach(
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

    private void RenderInclude(
        IUserDocument document,
        EntryTemplateDefinition template,
        EntryTemplateBlock block,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        var fragmentName = block.Fragment!;
        if (!includeStack.Add(fragmentName))
            throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");
        try
        {
            RenderBlocks(document, template, template.Fragments[fragmentName], CreateIncludeVariables(block, variables), includeStack);
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
        EntryTemplateBlock block,
        Dictionary<string, object?> variables,
        HashSet<string> includeStack)
    {
        var fragmentName = block.Fragment!;
        if (!includeStack.Add(fragmentName))
            throw new InvalidDataException($"Template include cycle detected at fragment '{fragmentName}'.");
        try
        {
            RenderInline(document, template, paragraph, template.Fragments[fragmentName], CreateIncludeVariables(block, variables), includeStack);
        }
        finally
        {
            includeStack.Remove(fragmentName);
        }
    }

    private static Dictionary<string, object?> CreateIncludeVariables(
        EntryTemplateBlock block,
        IReadOnlyDictionary<string, object?> variables)
    {
        var includeVariables = variables.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        if (block.Context is not null)
            includeVariables[EntryTemplateContract.Variable.Person] = Resolve(block.Context, variables);
        return includeVariables;
    }

    private void ApplyLayoutStyle(
        IDocParagraph paragraph,
        string? templateRole,
        IReadOnlyDictionary<string, object?> variables)
    {
        var style = templateRole switch
        {
            EntryTemplateContract.Role.FamilyEntry => _layout.FamilyEntry,
            EntryTemplateContract.Role.ParentEntry => _layout.ParentEntry,
            EntryTemplateContract.Role.ParentLifeEvents => _layout.ParentLifeEvents,
            EntryTemplateContract.Role.ChildHeader => _layout.ChildHeader,
            EntryTemplateContract.Role.ChildEntry => _layout.ChildEntry,
            EntryTemplateContract.Role.ChildLifeEvents => _layout.ChildLifeEvents,
            EntryTemplateContract.Role.IndexEntry => _layout.IndexEntry,
            EntryTemplateContract.Role.FamilyData when variables.TryGetValue(EntryTemplateContract.Variable.Person, out var value)
                && value is PersonEntryTemplateModel person && person.Ordinal.HasValue => _layout.ChildLifeEvents,
            EntryTemplateContract.Role.FamilyData when variables.ContainsKey(EntryTemplateContract.Variable.Person) => _layout.ParentLifeEvents,
            EntryTemplateContract.Role.FamilyData => _layout.FamilyEntry,
            EntryTemplateContract.Role.Adult => _layout.ParentEntry,
            EntryTemplateContract.Role.ChildrenHeader => _layout.ChildHeader,
            EntryTemplateContract.Role.Child => _layout.ChildEntry,
            _ => null
        };
        if (style is null)
            return;

        OFBDocumentLayout.ApplyStyle(paragraph, style);
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

            if (value is null)
                throw new InvalidDataException($"Template value '{path}' is not available in this rendering context.");

            var property = EntryTemplateContract.GetProperty(value.GetType(), parts[index]);
            if (property is null)
                throw new InvalidDataException($"Template value '{path}' is not available in this rendering context.");
            value = property.GetValue(value);
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
            EntryTemplateContract.Formatter.ChildCount when value is IReadOnlyCollection<PersonEntryTemplateModel> children
                => $"{children.Count.ToString(CultureInfo.InvariantCulture)} Kdr:",
            EntryTemplateContract.Formatter.DateSuffix when value is string text
                => string.IsNullOrWhiteSpace(text) ? string.Empty : $" {text}",
            EntryTemplateContract.Formatter.GedcomDatePrefix when value is string date
                => string.IsNullOrWhiteSpace(date) ? string.Empty : $"{date}: ",
            EntryTemplateContract.Formatter.Ordinal when value is int ordinal => $"{ordinal.ToString(CultureInfo.InvariantCulture)}.",
            EntryTemplateContract.Formatter.VitalEvents when value is PersonEntryTemplateModel person => person.VitalEvents,
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
