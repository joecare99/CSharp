using BaseLib.Helper;

namespace GenFree.Data.Models;

public record struct TGedLine(int iLvl = -1, string sTag = "", string? link = null, string? sData = null)
{
    public TGedLine(string v) : this(-1, "", null, null)
    {
        var eing2 = v.Split(' ');
        var _iLvl = eing2[0].AsInt();
        if (eing2[0] != _iLvl.ToString())
        {
            sData = v;
            return;
        }
        else
        {
            iLvl = _iLvl;
        }
        int iPrse = 1;
        if (eing2.Length > iPrse && eing2[iPrse].StartsWith("@"))
        {
            link = eing2[iPrse++];
        }
        if (eing2.Length > iPrse)
        {
            sTag = eing2[iPrse++];
        }
        while (eing2.Length > iPrse)
        {
            sData = sData == null ? eing2[iPrse] : sData + " " + eing2[iPrse];
            iPrse++;
        }
    }

    public void SetTag(string sTag) => this.sTag= sTag;
    public void SetLvl(int iLvl) => this.iLvl= iLvl;
    public void SetData(string data) => sData= data;

    

    public readonly (int,string) tLvlTag => (iLvl,sTag);

    public static implicit operator (int iLvl, string? link, string sTag, string? sData)(TGedLine value)
    {
        return (value.iLvl, value.link, value.sTag, value.sData);
    }

    public static implicit operator TGedLine((int iLvl, string? link, string sTag, string? sData) value)
    {
        return new TGedLine(value.iLvl, value.sTag, value.link, value.sData);
    }

    public override string ToString()
    {
        return $"{iLvl} {(link == null ? "" : link+" ")}{sTag}{(sData==null?"":" "+sData)}";
    }

    public static implicit operator string(TGedLine value)
    {
        return value.ToString();
    }

    public string Trim()
    {
        return ToString().Trim();
    }

    public string TrimEnd()
    {
        return ToString().TrimEnd();
    }

    public int Length => ToString().Length;
}