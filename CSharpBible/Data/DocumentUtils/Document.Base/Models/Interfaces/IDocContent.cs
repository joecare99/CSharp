using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.Base.Models.Interfaces;

public interface IDocContent : IDocElement
{
    /// <summary>
    /// Gets or sets the content of the text.
    /// </summary>
    /// <value>The content of the text.</value>
    string TextContent { get; set; }
    /// <summary>
    /// Appends the text.
    /// </summary>
    /// <param name="text">The text.</param>
    void AppendText(string text);
    /// <summary>
    /// 1
    /// </summary>
    /// <remarks>3</remarks>
    /// <returns>2</returns>
    IDocContent AddLineBreak();
    IDocContent AddNBSpace(IDocFontStyle docFontStyle);
    IDocContent AddTab(IDocFontStyle docFontStyle);
    IDocSpan AddSpan(IDocFontStyle docFontStyle);
    IDocSpan AddSpan(string text,IList<object> docFontStyle);
    IDocSpan AddSpan(string text,IDocFontStyle docFontStyle);
    IDocSpan AddSpan(string text, EFontStyle eFontStyle);
    IDocSpan AddLink(string Href,IDocFontStyle docFontStyle);

    IDocStyleStyle GetStyle();

    string GetTextContent(bool xRecursive = true);
}
