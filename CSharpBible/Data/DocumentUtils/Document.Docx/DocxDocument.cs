using Document.Base.Models.Interfaces;
using Document.Base.Models;
using Document.Base.Registration;
using Document.Docx.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Document.Docx;

[UserDocumentProvider("docx", Extensions = new[] { ".docx" }, ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document", DisplayName = "Word (DocX)")]
public sealed class DocxDocument : IUserDocument, IDocSectionProvider
{
    private bool _isModified;
    private readonly List<PendingHyperlinkStyle> _pendingHyperlinkStyles = new();

    public DocxDocument() => Root = new DocxSection();

    public IDocElement Root { get; private set; }
    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    { var section = EnsureRoot(); _isModified = true; return section.AddParagraph(cStylename); }

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    { var section = EnsureRoot(); _isModified = true; return section.AddHeadline(nLevel, Id ?? Guid.NewGuid().ToString("N")); }

    public IDocTOC AddTOC(string cName, int nLevel)
    { var section = EnsureRoot(); _isModified = true; return section.AddTOC(cName, nLevel); }

    /// <summary>
    /// Adds a real document section with an optional column count.
    /// </summary>
    public DocxSection AddSection(int? columns = null)
    {
        if (columns is < 1 or > 4)
            throw new ArgumentOutOfRangeException(nameof(columns), "A section must have between one and four columns.");

        var section = new DocxSection(columns);
        EnsureRoot().AddChild(section);
        _isModified = true;
        return section;
    }

    IDocElement IDocSectionProvider.AddSection(int? columns) => AddSection(columns);

    public IEnumerable<IDocElement> Enumerate() => Root.Enumerate();

    public bool SaveTo(string cOutputPath)
    {
        try
        {
            var columnCounts = new List<int> { 1 };
            using (var doc = DocX.Create(cOutputPath))
            {
                _pendingHyperlinkStyles.Clear();
                BuildDocX(doc, EnsureRoot(), columnCounts);
                doc.Save();
            }

            ApplyHyperlinkStyles(cOutputPath, _pendingHyperlinkStyles);
            ApplyColumnCounts(cOutputPath, columnCounts);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SaveTo(Stream sOutputStream, object? options = null)
    {
        try
        {
            using var buffer = new MemoryStream();
            var columnCounts = new List<int> { 1 };
            _pendingHyperlinkStyles.Clear();
            using (var doc = DocX.Create(buffer))
            {
                BuildDocX(doc, EnsureRoot(), columnCounts);
                doc.Save();
            }

            ApplyHyperlinkStyles(buffer, _pendingHyperlinkStyles);
            ApplyColumnCounts(buffer, columnCounts);
            buffer.Position = 0;
            if (sOutputStream.CanSeek)
            {
                sOutputStream.Position = 0;
                sOutputStream.SetLength(0);
            }
            buffer.CopyTo(sOutputStream);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool LoadFrom(string cInputPath)
    { try { using var _ = DocX.Load(cInputPath); Root = new DocxSection(); _isModified = false; return true; } catch { return false; } }

    public bool LoadFrom(Stream sInputStream, object? options = null)
    { try { using var _ = DocX.Load(sInputStream); Root = new DocxSection(); _isModified = false; return true; } catch { return false; } }

    private DocxSection EnsureRoot() => Root as DocxSection ?? throw new InvalidOperationException("Root ist nicht DocxSection");

    private static void ApplyRunFormatting(Formatting fmt, IList<IDocAttributes> attributes)
    {
        var bold = GetAttribute(attributes, DocAttributeNames.Bold, false);
        var italic = GetAttribute(attributes, DocAttributeNames.Italic, false);
        var underline = GetAttribute(attributes, DocAttributeNames.Underline, false);
        var strikeout = GetAttribute(attributes, DocAttributeNames.Strikeout, false);
        var fontFamily = GetAttribute<string?>(attributes, DocAttributeNames.FontFamily, null);
        var fontSize = GetAttribute<double?>(attributes, DocAttributeNames.FontSizePt, null);
        if (bold)
            fmt.Bold = true;
        if (italic)
            fmt.Italic = true;
        if (underline)
            fmt.UnderlineStyle = UnderlineStyle.singleLine;
        if (strikeout)
            fmt.StrikeThrough = StrikeThrough.strike;
        if (!string.IsNullOrWhiteSpace(fontFamily))
            fmt.FontFamily = new Font(fontFamily);
        if (fontSize is double size)
            fmt.Size = (float)size;
    }

    private static T GetAttribute<T>(IList<IDocAttributes> attributes, string name, T fallback)
    {
        var attribute = attributes.LastOrDefault(item => item.Name == name);
        if (attribute?.Value is T value)
            return value;
        if (attribute?.Value is not null && typeof(T) == typeof(bool) && bool.TryParse(attribute.Value.ToString(), out var boolean))
            return (T)(object)boolean;
        if (attribute?.Value is not null && typeof(T) == typeof(double?) && double.TryParse(attribute.Value.ToString(), out var number))
            return (T)(object)(double?)number;
        return fallback;
    }

    private void BuildDocX(DocX doc, DocxSection root, List<int> columnCounts)
    {
        foreach (var node in root.Nodes)
        {
            if (node is DocxSection section)
            {
                doc.InsertSection(trackChanges: false);
                var activeColumns = section.Columns ?? columnCounts[^1];
                columnCounts.Add(activeColumns);
                BuildDocX(doc, section, columnCounts);
                continue;
            }

            switch (node)
            {
                case DocxParagraph p:
                    {
                        var par = doc.InsertParagraph();
                        var styleName = GetDocxStyleName(p.StyleName);
                        if (!string.IsNullOrWhiteSpace(styleName))
                            par.StyleId = styleName;
                        var indentBefore = GetAttribute<int?>(p.DocAttributes, DocAttributeNames.IndentationBefore, null);
                        var hangingIndentPoints = GetAttribute<int?>(p.DocAttributes, DocAttributeNames.IndentationHanging, null);
                        var firstLineIndent = GetAttribute<int?>(p.DocAttributes, DocAttributeNames.IndentationFirstLine, null);
                        if (indentBefore is int before)
                            par.IndentationBefore = before;
                        if (hangingIndentPoints is int hanging)
                        {
                            par.IndentationBefore = indentBefore ?? hanging;
                            par.IndentationHanging = hanging;
                        }
                        if (firstLineIndent is int firstLine)
                            par.IndentationFirstLine = firstLine;
                        if (!string.IsNullOrEmpty(p.TextContent))
                            par.Append(p.TextContent);
                        foreach (var child in p.Nodes)
                        {
                            if (child is DocxSpan sp)
                            {
                                var fmt = new Formatting();
                                ApplyRunFormatting(fmt, sp.DocAttributes);
                                if (sp.IsLink && !string.IsNullOrEmpty(sp.Href))
                                {
                                    if (sp.Href!.StartsWith("#", StringComparison.Ordinal))
                                    {
                                        var internalLink = doc.AddHyperlink(sp.TextContent ?? sp.Href, sp.Href[1..]);
                                        _pendingHyperlinkStyles.Add(new PendingHyperlinkStyle(sp.Href[1..], sp.TextContent ?? string.Empty, sp.DocAttributes.ToArray()));
                                        par.AppendHyperlink(internalLink);
                                    }
                                    else
                                    {
                                        var hyperlink = doc.AddHyperlink(sp.TextContent ?? sp.Href!, new Uri(sp.Href!, UriKind.RelativeOrAbsolute));
                                        _pendingHyperlinkStyles.Add(new PendingHyperlinkStyle(sp.Href!, sp.TextContent ?? string.Empty, sp.DocAttributes.ToArray()));
                                        par.AppendHyperlink(hyperlink);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(sp.Id))
                                        par.AppendBookmark(sp.Id);
                                    AppendFormattedRun(par, sp.TextContent ?? string.Empty, sp.DocAttributes, fmt);
                                }
                            }
                            else if (child is DocxLineBreak)
                            {
                                par.AppendLine(string.Empty);
                            }
                            else if (child is DocxNbSpace)
                            {
                                par.Append("\u00A0");
                            }
                            else if (child is DocxTab)
                            {
                                par.Append("\t");
                            }
                        }
                        break;
                    }
                case DocxHeadline h:
                    {
                        var par = doc.InsertParagraph();
                        if (!string.IsNullOrWhiteSpace(h.Id))
                            par.AppendBookmark(h.Id);
                        if (!string.IsNullOrEmpty(h.TextContent))
                            par.Append(h.TextContent);
                        par.StyleId = $"Heading{Math.Clamp(h.Level, 1, 9)}";
                        break;
                    }
                case DocxTOC toc:
                    {
                        var switches = new Dictionary<TableOfContentsSwitches, string?>
                    {
                        { TableOfContentsSwitches.O, $"\"1-{Math.Clamp(toc.Level,1,9)}\"" },
                        { TableOfContentsSwitches.H, string.Empty },
                        { TableOfContentsSwitches.Z, string.Empty },
                        { TableOfContentsSwitches.U, string.Empty }
                    };

                        doc.InsertTableOfContents("Table of Contents", switches);
                        break;
                    }
            }
        }
    }

    private static string GetDocxStyleName(string logicalStyleName) => logicalStyleName switch
    {
                    "section-heading" => "Heading1",
                    "family-header" => "Heading3",
                    "family-data" or "adult" or "children-header" or "child" or "note" => "Normal",
                    "Normaler Absatz" => "Normal",
                    _ => logicalStyleName
    };

    private static void ApplyHyperlinkStyles(string path, IReadOnlyList<PendingHyperlinkStyle> styles)
    {
        if (styles.Count == 0)
            return;
        using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        ApplyHyperlinkStyles(stream, styles);
    }

    private static void ApplyHyperlinkStyles(Stream stream, IReadOnlyList<PendingHyperlinkStyle> styles)
    {
        if (styles.Count == 0)
            return;
        stream.Position = 0;
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update, leaveOpen: true);
        var entry = archive.GetEntry("word/document.xml")
            ?? throw new InvalidDataException("The generated DOCX has no word/document.xml part.");
        XDocument document;
        using (var source = entry.Open())
            document = XDocument.Load(source);

        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        XNamespace relationships = "http://schemas.openxmlformats.org/package/2006/relationships";
        XNamespace relationshipIds = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        var relationshipsEntry = archive.GetEntry("word/_rels/document.xml.rels");
        var relationshipTargets = new Dictionary<string, string>(StringComparer.Ordinal);
        if (relationshipsEntry is not null)
        {
            using var relationshipsStream = relationshipsEntry.Open();
            var relationshipDocument = XDocument.Load(relationshipsStream);
            foreach (var relationship in relationshipDocument.Descendants(relationships + "Relationship"))
            {
                var id = (string?)relationship.Attribute("Id");
                var target = (string?)relationship.Attribute("Target");
                if (id is not null && target is not null)
                    relationshipTargets[id] = target;
            }
        }

        foreach (var hyperlink in document.Descendants(word + "hyperlink"))
        {
            var anchor = (string?)hyperlink.Attribute(word + "anchor");
            var relationshipId = (string?)hyperlink.Attribute(relationshipIds + "id");
            var target = anchor;
            if (target is null && relationshipId is not null)
                relationshipTargets.TryGetValue(relationshipId, out target);
            var text = string.Concat(hyperlink.Descendants(word + "t").Select(node => node.Value));
            var style = styles.FirstOrDefault(candidate => candidate.Target == target && candidate.Text == text);
            if (style is null)
                continue;
            foreach (var run in hyperlink.Elements(word + "r"))
                ApplyRunAttributes(run, style.Attributes, word);
        }

        entry.Delete();
        var replacement = archive.CreateEntry("word/document.xml", CompressionLevel.Optimal);
        using var destination = replacement.Open();
        document.Save(destination);
    }

    private static void ApplyRunAttributes(XElement run, IList<IDocAttributes> attributes, XNamespace word)
    {
        var properties = run.Element(word + "rPr");
        if (properties is null)
        {
            properties = new XElement(word + "rPr");
            run.AddFirst(properties);
        }

        if (GetAttribute(attributes, DocAttributeNames.Bold, false))
            properties.Add(new XElement(word + "b"));
        if (GetAttribute(attributes, DocAttributeNames.Italic, false))
            properties.Add(new XElement(word + "i"));
        if (GetAttribute(attributes, DocAttributeNames.Underline, false))
            properties.Add(new XElement(word + "u", new XAttribute(word + "val", "single")));
        if (GetAttribute(attributes, DocAttributeNames.Strikeout, false))
            properties.Add(new XElement(word + "strike"));

        if (GetAttribute<double?>(attributes, DocAttributeNames.FontSizePt, null) is double fontSize)
            properties.Add(new XElement(word + "sz", new XAttribute(word + "val", Math.Round(fontSize * 2).ToString(System.Globalization.CultureInfo.InvariantCulture))));
        if (GetAttribute<string?>(attributes, DocAttributeNames.FontFamily, null) is string fontFamily)
            properties.Add(new XElement(word + "rFonts", new XAttribute(word + "ascii", fontFamily), new XAttribute(word + "hAnsi", fontFamily)));
        if (GetAttribute<string?>(attributes, DocAttributeNames.Color, null) is string color)
            properties.Add(new XElement(word + "color", new XAttribute(word + "val", color.TrimStart('#'))));
    }

    private sealed record PendingHyperlinkStyle(string Target, string Text, IList<IDocAttributes> Attributes);

    private static void AppendFormattedRun(Paragraph paragraph, string text, IList<IDocAttributes> attributes, Formatting formatting)
    {
        var run = paragraph.Append(text, formatting);
        var color = GetAttribute<string?>(attributes, DocAttributeNames.Color, null);
        if (string.IsNullOrWhiteSpace(color))
            return;

        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var runProperties = run.Xml.Element(word + "rPr");
        if (runProperties is null)
        {
            runProperties = new XElement(word + "rPr");
            run.Xml.AddFirst(runProperties);
        }
        if (GetAttribute(attributes, DocAttributeNames.Bold, false))
            runProperties.Add(new XElement(word + "b"));
        if (GetAttribute(attributes, DocAttributeNames.Italic, false))
            runProperties.Add(new XElement(word + "i"));
        if (GetAttribute(attributes, DocAttributeNames.Underline, false))
            runProperties.Add(new XElement(word + "u", new XAttribute(word + "val", "single")));
        if (GetAttribute(attributes, DocAttributeNames.Strikeout, false))
            runProperties.Add(new XElement(word + "strike"));
        runProperties.Add(new XElement(word + "color", new XAttribute(word + "val", color.TrimStart('#'))));
    }

    private static void ApplyColumnCounts(string path, IReadOnlyList<int> columnCounts)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        ApplyColumnCounts(stream, columnCounts);
    }

    private static void ApplyColumnCounts(Stream stream, IReadOnlyList<int> columnCounts)
    {
        if (columnCounts.Count == 1)
            return;

        stream.Position = 0;
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update, leaveOpen: true);
        var entry = archive.GetEntry("word/document.xml")
            ?? throw new InvalidDataException("The generated DOCX has no word/document.xml part.");
        XDocument document;
        using (var source = entry.Open())
            document = XDocument.Load(source);

        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var sectionProperties = document.Descendants(word + "sectPr").ToArray();
        if (sectionProperties.Length != columnCounts.Count)
            throw new InvalidDataException(
                $"The generated DOCX contains {sectionProperties.Length} section definitions for {columnCounts.Count} requested sections.");

        for (var index = 0; index < sectionProperties.Length; index++)
        {
            var properties = sectionProperties[index];
            var columns = properties.Element(word + "cols");
            if (columns is null)
            {
                columns = new XElement(word + "cols");
                properties.Add(columns);
            }

            columns.SetAttributeValue(word + "num", columnCounts[index].ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        entry.Delete();
        var replacement = archive.CreateEntry("word/document.xml", CompressionLevel.Optimal);
        using var destination = replacement.Open();
        document.Save(destination);
    }

}
