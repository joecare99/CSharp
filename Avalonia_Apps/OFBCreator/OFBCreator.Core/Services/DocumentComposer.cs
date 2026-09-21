using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Default implementation of <see cref="IDocumentComposer"/> that writes OFB document structure.
/// Produces: title page, TOC, preamble/glossary, numbered families, five indices (person, occupation, property, alphabetical place, hierarchical place).
/// Uses duck-typing for <c>IUserDocument</c> — no direct dependency on a concrete format provider.
/// </summary>
public class DocumentComposer : IDocumentComposer
{
    /// <inheritdoc />
    public Task ComposeAsync(
        object document,
        OFBSourceSelection selection,
        IReadOnlyList<OFBIndexEntry> personIndex,
        IReadOnlyList<OFBIndexEntry> occupationIndex,
        IReadOnlyList<OFBIndexEntry> propertyIndex,
        IReadOnlyList<OFBPlaceHierarchyNode> placeHierarchyIndex,
        IReadOnlyList<OFBIndexEntry> placeAlphaIndex,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(selection);

        // Discover IUserDocument members via duck-typing
        var docType = document.GetType();
        var addParagraphMethod = docType.GetMethod("AddParagraph", BindingFlags.Instance | BindingFlags.Public);
        var addHeadlineMethod = docType.GetMethod("AddHeadline", BindingFlags.Instance | BindingFlags.Public);
        var addTocMethod = docType.GetMethod("AddTOC", BindingFlags.Instance | BindingFlags.Public);
        var rootProperty = docType.GetProperty("Root", BindingFlags.Instance | BindingFlags.Public);
        var isModifiedProp = docType.GetProperty("IsModified", BindingFlags.Instance | BindingFlags.Public);

        if (addParagraphMethod == null || addHeadlineMethod == null || addTocMethod == null || rootProperty == null)
            throw new InvalidOperationException("Document object does not implement the required IUserDocument contract (AddParagraph, AddHeadline, AddTOC, Root).");

        // Phase 1: Title page (headline level 1)
        AddHeadline(addHeadlineMethod, document, 1, "title-page");

        // Phase 2: Table of contents
        _ = AddToc(addTocMethod, document, "Inhalt", 1);

        // Phase 3: Preface section (optional)
        ComposePreamble(document, addHeadlineMethod, addParagraphMethod, selection, cancellationToken);

        // Phase 4: Numbered families main section
        ComposeFamilies(document, selection, addHeadlineMethod, addParagraphMethod, cancellationToken);

        // Phase 5: Person index
        ComposePersonIndex(document, personIndex, addHeadlineMethod, addParagraphMethod, cancellationToken);

        // Phase 6: Occupation index
        ComposeOccupationIndex(document, occupationIndex, addHeadlineMethod, addParagraphMethod, cancellationToken);

        // Phase 7: Property index
        ComposePropertyIndex(document, propertyIndex, addHeadlineMethod, addParagraphMethod, cancellationToken);

        // Phase 8: Alphabetical place index
        ComposeAlphabeticalPlaceIndex(document, placeAlphaIndex, addHeadlineMethod, addParagraphMethod, cancellationToken);

        // Phase 9: Hierarchical place index
        ComposeHierarchicalPlaceIndex(document, placeHierarchyIndex, addHeadlineMethod, addParagraphMethod, cancellationToken);

        // Signal document as modified
        if (isModifiedProp != null && isModifiedProp.CanWrite)
            isModifiedProp.SetValue(document, true);

        return Task.CompletedTask;
    }

    #region Preamble / Title helpers

    private static void ComposePreamble(
        object document,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        OFBSourceSelection selection,
        CancellationToken cancellationToken)
    {
        // Preface section (host-supplied via document.PrefaceText)
        var prefTextProp = document.GetType().GetProperty("PrefaceText");
        if (prefTextProp != null)
        {
            var prefaceText = prefTextProp.GetValue(document)?.ToString();
            if (!string.IsNullOrWhiteSpace(prefaceText))
            {
                AddHeadline(addHeadlineMethod, document, 1, "preamble");

                foreach (var line in prefaceText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmed = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        AddParagraph(addParagraphMethod, document, trimmed);
                }
            }
        }

        // Legend / Zeichenerklärung section (host-supplied via document.LegendText)
        var legendProp = document.GetType().GetProperty("LegendText");
        if (legendProp != null)
        {
            var legendText = legendProp.GetValue(document)?.ToString();
            if (!string.IsNullOrWhiteSpace(legendText))
            {
                AddHeadline(addHeadlineMethod, document, 1, "legend");

                foreach (var line in legendText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmed = line.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        AddParagraph(addParagraphMethod, document, trimmed);
                }
            }
        }
    }

    #endregion

    #region Family composition

    private static void ComposeFamilies(
        object document,
        OFBSourceSelection selection,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        CancellationToken cancellationToken)
    {
        // Section headline "Familien"
        AddHeadline(addHeadlineMethod, document, 1, "families-section");

        // Check test data source first (supports duck-typed families with SourceRefId etc.)
        var testDataProp = typeof(OFBSourceSelection).GetProperty("TestFamilyData", BindingFlags.Instance | BindingFlags.Public);
        var testFamiliesVal = testDataProp?.GetValue(selection);
        var familiesList = testFamiliesVal as System.Collections.IList ?? 
            (typeof(OFBSourceSelection).GetProperty("SelectedFamilies")?.GetValue(selection) as System.Collections.IList);

        if (familiesList == null || familiesList.Count == 0)
            return;

        foreach (var familyObj in familiesList)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sourceRef = GetProp<string>(familyObj, "SourceRefId") ?? string.Empty;
            var husband = GetProp<IGenPerson>(familyObj, "Husband");
            var wife = GetProp<IGenPerson>(familyObj, "Wife");
            var marriageDate = GetProp<object>(familyObj, "MarriageDate");
            var marriagePlace = GetProp<IGenPlace>(familyObj, "MarriagePlace");

            // Family headline: numbered anchor
            AddHeadline(addHeadlineMethod, document, 2, $"family-{sourceRef}");

            // Spouse line: "H [Name] — F [Name]"
            var spouseLine = new StringBuilder();
            if (husband != null)
                spouseLine.Append($"H {FormatPersonName(husband)}");
            if (husband != null && wife != null)
                spouseLine.Append(" \\ ");
            if (wife != null)
                spouseLine.Append($"F {FormatPersonName(wife)}");

            if (spouseLine.Length > 0)
                AddParagraph(addParagraphMethod, document, spouseLine.ToString());

            // Marriage date and place
            var marrParts = new List<string>();
            if (marriageDate != null)
                marrParts.Add($"Trauung: {marriageDate}");
            if (marriagePlace != null && !string.IsNullOrEmpty(marriagePlace.Name))
                marrParts[^1] += $" in {marriagePlace.Name}";

            foreach (var part in marrParts)
                AddParagraph(addParagraphMethod, document, part);

            // Children
            var childrenList = GetChildren(familyObj);
            if (childrenList != null)
            {
                foreach (var child in childrenList)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var parts = new List<string>();
                    parts.Add($"K {FormatPersonName(child)}");
                    var birthDate = GetProp<object>(child, "BirthDate");
                    if (birthDate != null)
                        parts[^1] += $", geb. {birthDate}";
                    var birthPlace = GetProp<IGenPlace>(child, "BirthPlace");
                    if (birthPlace != null && !string.IsNullOrEmpty(birthPlace.Name))
                        parts[^1] += $" in {birthPlace.Name}";

                    AddParagraph(addParagraphMethod, document, parts[0]);
                }
            }
        }
    }

    #endregion

    #region Index composition helpers

    private static void ComposePersonIndex(
        object document,
        IReadOnlyList<OFBIndexEntry> entries,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        CancellationToken cancellationToken) =>
        ComposeGenericIndex(document, "person-index", "P", entries, addHeadlineMethod, addParagraphMethod, cancellationToken);

    private static void ComposeOccupationIndex(
        object document,
        IReadOnlyList<OFBIndexEntry> entries,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        CancellationToken cancellationToken) =>
        ComposeGenericIndex(document, "occupation-index", "B", entries, addHeadlineMethod, addParagraphMethod, cancellationToken);

    private static void ComposePropertyIndex(
        object document,
        IReadOnlyList<OFBIndexEntry> entries,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        CancellationToken cancellationToken) =>
        ComposeGenericIndex(document, "property-index", "E", entries, addHeadlineMethod, addParagraphMethod, cancellationToken);

    private static void ComposeAlphabeticalPlaceIndex(
        object document,
        IReadOnlyList<OFBIndexEntry> entries,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        CancellationToken cancellationToken) =>
        ComposeGenericIndex(document, "place-alpha-index", "O", entries, addHeadlineMethod, addParagraphMethod, cancellationToken);

    private static void ComposeGenericIndex(
        object document,
        string anchorId,
        string prefix,
        IReadOnlyList<OFBIndexEntry> entries,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        CancellationToken cancellationToken)
    {
        AddHeadline(addHeadlineMethod, document, 1, anchorId);

        if (entries == null || entries.Count == 0)
            return;

        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = $"{prefix} {entry.SortKey} — {entry.Ref}";
            AddParagraph(addParagraphMethod, document, line);
        }
    }

    private static void ComposeHierarchicalPlaceIndex(
        object document,
        IReadOnlyList<OFBPlaceHierarchyNode> nodes,
        MethodInfo addHeadlineMethod,
        MethodInfo addParagraphMethod,
        CancellationToken cancellationToken)
    {
        AddHeadline(addHeadlineMethod, document, 1, "place-hierarchy-index");

        if (nodes == null || nodes.Count == 0)
            return;

        foreach (var node in nodes)
            WritePlaceNode(document, node, addParagraphMethod, indentation: 0, cancellationToken);
    }

    private static void WritePlaceNode(
        object document,
        OFBPlaceHierarchyNode node,
        MethodInfo addParagraphMethod,
        int indentation,
        CancellationToken cancellationToken)
    {
        var indent = new string(' ', indentation * 2);
        AddParagraph(addParagraphMethod, document, $"{indent}{node.Name}");

        foreach (var child in node.Children)
        {
            cancellationToken.ThrowIfCancellationRequested();
            WritePlaceNode(document, child, addParagraphMethod, indentation + 1, cancellationToken);
        }
    }

    #endregion

    #region Reflection helpers

    private static void AddHeadline(MethodInfo method, object document, int level, string? id)
    {
        var parameters = new object[] { level, (object?)id };
        _ = method.Invoke(document, parameters);
    }

    private static void AddParagraph(MethodInfo method, object document, string text)
    {
        var parameters = new object[] { text };
        _ = method.Invoke(document, parameters);
    }

    private static object? AddToc(MethodInfo method, object document, string name, int level)
    {
        var parameters = new object[] { name, level };
        return method.Invoke(document, parameters);
    }

    private static T? GetProp<T>(object? obj, string propName)
    {
        if (obj == null)
            return default;
        var prop = obj.GetType().GetProperty(propName);
        var val = prop?.GetValue(obj);
        return val is T typed ? typed : default;
    }

    private static IReadOnlyList<IGenPerson>? GetChildren(object familyObj)
    {
        var childrenProp = familyObj.GetType().GetProperty("Children");
        var childrenVal = childrenProp?.GetValue(familyObj);

        // Try as strongly-typed IReadOnlyList<IGenPerson>
        if (childrenVal is IReadOnlyList<IGenPerson> typedList)
            return typedList;

        // Fallback: try IEnumerable and cast items
        if (childrenVal is System.Collections.IEnumerable enumerable)
        {
            var list = new List<IGenPerson>();
            foreach (var item in enumerable)
            {
                if (item is IGenPerson person)
                    list.Add(person);
            }
            return list.Count > 0 ? list.AsReadOnly() : null;
        }

        return null;
    }

    private static string FormatPersonName(IGenPerson person)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(person.Surname))
            parts.Add(person.Surname!);
        if (!string.IsNullOrEmpty(person.GivenName))
            parts.Add(person.GivenName!);
        return parts.Count > 0 ? string.Join(" ", parts) : "Unknown";
    }

    #endregion
}
