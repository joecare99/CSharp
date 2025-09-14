using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document.Base.Models.Interfaces;

public interface IDocSpan: IDocContent
{
    void SetStyle(object fs);
    void SetStyle(IDocFontStyle fs);
    void SetStyle(IUserDocument doc, object aFont);
    void SetStyle(IUserDocument doc, IDocFontStyle aFont);
    void SetStyle(string aStyleName);
}
