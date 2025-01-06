namespace WinAhnenCls.Model.GenBase;

public class GedComObject
{
    private GedComBaseReader.SGedComLine _DefLine;

    public GedComObject(GedComBaseReader.SGedComLine sLine)
    {
        this._DefLine = sLine;
        _tag = $"Ged_{sLine.sTag}".AsEnum<EGedComTags>;
    }
}