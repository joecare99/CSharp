using System;
using System.Collections.Generic;
using System.IO;
using Document.Base.Models.Interfaces;
using Document.Base.Registration;
using Document.Docx.Model;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Document.Docx;

[UserDocumentProvider("docx", Extensions = new[]{ ".docx" }, ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document", DisplayName = "Word (DocX)")]
public sealed class DocxDocument : IUserDocument
{
    private bool _isModified;

    public DocxDocument() => Root = new DocxSection();

    public IDocElement Root { get; private set; }
    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    { var section = EnsureRoot(); _isModified = true; return section.AddParagraph(cStylename); }

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    { var section = EnsureRoot(); _isModified = true; return section.AddHeadline(nLevel, Id ?? Guid.NewGuid().ToString("N")); }

    public IDocTOC AddTOC(string cName, int nLevel)
    { var section = EnsureRoot(); _isModified = true; return section.AddTOC(cName, nLevel); }

    public IEnumerable<IDocElement> Enumerate() => Root.Enumerate();

    public bool SaveTo(string cOutputPath)
    { try { using var doc = DocX.Create(cOutputPath); BuildDocX(doc, EnsureRoot()); doc.Save(); _isModified = false; return true; } catch { return false; } }

    public bool SaveTo(Stream sOutputStream, object? options = null)
    { try { using var doc = DocX.Create(sOutputStream); BuildDocX(doc, EnsureRoot()); doc.Save(); _isModified = false; return true; } catch { return false; } }

    public bool LoadFrom(string cInputPath)
    { try { using var _ = DocX.Load(cInputPath); Root = new DocxSection(); _isModified = false; return true; } catch { return false; } }

    public bool LoadFrom(Stream sInputStream, object? options = null)
    { try { using var _ = DocX.Load(sInputStream); Root = new DocxSection(); _isModified = false; return true; } catch { return false; } }

    private DocxSection EnsureRoot() => Root as DocxSection ?? throw new InvalidOperationException("Root ist nicht DocxSection");

    private static void ApplyRunFormatting(Formatting fmt, IDocFontStyle style)
    {
        if (style.Bold) fmt.Bold = true;
        if (style.Italic) fmt.Italic = true;
        if (style.Underline) fmt.UnderlineStyle = UnderlineStyle.singleLine;
        if (style.Strikeout) fmt.StrikeThrough = StrikeThrough.strike;
        if (!string.IsNullOrWhiteSpace(style.FontFamily)) fmt.FontFamily = new Font(style.FontFamily);
        if (style.FontSizePt is double sz) fmt.Size = (float)sz;
    }

    private static void BuildDocX(DocX doc, DocxSection root)
    {
        foreach (var node in root.Nodes)
        {
            switch (node)
            {
                case DocxParagraph p:
                {
                    var par = doc.InsertParagraph();
                    if (!string.IsNullOrEmpty(p.TextContent)) par.Append(p.TextContent);
                    foreach (var child in p.Nodes)
                    {
                        if (child is DocxSpan sp)
                        {
                            var fmt = new Formatting();
                            ApplyRunFormatting(fmt, sp.Style);
                            if (sp.IsLink && !string.IsNullOrEmpty(sp.Href))
                            {
                                var hyperlink = doc.AddHyperlink(sp.TextContent ?? sp.Href!, new Uri(sp.Href!, UriKind.RelativeOrAbsolute));
                                par.AppendHyperlink(hyperlink);
                            }
                            else
                            {
                                par.Append(sp.TextContent ?? string.Empty, fmt);
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
                    if (!string.IsNullOrEmpty(h.TextContent)) par.Append(h.TextContent);
                    par.StyleName = $"Heading {Math.Clamp(h.Level,1,9)}";
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
}
