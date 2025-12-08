using BaseLib.Helper;
using GenInterfaces.Data;
namespace WinAhnenCls.Model.GenBase;

public class GedComObject : IGedComObjects
{
    private GedComBaseReader.SGedComLine _DefLine;
    private EGedComTags _tag;

    public GedComObject(GedComBaseReader.SGedComLine sLine)
    {
        this._DefLine = sLine;
        _tag = $"Ged_{sLine.sTag}".AsEnum<EGedComTags>();
    }

    public void AddLine(GedComBaseReader.SGedComLine sLine)
    {
        throw new System.NotImplementedException();
    }
}