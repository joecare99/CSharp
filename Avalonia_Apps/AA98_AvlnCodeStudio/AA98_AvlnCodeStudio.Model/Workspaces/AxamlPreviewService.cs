using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Parses AXAML into a bounded safe preview tree without loading or instantiating user types.
/// </summary>
public sealed class AxamlPreviewService
{
    private static readonly XNamespace s_avaloniaNamespace = "https://github.com/avaloniaui";
    private const int MaximumDocumentCharacters = 1_000_000;
    private const int MaximumTreeDepth = 64;
    private const int MaximumControls = 2_000;
    private const int MaximumGridCells = 100_000;
    /// <summary>
    /// Parses an AXAML document and returns its preview tree.
    /// </summary>
    public AxamlPreviewNode Parse(string axaml)
    {
        ArgumentNullException.ThrowIfNull(axaml);
        if (axaml.Length > MaximumDocumentCharacters)
        {
            throw new InvalidDataException($"AXAML preview is limited to {MaximumDocumentCharacters} characters.");
        }

        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = MaximumDocumentCharacters,
            MaxCharactersFromEntities = 0,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };
        using StringReader textReader = new(axaml);
        using XmlReader xmlReader = XmlReader.Create(textReader, settings);
        XDocument document = XDocument.Load(xmlReader, LoadOptions.SetLineInfo);
        XElement root = document.Root
            ?? throw new InvalidDataException("The AXAML document has no root element.");
        int controlCount = 0;
        return ParseElement(root, axaml, depth: 0, ref controlCount);
    }

    public AxamlPreviewParseResult ParseForPreview(string axaml)
    {
        try
        {
            return new AxamlPreviewParseResult(Parse(axaml), null);
        }
        catch (XmlException exception)
        {
            if (axaml.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }

            AxamlPreviewNode? recoveredRoot = RecoverValidElements(axaml, exception);
            return recoveredRoot is null
                ? new AxamlPreviewParseResult(null, exception.Message)
                : new AxamlPreviewParseResult(recoveredRoot, recoveredRoot.Diagnostic);
        }
    }

    public bool CanEditText(AxamlPreviewNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        string? propertyName = GetEditablePropertyName(node.TypeName);
        return propertyName is not null
            && node.Children.Count == 0
            && !node.HasInlineText
            && CanEditProperty(node, propertyName);
    }

    public bool CanEditProperty(AxamlPreviewNode node, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        return node.IsSupported
            && GetEditableProperties(node).Contains(propertyName, StringComparer.Ordinal)
            && node.SourceStartOffset >= 0
            && (!node.Properties.TryGetValue(propertyName, out string? value)
                || !value.TrimStart().StartsWith('{'))
            && !(propertyName == GetEditablePropertyName(node.TypeName) && node.HasInlineText)
            && !node.PropertyElementNames.Contains($"{node.TypeName}.{propertyName}", StringComparer.Ordinal);
    }

    public IReadOnlyList<string> GetEditableProperties(AxamlPreviewNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return AxamlControlCatalog.TryGet(node.TypeName, out AxamlControlCapability capability)
            ? capability.EditableProperties
            : [];
    }

    public string UpdateTextAttribute(string axaml, AxamlPreviewNode node, string value)
    {
        ArgumentNullException.ThrowIfNull(axaml);
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(value);
        if (!CanEditText(node))
        {
            throw new InvalidOperationException($"The '{node.TypeName}' preview element cannot be edited.");
        }

        string propertyName = GetEditablePropertyName(node.TypeName)!;
        return UpdateAttribute(axaml, node, propertyName, value);
    }

    public string UpdateAttribute(string axaml, AxamlPreviewNode node, string propertyName, string value)
    {
        return UpdateAttributeCore(axaml, node, propertyName, value, allowExistingBinding: false);
    }

    public string UpdateBinding(
        string axaml,
        AxamlPreviewNode node,
        string propertyName,
        string memberName,
        AxamlBindingMode bindingMode = AxamlBindingMode.Default,
        AxamlBindingUpdateSourceTrigger updateSourceTrigger = AxamlBindingUpdateSourceTrigger.Default)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
        if (!Enum.IsDefined(bindingMode))
        {
            throw new ArgumentOutOfRangeException(nameof(bindingMode));
        }

        if (!Enum.IsDefined(updateSourceTrigger))
        {
            throw new ArgumentOutOfRangeException(nameof(updateSourceTrigger));
        }

        if (!AxamlMvvmMemberName.IsValid(memberName))
        {
            throw new InvalidOperationException($"'{memberName}' is not a valid simple ViewModel member name.");
        }

        if (!AxamlControlCatalog.TryGet(node.TypeName, out AxamlControlCapability capability)
            || !capability.BindableProperties.Any(property => property.Name == propertyName)
            || !node.IsSupported
            || node.SourceStartOffset < 0
            || node.PropertyElementNames.Contains($"{node.TypeName}.{propertyName}", StringComparer.Ordinal))
        {
            throw new InvalidOperationException($"The '{propertyName}' binding on '{node.TypeName}' cannot be edited safely.");
        }

        List<string> options = [];
        if (bindingMode != AxamlBindingMode.Default)
        {
            options.Add($"Mode={bindingMode}");
        }
        if (updateSourceTrigger != AxamlBindingUpdateSourceTrigger.Default)
        {
            options.Add($"UpdateSourceTrigger={updateSourceTrigger}");
        }

        string optionSuffix = options.Count == 0 ? string.Empty : $", {string.Join(", ", options)}";
        return UpdateAttributeCore(
            axaml,
            node,
            propertyName,
            $"{{Binding {memberName}{optionSuffix}}}",
            allowExistingBinding: true);
    }

    private string UpdateAttributeCore(
        string axaml,
        AxamlPreviewNode node,
        string propertyName,
        string value,
        bool allowExistingBinding)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(axaml);
        ArgumentNullException.ThrowIfNull(value);
        if (!CanEditProperty(node, propertyName)
            && !(allowExistingBinding
                && node.IsSupported
                && node.SourceStartOffset >= 0
                && AxamlControlCatalog.TryGet(node.TypeName, out AxamlControlCapability capability)
                && capability.BindableProperties.Any(property => property.Name == propertyName)
                && !node.PropertyElementNames.Contains($"{node.TypeName}.{propertyName}", StringComparer.Ordinal)))
        {
            throw new InvalidOperationException($"The '{propertyName}' property on '{node.TypeName}' cannot be edited.");
        }

        int tagStart = node.SourceStartOffset;
        if (tagStart >= axaml.Length || axaml[tagStart] != '<')
        {
            throw new InvalidDataException("The AXAML source changed; select the preview element again.");
        }

        int tagEnd = FindStartTagEnd(axaml, tagStart);
        if (node.SourceStartTag is null
            || !axaml.AsSpan(tagStart, tagEnd - tagStart + 1).SequenceEqual(node.SourceStartTag.AsSpan()))
        {
            throw new InvalidDataException("The AXAML source changed; select the preview element again.");
        }

        int elementNameStart = tagStart + 1;
        int elementNameEnd = elementNameStart;
        while (elementNameEnd < tagEnd
            && !char.IsWhiteSpace(axaml[elementNameEnd])
            && axaml[elementNameEnd] is not '/' and not '>')
        {
            elementNameEnd++;
        }

        string sourceTypeName = axaml[elementNameStart..elementNameEnd];
        int prefixSeparator = sourceTypeName.LastIndexOf(':');
        if (prefixSeparator >= 0)
        {
            sourceTypeName = sourceTypeName[(prefixSeparator + 1)..];
        }

        if (!string.Equals(sourceTypeName, node.TypeName, StringComparison.Ordinal))
        {
            throw new InvalidDataException("The AXAML source changed; select the preview element again.");
        }

        int attributeStart = FindAttributeValueStart(
            axaml,
            tagStart,
            tagEnd,
            propertyName,
            out int attributeNameStart,
            out int attributeEnd);
        if (value.Length == 0)
        {
            return attributeStart < 0
                ? axaml
                : string.Concat(axaml.AsSpan(0, attributeNameStart), axaml.AsSpan(attributeEnd));
        }

        string escapedValue = EscapeAttributeValue(value);
        if (attributeStart >= 0)
        {
            char quote = axaml[attributeStart - 1];
            int valueQuoteEnd = axaml.IndexOf(quote, attributeStart);
            if (valueQuoteEnd < 0 || valueQuoteEnd > tagEnd)
            {
                throw new InvalidDataException("The AXAML text attribute is malformed.");
            }

            return string.Concat(axaml.AsSpan(0, attributeStart), escapedValue, axaml.AsSpan(valueQuoteEnd));
        }

        int insertionOffset = tagEnd;
        while (insertionOffset > tagStart && char.IsWhiteSpace(axaml[insertionOffset - 1]))
        {
            insertionOffset--;
        }

        if (insertionOffset > tagStart && axaml[insertionOffset - 1] == '/')
        {
            insertionOffset--;
        }

        string separator = insertionOffset > tagStart && char.IsWhiteSpace(axaml[insertionOffset - 1])
            ? string.Empty
            : " ";
        return string.Concat(
            axaml.AsSpan(0, insertionOffset),
            $"{separator}{propertyName}=\"{escapedValue}\"",
            axaml.AsSpan(insertionOffset));
    }

    public string InsertChildControl(
        string axaml,
        AxamlPreviewNode parent,
        string childTypeName,
        AxamlDockPanelPlacement? dockPanelPlacement = null)
    {
        ArgumentNullException.ThrowIfNull(axaml);
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentException.ThrowIfNullOrWhiteSpace(childTypeName);
        if (!parent.IsSupported
            || !AxamlControlCatalog.TryGet(parent.TypeName, out AxamlControlCapability parentCapability)
            || parentCapability.MaximumChildren == 0
            || parent.Children.Count >= parentCapability.MaximumChildren)
        {
            throw new InvalidOperationException($"The '{parent.TypeName}' control cannot contain another child.");
        }

        if (!AxamlControlCatalog.TryGet(childTypeName, out _))
        {
            throw new InvalidOperationException($"The '{childTypeName}' control is not in the supported designer catalog.");
        }

        bool isGrid = string.Equals(parent.TypeName, "Grid", StringComparison.Ordinal);
        bool isDockPanel = string.Equals(parent.TypeName, "DockPanel", StringComparison.Ordinal);
        bool dockPanelFillsLastChild = isDockPanel
            && !string.Equals(
                parent.Properties.GetValueOrDefault("LastChildFill"),
                "False",
                StringComparison.OrdinalIgnoreCase);
        if (isDockPanel)
        {
            if (dockPanelPlacement is null)
            {
                throw new InvalidOperationException("Choose a DockPanel child placement before inserting a control.");
            }

            if (!Enum.IsDefined(dockPanelPlacement.Value))
            {
                throw new ArgumentOutOfRangeException(nameof(dockPanelPlacement));
            }

            if (!parent.CanAppendChildren)
            {
                throw new InvalidOperationException(
                    "Correct invalid or unsupported DockPanel markup before adding controls; placement requires a simple panel with a literal LastChildFill value.");
            }

            if (dockPanelFillsLastChild
                && dockPanelPlacement == AxamlDockPanelPlacement.Client
                && parent.HasChildElements)
            {
                throw new InvalidOperationException(
                    "A DockPanel with LastChildFill enabled can have only one client child; add docked children before the client child.");
            }

            if (dockPanelFillsLastChild
                && dockPanelPlacement != AxamlDockPanelPlacement.Client
                && !parent.HasChildElements)
            {
                throw new InvalidOperationException(
                    "Add the DockPanel client child first; docked children are inserted before it.");
            }

            if (!dockPanelFillsLastChild && dockPanelPlacement == AxamlDockPanelPlacement.Client)
            {
                throw new InvalidOperationException(
                    "Client placement requires DockPanel LastChildFill to be enabled.");
            }
        }

        (int Row, int Column) gridCell = isGrid
            ? GetNextGridCell(parent)
            : (-1, -1);
        if (parent.HasInlineText)
        {
            throw new InvalidOperationException(
                "Correct invalid inline text before adding controls; panels require controls or supported property elements as children.");
        }

        if (parent.HasChildElements && !parent.CanAppendChildren && !parent.CanAddGridChild)
        {
            throw new InvalidOperationException(
                "Correct or simplify unsupported child/property markup before adding controls; existing content is preserved.");
        }

        if (parent.SourceStartOffset < 0 || parent.SourceStartTag is null)
        {
            throw new InvalidOperationException(
                "The selected container cannot be located safely; refresh the preview before adding controls.");
        }

        int tagStart = parent.SourceStartOffset;
        int tagEnd = FindStartTagEnd(axaml, tagStart);
        if (!axaml.AsSpan(tagStart, tagEnd - tagStart + 1).SequenceEqual(parent.SourceStartTag.AsSpan()))
        {
            throw new InvalidDataException("The AXAML source changed; select the container again.");
        }

        string qualifiedName = GetElementName(parent.SourceStartTag);
        int colon = qualifiedName.LastIndexOf(':');
        string childQualifiedName = colon < 0
            ? childTypeName
            : qualifiedName[..(colon + 1)] + childTypeName;
        string placementAttributes = isGrid
            ? $" Grid.Row=\"{gridCell.Row}\" Grid.Column=\"{gridCell.Column}\""
            : isDockPanel && dockPanelPlacement != AxamlDockPanelPlacement.Client
                ? $" DockPanel.Dock=\"{dockPanelPlacement}\""
            : string.Empty;
        string childMarkup = $"<{childQualifiedName}{placementAttributes} />";
        string lineIndentation = GetLineIndentation(axaml, tagStart);
        string childIndentation = lineIndentation + "  ";
        string newline = axaml.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";

        if (parent.HasChildElements)
        {
            if (isDockPanel && dockPanelFillsLastChild && dockPanelPlacement != AxamlDockPanelPlacement.Client)
            {
                AxamlPreviewNode fillChild = parent.Children.Last();
                int childStart = fillChild.SourceStartOffset;
                if (childStart < 0)
                {
                    throw new InvalidOperationException("The DockPanel client child cannot be located safely in the source.");
                }

                int childLineStart = axaml.LastIndexOf('\n', childStart - 1) + 1;
                bool childStartsOnOwnLine = axaml.AsSpan(childLineStart, childStart - childLineStart)
                    .Trim()
                    .IsEmpty;
                int dockedChildInsertionOffset = childStartsOnOwnLine ? childLineStart : childStart;
                string dockedChildInsertion = childStartsOnOwnLine
                    ? $"{childIndentation}{childMarkup}{newline}"
                    : $"{newline}{childIndentation}{childMarkup}{newline}{lineIndentation}";
                return string.Concat(
                    axaml.AsSpan(0, dockedChildInsertionOffset),
                    dockedChildInsertion,
                    axaml.AsSpan(dockedChildInsertionOffset));
            }

            int appendClosingTagStart = FindMatchingEndTag(axaml, tagEnd, qualifiedName);
            int closingLineStart = axaml.LastIndexOf('\n', appendClosingTagStart - 1) + 1;
            bool closingTagHasOwnLine = axaml.AsSpan(closingLineStart, appendClosingTagStart - closingLineStart)
                .Trim()
                .IsEmpty;
            int appendInsertionOffset = closingTagHasOwnLine ? closingLineStart : appendClosingTagStart;
            string insertion = closingTagHasOwnLine
                ? $"{childIndentation}{childMarkup}{newline}"
                : $"{newline}{childIndentation}{childMarkup}{newline}{lineIndentation}";
            return string.Concat(
                axaml.AsSpan(0, appendInsertionOffset),
                insertion,
                axaml.AsSpan(appendInsertionOffset));
        }

        int insertionOffset = tagEnd;
        while (insertionOffset > tagStart && char.IsWhiteSpace(axaml[insertionOffset - 1]))
        {
            insertionOffset--;
        }

        if (insertionOffset > tagStart && axaml[insertionOffset - 1] == '/')
        {
            int openingTagEnd = insertionOffset - 1;
            while (openingTagEnd > tagStart && char.IsWhiteSpace(axaml[openingTagEnd - 1]))
            {
                openingTagEnd--;
            }

            string openingTag = axaml[tagStart..openingTagEnd] + ">";
            string closingTag = $"</{qualifiedName}>";
            return string.Concat(
                axaml[..tagStart],
                openingTag,
                newline,
                childIndentation,
                childMarkup,
                newline,
                lineIndentation,
                closingTag,
                axaml[(tagEnd + 1)..]);
        }

        int closingTagStart = FindMatchingEndTag(axaml, tagEnd, qualifiedName);

        return string.Concat(
            axaml[..closingTagStart],
            newline,
            childIndentation,
            childMarkup,
            newline,
            lineIndentation,
            axaml[closingTagStart..]);
    }

        private static AxamlPreviewNode? RecoverValidElements(string source, XmlException parseException)
        {
            IReadOnlyList<string> inheritedNamespaces = GetRootNamespaceDeclarations(source);
            List<(int Start, int End, AxamlPreviewNode Node)> recovered = [];
            List<(int Start, int End, string Name, string Message)> rejected = [];
            for (int index = 0; index < source.Length; index++)
            {
                if (source.AsSpan(index).StartsWith("<!--", StringComparison.Ordinal))
                {
                    int commentEnd = source.IndexOf("-->", index + 4, StringComparison.Ordinal);
                    index = commentEnd < 0 ? source.Length : commentEnd + 2;
                    continue;
                }

                if (source.AsSpan(index).StartsWith("<![CDATA[", StringComparison.Ordinal))
                {
                    int cdataEnd = source.IndexOf("]]>", index + 9, StringComparison.Ordinal);
                    index = cdataEnd < 0 ? source.Length : cdataEnd + 2;
                    continue;
                }

                if (source.AsSpan(index).StartsWith("<?", StringComparison.Ordinal))
                {
                    int instructionEnd = source.IndexOf("?>", index + 2, StringComparison.Ordinal);
                    index = instructionEnd < 0 ? source.Length : instructionEnd + 1;
                    continue;
                }

                if (source[index] != '<' || !TryReadElementStart(source, index, out string? name, out int tagEnd, out bool selfClosing))
                {
                    continue;
                }

                int elementEnd = selfClosing ? tagEnd + 1 : FindElementEnd(source, tagEnd + 1, name);
                if (elementEnd <= tagEnd)
                {
                    continue;
                }

                string fragment = source[index..elementEnd];
                string wrappedFragment = $"<RecoveryRoot {string.Join(" ", inheritedNamespaces)}>{fragment}</RecoveryRoot>";
                try
                {
                    XDocument fragmentDocument = XDocument.Parse(wrappedFragment, LoadOptions.SetLineInfo);
                    XElement? element = fragmentDocument.Root?.Elements().SingleOrDefault();
                    if (element is null || element.Name.LocalName.Contains('.', StringComparison.Ordinal))
                    {
                        continue;
                    }

                    int controlCount = 0;
                    AxamlPreviewNode node = DisableSourceEdits(
                        ParseElement(element, fragment, depth: 0, ref controlCount));
                    recovered.Add((index, elementEnd, node));
                }
                catch (XmlException exception)
                {
                    if (elementEnd > tagEnd)
                    {
                        rejected.Add((index, elementEnd, name, exception.Message));
                    }
                }

                index = tagEnd;
            }

            List<(int Start, int End, AxamlPreviewNode Node)> candidates = [.. recovered];
            foreach ((int start, int end, string name, string message) in rejected.Where(rejectedElement =>
                !recovered.Any(validElement =>
                    validElement.Start >= rejectedElement.Start && validElement.End <= rejectedElement.End)))
            {
                candidates.Add((start, end, new AxamlPreviewNode
                {
                    TypeName = "Invalid AXAML",
                    IsSupported = false,
                    Diagnostic = $"Could not preview <{name}>: {message}",
                }));
            }

            List<AxamlPreviewNode> roots = [];
            int coveredUntil = -1;
            foreach ((int start, int end, AxamlPreviewNode node) in candidates.OrderBy(static item => item.Start))
            {
                if (start < coveredUntil)
                {
                    continue;
                }

                roots.Add(node);
                coveredUntil = end;
            }

            if (roots.Count == 0)
            {
                return null;
            }

            string diagnostic =
                $"Partial preview: valid AXAML elements are shown and malformed elements are marked for correction. Editing is disabled until the document is well-formed. {parseException.Message}";
            return new AxamlPreviewNode
            {
                TypeName = "StackPanel",
                IsSupported = true,
                IsRecoveryRoot = true,
                Diagnostic = diagnostic,
                HasChildElements = true,
                Children = roots,
            };
        }

        private static AxamlPreviewNode DisableSourceEdits(AxamlPreviewNode node)
        {
            return new AxamlPreviewNode
            {
                TypeName = node.TypeName,
                Text = node.Text,
                IsSupported = node.IsSupported,
                SourceStartOffset = -1,
                HasTextPropertyElement = node.HasTextPropertyElement,
                HasInlineText = node.HasInlineText,
                HasChildElements = node.HasChildElements,
                CanAppendChildren = false,
                CanAddGridChild = false,
                PropertyElementNames = node.PropertyElementNames,
                Properties = node.Properties,
                Children = node.Children.Select(DisableSourceEdits).ToArray(),
                RowDefinitions = node.RowDefinitions,
                ColumnDefinitions = node.ColumnDefinitions,
            };
        }

        private static IReadOnlyList<string> GetRootNamespaceDeclarations(string source)
        {
            int rootStart = 0;
            int rootTagEnd = -1;
            bool foundRoot = false;
            while ((rootStart = source.IndexOf('<', rootStart)) >= 0)
            {
                if (TryReadElementStart(source, rootStart, out _, out rootTagEnd, out _))
                {
                    foundRoot = true;
                    break;
                }

                if (source.AsSpan(rootStart).StartsWith("<!--", StringComparison.Ordinal))
                {
                    int commentEnd = source.IndexOf("-->", rootStart + 4, StringComparison.Ordinal);
                    rootStart = commentEnd < 0 ? source.Length : commentEnd + 3;
                }
                else if (source.AsSpan(rootStart).StartsWith("<?", StringComparison.Ordinal))
                {
                    int instructionEnd = source.IndexOf("?>", rootStart + 2, StringComparison.Ordinal);
                    rootStart = instructionEnd < 0 ? source.Length : instructionEnd + 2;
                }
                else
                {
                    rootStart++;
                }
            }

            if (!foundRoot)
            {
                return [];
            }

            const string pattern = @"\s+xmlns(?::[A-Za-z_][\w.-]*)?\s*=\s*(?:""[^""]*""|'[^']*')";
            return Regex.Matches(source[rootStart..(rootTagEnd + 1)], pattern, RegexOptions.CultureInvariant)
                .Select(static match => match.Value.Trim())
                .ToArray();
        }

        private static bool TryReadElementStart(
            string source,
            int start,
            out string name,
            out int tagEnd,
            out bool selfClosing)
        {
            name = string.Empty;
            tagEnd = -1;
            selfClosing = false;
            if (start + 1 >= source.Length || source[start + 1] is '/' or '!' or '?')
            {
                return false;
            }

            int nameStart = start + 1;
            int cursor = nameStart;
            while (cursor < source.Length
                && !char.IsWhiteSpace(source[cursor])
                && source[cursor] is not '/' and not '>')
            {
                cursor++;
            }

            if (cursor == nameStart)
            {
                return false;
            }

            name = source[nameStart..cursor];
            char quote = '\0';
            for (; cursor < source.Length; cursor++)
            {
                char current = source[cursor];
                if (quote != '\0')
                {
                    if (current == quote)
                    {
                        quote = '\0';
                    }
                }
                else if (current is '"' or '\'')
                {
                    quote = current;
                }
                else if (current == '>')
                {
                    int previous = cursor - 1;
                    while (previous > start && char.IsWhiteSpace(source[previous]))
                    {
                        previous--;
                    }

                    tagEnd = cursor;
                    selfClosing = source[previous] == '/';
                    return true;
                }
            }

            return false;
        }

        private static int FindElementEnd(string source, int searchStart, string elementName)
        {
            int nesting = 1;
            for (int index = searchStart; index < source.Length; index++)
            {
                if (source[index] != '<')
                {
                    continue;
                }

                if (source.AsSpan(index).StartsWith("<!--", StringComparison.Ordinal))
                {
                    int commentEnd = source.IndexOf("-->", index + 4, StringComparison.Ordinal);
                    if (commentEnd < 0)
                    {
                        return -1;
                    }

                    index = commentEnd + 2;
                    continue;
                }

                if (source.AsSpan(index).StartsWith("<![CDATA[", StringComparison.Ordinal))
                {
                    int cdataEnd = source.IndexOf("]]>", index + 9, StringComparison.Ordinal);
                    if (cdataEnd < 0)
                    {
                        return -1;
                    }

                    index = cdataEnd + 2;
                    continue;
                }

                if (source.AsSpan(index).StartsWith($"</{elementName}", StringComparison.Ordinal))
                {
                    int closingEnd = source.IndexOf('>', index + 2 + elementName.Length);
                    if (closingEnd < 0)
                    {
                        return -1;
                    }

                    if (--nesting == 0)
                    {
                        return closingEnd + 1;
                    }

                    index = closingEnd;
                    continue;
                }

                if (TryReadElementStart(source, index, out string? nestedName, out int nestedTagEnd, out bool selfClosing)
                    && string.Equals(nestedName, elementName, StringComparison.Ordinal)
                    && !selfClosing)
                {
                    nesting++;
                    index = nestedTagEnd;
                }
                else if (TryReadElementStart(source, index, out _, out int tagEnd, out _))
                {
                    index = tagEnd;
                }
            }

            return -1;
        }

    private static AxamlPreviewNode ParseElement(XElement element, string source, int depth, ref int controlCount)
    {
        if (depth > MaximumTreeDepth)
        {
            throw new InvalidDataException($"AXAML preview supports at most {MaximumTreeDepth} nested controls.");
        }

        if (++controlCount > MaximumControls)
        {
            throw new InvalidDataException($"AXAML preview supports at most {MaximumControls} controls.");
        }

        string typeName = element.Name.LocalName;
        bool canAppendChildren = AxamlControlCatalog.TryGet(typeName, out AxamlControlCapability capability)
            && capability.AllowsAppendingChildren
            && (typeName != "DockPanel" || HasLiteralLastChildFill(element))
            && !element.Nodes().OfType<XText>().Any(static value => !string.IsNullOrWhiteSpace(value.Value))
            && element.Elements().All(static child =>
                !child.Name.LocalName.Contains('.', StringComparison.Ordinal)
                && LooksLikeControl(child)
                && IsSupportedElement(child, child.Name.LocalName));
        bool canAddGridChild = CanAddGridChild(element);
        Dictionary<string, string> properties = element.Attributes()
            .Where(static attribute => !attribute.IsNamespaceDeclaration && attribute.Name.NamespaceName.Length == 0)
            .Where(static attribute => !attribute.Name.LocalName.StartsWith("x:", StringComparison.Ordinal))
            .ToDictionary(
                static attribute => attribute.Name.LocalName,
                static attribute => attribute.Value,
                StringComparer.Ordinal);
        List<AxamlPreviewNode> children = [];
        List<string> rowDefinitions = GetDefinitionValues(properties, "RowDefinitions");
        List<string> columnDefinitions = GetDefinitionValues(properties, "ColumnDefinitions");
        string? textPropertyName = GetEditablePropertyName(typeName);
        bool hasTextPropertyElement = false;
        List<string> propertyElementNames = [];

        foreach (XElement child in element.Elements())
        {
            string childName = child.Name.LocalName;
            if (childName.StartsWith($"{typeName}.", StringComparison.Ordinal))
            {
                propertyElementNames.Add(childName);
            }

            if (textPropertyName is not null && childName == $"{typeName}.{textPropertyName}")
            {
                hasTextPropertyElement = true;
            }
            else if (childName == $"{typeName}.RowDefinitions")
            {
                rowDefinitions.AddRange(child.Elements().Select(static definition =>
                    (string?)definition.Attribute("Height") ?? "Auto"));
            }
            else if (childName == $"{typeName}.ColumnDefinitions")
            {
                columnDefinitions.AddRange(child.Elements().Select(static definition =>
                    (string?)definition.Attribute("Width") ?? "Auto"));
            }
            else if (childName.Contains('.', StringComparison.Ordinal))
            {
                foreach (XElement propertyChild in child.Elements())
                {
                    if (LooksLikeControl(propertyChild))
                    {
                        children.Add(ParseElement(propertyChild, source, depth + 1, ref controlCount));
                    }
                }
            }
            else if (LooksLikeControl(child))
            {
                children.Add(ParseElement(child, source, depth + 1, ref controlCount));
            }
        }

        if (IsContentRoot(typeName) && children.Count == 1)
        {
            return children[0];
        }

        string? text = GetDisplayText(typeName, properties, element);
        int sourceStartOffset = GetSourceStartOffset(element, source);
        return new AxamlPreviewNode
        {
            TypeName = typeName,
            Text = text,
            IsSupported = IsSupportedElement(element, typeName),
            SourceStartOffset = sourceStartOffset,
            SourceStartTag = sourceStartOffset >= 0
                ? source[sourceStartOffset..(FindStartTagEnd(source, sourceStartOffset) + 1)]
                : null,
            HasTextPropertyElement = hasTextPropertyElement,
            HasInlineText = element.Nodes().OfType<XText>().Any(static value => !string.IsNullOrWhiteSpace(value.Value)),
            HasChildElements = element.HasElements,
            CanAppendChildren = canAppendChildren,
            CanAddGridChild = canAddGridChild,
            PropertyElementNames = propertyElementNames,
            Properties = properties,
            Children = children,
            RowDefinitions = rowDefinitions,
            ColumnDefinitions = columnDefinitions,
        };
    }

    private static string? GetEditablePropertyName(string typeName)
    {
        return typeName switch
        {
            "TextBlock" or "TextBox" => "Text",
            "Button" or "CheckBox" or "RadioButton" or "Label" => "Content",
            _ => null,
        };
    }

    private static int GetSourceStartOffset(XElement element, string source)
    {
        if (element is not IXmlLineInfo lineInfo || !lineInfo.HasLineInfo())
        {
            return -1;
        }

        int line = 1;
        int lineStart = 0;
        for (int index = 0; index < source.Length && line < lineInfo.LineNumber; index++)
        {
            if (source[index] == '\n')
            {
                line++;
                lineStart = index + 1;
            }
        }

        if (line != lineInfo.LineNumber)
        {
            return -1;
        }

        int reportedOffset = lineStart + lineInfo.LinePosition - 1;
        int tagStart = source.LastIndexOf('<', Math.Min(reportedOffset, source.Length - 1));
        return tagStart >= lineStart ? tagStart : -1;
    }

    private static int FindStartTagEnd(string source, int tagStart)
    {
        char quote = '\0';
        for (int index = tagStart + 1; index < source.Length; index++)
        {
            char current = source[index];
            if (quote != '\0')
            {
                if (current == quote)
                {
                    quote = '\0';
                }
            }
            else if (current is '"' or '\'')
            {
                quote = current;
            }
            else if (current == '>')
            {
                return index;
            }
        }

        throw new InvalidDataException("The AXAML start tag is incomplete.");
    }

    private static int FindMatchingEndTag(string source, int startTagEnd, string qualifiedName)
    {
        int depth = 1;
        int index = startTagEnd + 1;
        while (index < source.Length)
        {
            int tagStart = source.IndexOf('<', index);
            if (tagStart < 0)
            {
                break;
            }

            if (source.AsSpan(tagStart).StartsWith("<!--", StringComparison.Ordinal))
            {
                int commentEnd = source.IndexOf("-->", tagStart + 4, StringComparison.Ordinal);
                if (commentEnd < 0)
                {
                    break;
                }

                index = commentEnd + 3;
                continue;
            }

            if (source.AsSpan(tagStart).StartsWith("<![CDATA[", StringComparison.Ordinal))
            {
                int cdataEnd = source.IndexOf("]]>", tagStart + 9, StringComparison.Ordinal);
                if (cdataEnd < 0)
                {
                    break;
                }

                index = cdataEnd + 3;
                continue;
            }

            if (tagStart + 1 < source.Length && source[tagStart + 1] == '?')
            {
                int instructionEnd = source.IndexOf("?>", tagStart + 2, StringComparison.Ordinal);
                if (instructionEnd < 0)
                {
                    break;
                }

                index = instructionEnd + 2;
                continue;
            }

            if (tagStart + 1 < source.Length && source[tagStart + 1] == '!')
            {
                int declarationEnd = FindStartTagEnd(source, tagStart);
                index = declarationEnd + 1;
                continue;
            }

            int tagEnd = FindStartTagEnd(source, tagStart);
            bool isEndTag = tagStart + 1 < source.Length && source[tagStart + 1] == '/';
            int nameStart = tagStart + (isEndTag ? 2 : 1);
            int nameEnd = nameStart;
            while (nameEnd < tagEnd
                && !char.IsWhiteSpace(source[nameEnd])
                && source[nameEnd] is not '/' and not '>')
            {
                nameEnd++;
            }

            string name = source[nameStart..nameEnd];
            int beforeEnd = tagEnd - 1;
            while (beforeEnd > tagStart && char.IsWhiteSpace(source[beforeEnd]))
            {
                beforeEnd--;
            }

            bool isSelfClosing = !isEndTag && source[beforeEnd] == '/';
            if (string.Equals(name, qualifiedName, StringComparison.Ordinal))
            {
                if (isEndTag)
                {
                    if (--depth == 0)
                    {
                        return tagStart;
                    }
                }
                else if (!isSelfClosing)
                {
                    depth++;
                }
            }

            index = tagEnd + 1;
        }

        throw new InvalidDataException("The AXAML container has no matching closing tag.");
    }

    private static int FindAttributeValueStart(
        string source,
        int tagStart,
        int tagEnd,
        string propertyName,
        out int attributeNameStart,
        out int attributeEnd)
    {
        attributeNameStart = -1;
        attributeEnd = -1;
        int index = tagStart + 1;
        while (index < tagEnd && !char.IsWhiteSpace(source[index]) && source[index] is not '/' and not '>')
        {
            index++;
        }

        while (index < tagEnd)
        {
            while (index < tagEnd && char.IsWhiteSpace(source[index]))
            {
                index++;
            }

            if (index >= tagEnd || source[index] == '/')
            {
                break;
            }

            int nameStart = index;
            while (index < tagEnd && !char.IsWhiteSpace(source[index]) && source[index] is not '=' and not '/' and not '>')
            {
                index++;
            }

            string attributeName = source[nameStart..index];
            while (index < tagEnd && char.IsWhiteSpace(source[index]))
            {
                index++;
            }

            if (index >= tagEnd || source[index] != '=')
            {
                continue;
            }

            index++;
            while (index < tagEnd && char.IsWhiteSpace(source[index]))
            {
                index++;
            }

            if (index >= tagEnd || source[index] is not ('"' or '\''))
            {
                continue;
            }

            char quote = source[index++];
            int valueStart = index;
            while (index < tagEnd && source[index] != quote)
            {
                index++;
            }

            if (attributeName == propertyName)
            {
                attributeNameStart = nameStart;
                attributeEnd = index < tagEnd ? index + 1 : -1;
                return valueStart;
            }
            index++;
        }

        return -1;
    }

    private static string EscapeAttributeValue(string value)
    {
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace("\r", "&#xD;", StringComparison.Ordinal)
            .Replace("\n", "&#xA;", StringComparison.Ordinal)
            .Replace("\t", "&#x9;", StringComparison.Ordinal);
    }

    private static bool LooksLikeControl(XElement element)
    {
        string name = element.Name.LocalName;
        return !name.Contains('.', StringComparison.Ordinal)
            && (AxamlControlCatalog.TryGet(name, out _) || name.Length > 0 && char.IsUpper(name[0]));
    }

    private static bool IsSupportedElement(XElement element, string typeName)
    {
        return (element.Name.Namespace == XNamespace.None || element.Name.Namespace == s_avaloniaNamespace)
            && AxamlControlCatalog.TryGet(typeName, out _);
    }

    private static bool CanAddGridChild(XElement element)
    {
        if (element.Name.LocalName != "Grid"
            || !IsSupportedElement(element, "Grid")
            || element.Nodes().OfType<XText>().Any(static text => !string.IsNullOrWhiteSpace(text.Value))
            || HasNamespacedGridAttribute(element, "RowDefinitions", "ColumnDefinitions"))
        {
            return false;
        }

        if (!IsSafeGridDefinitionList((string?)element.Attribute("RowDefinitions"))
            || !IsSafeGridDefinitionList((string?)element.Attribute("ColumnDefinitions")))
        {
            return false;
        }

        HashSet<string> definitionProperties = new(StringComparer.Ordinal);
        foreach (XElement child in element.Elements())
        {
            string childName = child.Name.LocalName;
            if (childName is "Grid.RowDefinitions" or "Grid.ColumnDefinitions")
            {
                if (child.Name.Namespace != element.Name.Namespace
                    || !definitionProperties.Add(childName)
                    || child.Nodes().OfType<XText>().Any(static text => !string.IsNullOrWhiteSpace(text.Value))
                    || child.Elements().Any(definition =>
                        definition.Name.Namespace != element.Name.Namespace
                        || definition.Name.LocalName != (childName == "Grid.RowDefinitions" ? "RowDefinition" : "ColumnDefinition")
                        || definition.HasElements
                        || definition.Nodes().OfType<XText>().Any(static text => !string.IsNullOrWhiteSpace(text.Value))
                        || HasNamespacedGridAttribute(
                            definition,
                            childName == "Grid.RowDefinitions" ? "Height" : "Width")
                        || !IsSafeGridLength((string?)definition.Attribute(
                            childName == "Grid.RowDefinitions" ? "Height" : "Width") ?? "Auto")))
                {
                    return false;
                }

                continue;
            }

            if (childName.Contains('.', StringComparison.Ordinal)
                || !LooksLikeControl(child)
                || !IsSupportedElement(child, childName)
                || HasNamespacedGridAttribute(child, "Grid.Row", "Grid.Column", "Grid.RowSpan", "Grid.ColumnSpan"))
            {
                return false;
            }
        }

        if ((element.Attribute("RowDefinitions") is not null && definitionProperties.Contains("Grid.RowDefinitions"))
            || (element.Attribute("ColumnDefinitions") is not null && definitionProperties.Contains("Grid.ColumnDefinitions")))
        {
            return false;
        }

        int rowCount = GetGridDefinitionCount(
            element,
            "RowDefinitions",
            "Grid.RowDefinitions",
            "RowDefinition");
        int columnCount = GetGridDefinitionCount(
            element,
            "ColumnDefinitions",
            "Grid.ColumnDefinitions",
            "ColumnDefinition");
        long gridCellCount = (long)rowCount * columnCount;
        if (gridCellCount > MaximumGridCells)
        {
            return false;
        }

        HashSet<(int Row, int Column)> occupiedCells = [];
        foreach (XElement child in element.Elements().Where(static child =>
            !child.Name.LocalName.Contains('.', StringComparison.Ordinal)))
        {
            if (!TryGetGridInteger(child, "Grid.Row", defaultValue: 0, minimum: 0, out int row)
                || !TryGetGridInteger(child, "Grid.Column", defaultValue: 0, minimum: 0, out int column)
                || !TryGetGridInteger(child, "Grid.RowSpan", defaultValue: 1, minimum: 1, out int rowSpan)
                || !TryGetGridInteger(child, "Grid.ColumnSpan", defaultValue: 1, minimum: 1, out int columnSpan)
                || !TryOccupyGridCells(occupiedCells, row, column, rowSpan, columnSpan, rowCount, columnCount))
            {
                return false;
            }
        }

        return (long)occupiedCells.Count < gridCellCount;
    }

    private static bool HasNamespacedGridAttribute(XElement element, params string[] attributeNames)
    {
        return element.Attributes().Any(attribute =>
            attribute.Name.NamespaceName.Length > 0
            && attributeNames.Contains(attribute.Name.LocalName, StringComparer.Ordinal));
    }

    private static bool HasLiteralLastChildFill(XElement element)
    {
        if (HasNamespacedGridAttribute(element, "LastChildFill"))
        {
            return false;
        }

        XAttribute? attribute = element.Attribute("LastChildFill");
        return attribute is null
            || string.Equals(attribute.Value, "True", StringComparison.OrdinalIgnoreCase)
            || string.Equals(attribute.Value, "False", StringComparison.OrdinalIgnoreCase);
    }

    private static int GetGridDefinitionCount(
        XElement grid,
        string attributeName,
        string propertyElementName,
        string definitionElementName)
    {
        if (grid.Attribute(attributeName) is { } attribute)
        {
            return CountGridDefinitionList(attribute.Value);
        }

        return Math.Max(
            1,
            grid.Elements()
                .Where(child => child.Name.LocalName == propertyElementName)
                .SelectMany(static property => property.Elements())
                .Count(definition => definition.Name.LocalName == definitionElementName));
    }

    private static int CountGridDefinitionList(string value)
    {
        int count = 0;
        int entryStart = 0;
        for (int index = 0; index <= value.Length; index++)
        {
            if (index != value.Length && value[index] != ',')
            {
                continue;
            }

            if (value.AsSpan(entryStart, index - entryStart).Trim().IsEmpty)
            {
                throw new InvalidOperationException("Grid definition lists cannot contain empty entries.");
            }

            count++;
            entryStart = index + 1;
        }

        return Math.Max(1, count);
    }

    private static bool TryGetGridInteger(
        XElement element,
        string propertyName,
        int defaultValue,
        int minimum,
        out int result)
    {
        XAttribute? attribute = element.Attribute(propertyName);
        if (attribute is null)
        {
            result = defaultValue;
            return true;
        }

        return int.TryParse(attribute.Value, NumberStyles.None, CultureInfo.InvariantCulture, out result)
            && result >= minimum;
    }

    private static bool TryOccupyGridCells(
        HashSet<(int Row, int Column)> occupiedCells,
        int row,
        int column,
        int rowSpan,
        int columnSpan,
        int rowCount,
        int columnCount)
    {
        if ((long)row + rowSpan > rowCount || (long)column + columnSpan > columnCount)
        {
            return false;
        }

        for (int currentRow = row; currentRow < row + rowSpan; currentRow++)
        {
            for (int currentColumn = column; currentColumn < column + columnSpan; currentColumn++)
            {
                if (!occupiedCells.Add((currentRow, currentColumn)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool IsSafeGridDefinitionList(string? definitions)
    {
        if (definitions is null)
        {
            return true;
        }

        string[] values = definitions.Split(',');
        return values.Length > 0 && values.All(IsSafeGridLength);
    }

    private static bool IsSafeGridLength(string value)
    {
        string trimmed = value.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('{'))
        {
            return false;
        }

        if (string.Equals(trimmed, "Auto", StringComparison.OrdinalIgnoreCase)
            || trimmed == "*")
        {
            return true;
        }

        bool isStarLength = trimmed.EndsWith('*');
        string numericPart = isStarLength ? trimmed[..^1].Trim() : trimmed;
        return double.TryParse(numericPart, NumberStyles.Float, CultureInfo.InvariantCulture, out double length)
            && double.IsFinite(length)
            && length >= 0;
    }

    private static string GetElementName(string startTag)
    {
        int nameStart = startTag.IndexOf('<') + 1;
        int nameEnd = nameStart;
        while (nameEnd < startTag.Length
            && !char.IsWhiteSpace(startTag[nameEnd])
            && startTag[nameEnd] is not '/' and not '>')
        {
            nameEnd++;
        }

        return startTag[nameStart..nameEnd];
    }

    private static string GetLineIndentation(string source, int offset)
    {
        int lineStart = source.LastIndexOf('\n', Math.Max(0, offset - 1)) + 1;
        int indentationEnd = lineStart;
        while (indentationEnd < offset && source[indentationEnd] is ' ' or '\t')
        {
            indentationEnd++;
        }

        return indentationEnd == offset ? source[lineStart..offset] : string.Empty;
    }

    private static bool IsContentRoot(string typeName)
    {
        return typeName is "Window" or "UserControl";
    }

    private static string? GetDisplayText(
        string typeName,
        IReadOnlyDictionary<string, string> properties,
        XElement element)
    {
        string? textProperty = typeName switch
        {
            "TextBlock" or "TextBox" => "Text",
            "Button" or "CheckBox" or "RadioButton" or "Label" => "Content",
            "Expander" or "GroupBox" => "Header",
            _ => null,
        };
        if (textProperty is not null && properties.TryGetValue(textProperty, out string? text))
        {
            return FormatMarkupValue(text);
        }

        if (typeName is "TextBlock" or "Label")
        {
            string? nodeText = element.Nodes()
                .OfType<XText>()
                .Select(static value => value.Value.Trim())
                .FirstOrDefault(static value => value.Length > 0);
            return nodeText is null ? null : FormatMarkupValue(nodeText);
        }

        return null;
    }

    private static string FormatMarkupValue(string value)
    {
        if (value.StartsWith("{Binding", StringComparison.Ordinal)
            || value.StartsWith("{StaticResource", StringComparison.Ordinal)
            || value.StartsWith("{DynamicResource", StringComparison.Ordinal))
        {
            return $"[{value.Trim('{', '}')}]";
        }

        return value;
    }

    private static List<string> GetDefinitionValues(
        IReadOnlyDictionary<string, string> properties,
        string propertyName)
    {
        if (!properties.TryGetValue(propertyName, out string? definitions)
            || string.IsNullOrWhiteSpace(definitions))
        {
            return [];
        }

        return definitions.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static (int Row, int Column) GetNextGridCell(AxamlPreviewNode grid)
    {
        if (!grid.CanAddGridChild || grid.HasInlineText)
        {
            throw new InvalidOperationException(
                "Grid insertion requires simple, supported direct children without property elements or inline text.");
        }

        int rowCount = grid.RowDefinitions.Count == 0 ? 1 : grid.RowDefinitions.Count;
        int columnCount = grid.ColumnDefinitions.Count == 0 ? 1 : grid.ColumnDefinitions.Count;
        HashSet<(int Row, int Column)> occupiedCells = [];
        foreach (AxamlPreviewNode child in grid.Children)
        {
            int row = GetGridIndex(child, "Grid.Row");
            int column = GetGridIndex(child, "Grid.Column");
            int rowSpan = GetGridSpan(child, "Grid.RowSpan");
            int columnSpan = GetGridSpan(child, "Grid.ColumnSpan");
            if (!TryOccupyGridCells(
                occupiedCells,
                row,
                column,
                rowSpan,
                columnSpan,
                rowCount,
                columnCount))
            {
                throw new InvalidOperationException(
                    "Grid insertion requires non-overlapping children with valid row, column, and span placements.");
            }
        }

        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                if (!occupiedCells.Contains((row, column)))
                {
                    return (row, column);
                }
            }
        }

        throw new InvalidOperationException("The selected Grid has no unoccupied cells.");
    }

    private static int GetGridIndex(AxamlPreviewNode node, string propertyName)
    {
        if (!node.Properties.TryGetValue(propertyName, out string? text))
        {
            return 0;
        }

        if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int index) && index >= 0)
        {
            return index;
        }

        throw new InvalidOperationException($"'{propertyName}' must be a non-negative integer for Grid insertion.");
    }

    private static int GetGridSpan(AxamlPreviewNode node, string propertyName)
    {
        if (!node.Properties.TryGetValue(propertyName, out string? text))
        {
            return 1;
        }

        if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int span) && span > 0)
        {
            return span;
        }

        throw new InvalidOperationException($"'{propertyName}' must be a positive integer for Grid insertion.");
    }
}
