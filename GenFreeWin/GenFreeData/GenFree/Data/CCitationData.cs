using GenFree.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.GenFree.Data;

public class CCitationData : ICitationData
{
    public string this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int iSourceId { get => field; set => field = value; }
    public short iSourceKnd { get ; set => throw new NotImplementedException(); }
    public string sSourceTitle { get ; set => throw new NotImplementedException(); }
    public string sPage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string sEntry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string sOriginalText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string sComment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Clear()
    {
        iSourceId = 0;
        iSourceKnd = 0;
        sSourceTitle = string.Empty;
        sPage = string.Empty;
        sEntry = string.Empty;
        sOriginalText = string.Empty;
        sComment = string.Empty;
    }

    public void Commit(int iPerFamNr)
    {
        throw new NotImplementedException();
    }
}
