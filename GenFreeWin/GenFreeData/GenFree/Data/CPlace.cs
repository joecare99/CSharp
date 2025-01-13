using System;
using System.Linq;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces;
using GenFree.Model;
using GenFree.Helper;

namespace GenFree.Data;


public class CPlace : CUsesIndexedRSet<int,PlaceIndex,PlaceFields,IPlaceData>, IPlace
{
    private Func<IRecordset> _value;

    public CPlace(Func<IRecordset> value)
    {
        _value = value;
    }

    protected override IRecordset _db_Table => _value();

    protected override PlaceIndex _keyIndex => PlaceIndex.OrtNr;

    public void ForeEachTextDo(Func<int, string> onGetText, Action<int, string[]> onDo, Action<float, int>? onProgress = null)
    {
        IRecordset dB_PlaceTable = _db_Table;

        string[] Kont2 = new string[6];
        var iMax = MaxID;
        dB_PlaceTable.MoveFirst();
        while (!dB_PlaceTable.EOF)
        {
            int Place_iOrtNr = dB_PlaceTable.Fields[nameof(PlaceFields.OrtNr)].AsInt();
            onProgress?.Invoke(Place_iOrtNr / (float)iMax, Place_iOrtNr);
            foreach (var f in new[] {
                    PlaceFields.Ort,
                    PlaceFields.Ortsteil,
                    PlaceFields.Kreis,
                    PlaceFields.Land,
                    PlaceFields.Staat
                })
                Kont2[(int)f + 1] = onGetText(dB_PlaceTable.Fields[f.ToString()].AsInt());
            Kont2[(int)PlaceFields.Ortsteil + 1] = Kont2[(int)PlaceFields.Ortsteil + 1].FrameIfNEoW("-", "");

            Kont2[0] = string.Join(" ", Kont2.Where((s) => !string.IsNullOrWhiteSpace(s)));

            onDo.Invoke(Place_iOrtNr, Kont2);

            dB_PlaceTable.MoveNext();
        }
    }

    protected override int GetID(IRecordset recordset)
    {
        return _db_Table.Fields[nameof(PlaceFields.OrtNr)].AsInt();
    }

    public override PlaceFields GetIndex1Field(PlaceIndex eIndex) => eIndex switch
    {
        PlaceIndex.OrtNr => PlaceFields.OrtNr,
        PlaceIndex.Orte => PlaceFields.Ort,
        PlaceIndex.OT => PlaceFields.Ortsteil,
        PlaceIndex.K => PlaceFields.Kreis,
        PlaceIndex.L => PlaceFields.Land,
        PlaceIndex.S => PlaceFields.Staat,
        _ => throw new ArgumentException(nameof(eIndex)),
    };

    protected override IPlaceData GetData(IRecordset rs) => new CPlaceData(rs);
}
