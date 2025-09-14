using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.Base.Models.Interfaces;

public interface IDocParagraph : IDocContent
{
    IDocSpan AddBookmark(IDocFontStyle docFontStyle);
}
