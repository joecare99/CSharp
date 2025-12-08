using System;
using System.Linq;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Helper;
using BaseLib.Helper;
using GenFree.Interfaces.Data;
using GenFree.Models;

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

        string[] Kont2 = new string[7];
        var iMax = MaxID;
        dB_PlaceTable.MoveFirst();
        while (!dB_PlaceTable.EOF)
        {
            int Place_iOrtNr = dB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
            onProgress?.Invoke(Place_iOrtNr / (float)iMax, Place_iOrtNr);
            foreach (var f in new[] {
                    PlaceFields.Ort,
                    PlaceFields.Ortsteil,
                    PlaceFields.Kreis,
                    PlaceFields.Land,
                    PlaceFields.Staat
                })
                Kont2[(int)f + 1] = onGetText(dB_PlaceTable.Fields[f].AsInt());
            Kont2[(int)PlaceFields.Ortsteil + 1] = Kont2[(int)PlaceFields.Ortsteil + 1].FrameIfNEoW("-", "");

            Kont2[0] = string.Join(" ", Kont2.Where((s) => !string.IsNullOrWhiteSpace(s)));

            onDo.Invoke(Place_iOrtNr, Kont2);

            dB_PlaceTable.MoveNext();
        }
    }

    protected override int GetID(IRecordset recordset)
    {
        return _db_Table.Fields[PlaceFields.OrtNr].AsInt();
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

    protected override IPlaceData GetData(IRecordset rs, bool xNoInit = false) => new CPlaceData(rs,xNoInit);

    public bool ReadIdxData(PlaceIndex eIdx, object value, out IPlaceData? cPlace)
    {
        cPlace = null;
        IRecordset rs = _db_Table;
        rs.Index = GetIndex1Field(eIdx).AsFld();
        rs.Seek("=",value);
        if (rs.NoMatch)
            return false;
        cPlace = GetData(rs);
        return true;
    }

    public string FullName(IPlaceData? cPlace, bool xNoPraepos=true, bool xAdditional=false)
    {
        if (cPlace == null)
            return string.Empty;
        string result;
        string Place_sPolName = cPlace.sPolName;
        string Place_sZusatz = xNoPraepos ? cPlace.sZusatz : "";
        string Place_sKreis = "";
        string Place_sLand = "";
        string Place_sStaat = "";

        if (Place_sPolName.AsInt() > 0)
        {
            Place_sPolName = DataModul.TextLese1(Place_sPolName.AsInt()).FrameIfNEoW(" (", ")");
        }
        else
        {
            Place_sPolName = Place_sPolName.FrameIfNEoW(" (", ")");
        }

        if (xNoPraepos && "" == Place_sZusatz)
        {
            Place_sZusatz = "in";
        }
        if (!xAdditional )
        {
            Place_sKreis = cPlace.sKreis;
            Place_sLand = cPlace.sLand;
            Place_sStaat = cPlace.sStaat;
        }
        result = (Place_sPolName + " " + cPlace.sOrt.TrimEnd() + cPlace.sOrtsteil.TrimEnd().FrameIfNEoW("-", "") + Place_sPolName + " " + Place_sKreis.TrimEnd() + " " + Place_sLand.TrimEnd() + " " + Place_sStaat.TrimEnd()).Trim();
        return result;
    }

}
