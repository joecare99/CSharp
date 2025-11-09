using Document.Base.Models;
using Document.Base.Models.Interfaces;
using PdfSharp.Pdf.Advanced;
using System.Globalization;

namespace Document.Pdf.Model;

public class PdfTOC : PdfContentBase, IDocTOC
{
    public string Name { get; }
    public int Level { get; }
    public PdfTOC(string name, int level)
    {
        Name = name;
        Level = Math.Clamp(level, 1, 6);
    }
    public override IDocStyleStyle GetStyle() => new PdfStyle("TOC");
    
    /// <summary>
    /// Rebuilds the table of contents by extracting headings from the specified document section.
    /// </summary>
    /// <remarks>This method clears any existing content and child elements before rebuilding the table of
    /// contents. Only headings with a level less than or equal to the current level are included.</remarks>
    /// <param name="root">The root document section from which headings are collected to generate the table of contents. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="root"/> is null.</exception>
    public void RebuildFrom(IDocSection root)
    {
        if (root is null) throw new ArgumentNullException(nameof(root));

        // Inhalt zurücksetzen (Kinder und Text)
        _children.Clear();
        TextContent = string.Empty;

        // Alle Überschriften einsammeln, bevor etwas zur TOC hinzugefügt wird (vermeidet Rekursion)
        var headings = root.Enumerate().OfType<IDocHeadline>()
            .Where(h => h.Level <= Level)
            .ToList();

        AddSpan(Name,EFontStyle.Bold);
        AddLineBreak();

        // Einfache Ausgabe: Einzug per Spaces, Text, dann Zeilenumbruch .. als Platzhalter
        foreach (var h in headings)
        {
            if (h.Level > 1)
            {
                var indent = new string(' ', (h.Level - 1) * 2);
                AddSpan(indent, EFontStyle.Default);
            }

            AddSpan(h.GetTextContent(), EFontStyle.Default);

            AddLineBreak();
        }
    }
}
