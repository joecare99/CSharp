using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAhnenCls.Model.GenBase;
public class GedComReader
{
    private readonly GedComBaseReader _Reader;

    public GedComReader(Stream sr)
    {
        if (sr is null)
        {
            throw new ArgumentNullException(nameof(sr));
        }
        if (!sr.CanRead)
        {
            throw new ArgumentException("Stream not readable", nameof(sr));
        }
        _Reader = new GedComBaseReader(sr);
    }

    public IList<IGedComObjects> GetGedComObjects()
    {
        var lRet = new List<IGedComObjects>();
        IGedComObjects? gco = null;
        var sLines = _Reader.GetGedComLines().GetEnumerator();       
        while (sLines.MoveNext())
        {
            var sLine = sLines.Current;
            if (sLine.iLevel == 0)
            {
                if (gco != null)
                {
                    lRet.Add(gco);
                }
                gco = new GedComObject(sLine);
            }
            gco?.AddLine(sLine);
        }
        if (gco != null)
        {
            lRet.Add(gco);
        }
        return lRet;
    }
}
