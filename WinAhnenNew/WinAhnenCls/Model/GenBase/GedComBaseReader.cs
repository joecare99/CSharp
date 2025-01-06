using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace WinAhnenCls.Model.GenBase;

public class GedComBaseReader : TextReader
{
    public struct SGedComLine
    {
        // Field for the level
        public int iLevel;
        // Field for the GedCom-ID if there is any
        public string? sID;
        // Field for the GedCom-tag
        public string sTag;
        // Field for data 
        public string? sData;
        // Prepared field for cross-references
        public string? sXRef;
        // Prepared field for rest of line
        public string? sRest;

        public object?[] ToOArray()
        {
            var _l=new List<object?>() { iLevel, sID, sTag };
            if(sData!=null || sXRef!=null || sRest!=null) _l.Add(sData);
            if(sXRef!=null || sRest!=null) _l.Add(sXRef);
            if(sRest!=null) _l.Add(sRest);
            return _l.ToArray();
        }
    }

    StreamReader _reader;

    public static readonly GedComBaseReader Null;
    public GedComBaseReader(Stream stream)
    {
        _reader = new StreamReader(stream);
    }

    public override void Close()
    {
        _reader.Close();
    }

   public IEnumerable<SGedComLine> GetGedComLines()
    {
        while (!_reader.EndOfStream)
        {
            yield return ReadLine();
        }
    }

    public new SGedComLine ReadLine()
    {
        string? sLine = _reader.ReadLine();
        SGedComLine sRet = new SGedComLine();
        if (sLine == null)
            return sRet;

        var _sLine = sLine.Split(' ');
        if (_sLine.Length >= 2
            && int.TryParse(_sLine[0], out sRet.iLevel))
        {
            var _iTagCol = 1;
            if (_sLine[1].StartsWith("@"))
            {
                sRet.sID = _sLine[1];
                _iTagCol = 2;
            }
            else
                sRet.sID = null;
            sRet.sTag = _sLine[_iTagCol];
            sRet.sData = _sLine.Length > _iTagCol+1? string.Join(" ", _sLine.Skip(_iTagCol + 1)):null;
            
        }
        return sRet;
    }
}
