using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace WinAhnenCls.Model.GenBase;
public class GedComReader
{
    private readonly GedComBaseReader _Reader;

    Func<GedComBaseReader.SGedComLine, IGedComObjects> GetObj { get; set; }=(s) => new GedComObject(s);

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
                gco = GetObj?.Invoke(sLine);
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
