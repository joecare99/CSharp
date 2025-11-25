using Document.Base.Models.Interfaces;
using Document.Base.Registration;
using Document.Flow.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Document.Flow;

[UserDocumentProvider("FlowDocument", Extensions = new[] { ".xaml" }, ContentType = "application/xaml+xml")]
public class FlowDocumentWrapper : IUserDocument
{
    private FlowDocument _doc = new();

    public bool IsModified { get; private set; }

    // This is a simplified representation. A full implementation would require a wrapper model
    // that bridges the gap between the abstract IDocElement and the concrete FlowDocument model.
    public IDocElement Root => throw new NotImplementedException();

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    {
        var h = new Headline(nLevel);
        _doc.Blocks.Add(h);
        IsModified = true;
        // This is a simplified implementation. A real implementation would need
        // to return a wrapper that implements IDocHeadline and maps to the new Headline class.
        throw new NotImplementedException();
    }

    public IDocParagraph AddParagraph(string cStylename)
    {
        var p = new Paragraph();
        _doc.Blocks.Add(p);
        IsModified = true;
        // This is a simplified implementation. A real implementation would need
        // to return a wrapper that implements IDocParagraph and maps to the new Paragraph class.
        throw new NotImplementedException();
    }

    public IDocTOC AddTOC(string cName, int nLevel)
    {
        // Not implemented in this simplified model.
        throw new NotImplementedException();
    }

    public IEnumerable<IDocElement> Enumerate()
    {
        // Not implemented in this simplified model.
        yield break;
    }

    public bool LoadFrom(Stream sInputStream, object? options = null)
    {
        try
        {
            var xmlDoc = XDocument.Load(sInputStream);
            var newDoc = new FlowDocument();
            
            // Basic deserialization logic
            foreach (var element in xmlDoc.Root?.Elements() ?? Enumerable.Empty<XElement>())
            {
                switch (element.Name.LocalName)
                {
                    case "Paragraph":
                        var p = new Paragraph();
                        foreach (var rElement in element.Elements("Run"))
                        {
                            p.Inlines.Add(new Run(rElement.Value));
                        }
                        newDoc.Blocks.Add(p);
                        break;
                    case "Headline":
                        var h = new Headline(int.TryParse(element.Attribute("Level")?.Value, out var level) ? level : 1);
                        foreach (var rElement in element.Elements("Run"))
                        {
                            h.Inlines.Add(new Run(rElement.Value));
                        }
                        newDoc.Blocks.Add(h);
                        break;
                }
            }

            _doc = newDoc;
            IsModified = false;
            return true;
        }
        catch
        {
            // Loading failed
            return false;
        }
    }

    public bool LoadFrom(string cInputPath)
    {
        var extension = Path.GetExtension(cInputPath).ToLowerInvariant();
        if (extension != ".xaml") return false; // Only support for .xaml for now

        using var fs = new FileStream(cInputPath, FileMode.Open, FileAccess.Read);
        return LoadFrom(fs);
    }

    public bool SaveTo(Stream sOutputStream, object? options = null)
    {
        try
        {
            // Basic serialization logic
            var root = new XElement("FlowDocument");
            foreach (var block in _doc.Blocks)
            {
                if (block is Paragraph p)
                {
                    var pElement = new XElement("Paragraph");
                    foreach (var inline in p.Inlines)
                    {
                        if (inline is Run r)
                        {
                            pElement.Add(new XElement("Run", r.Text));
                        }
                    }
                    root.Add(pElement);
                }
                else if (block is Headline h)
                {
                    var hElement = new XElement("Headline", new XAttribute("Level", h.Level));
                    foreach (var inline in h.Inlines)
                    {
                        if (inline is Run r)
                        {
                            hElement.Add(new XElement("Run", r.Text));
                        }
                    }
                    root.Add(hElement);
                }
            }

            var xmlDoc = new XDocument(root);
            xmlDoc.Save(sOutputStream);
            IsModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SaveTo(string cOutputPath)
    {
        var extension = Path.GetExtension(cOutputPath).ToLowerInvariant();
        if (extension != ".xaml") return false; // Only support for .xaml for now

        using var fs = new FileStream(cOutputPath, FileMode.Create, FileAccess.Write);
        return SaveTo(fs);
    }
}
