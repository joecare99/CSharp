﻿using System;
using System.Linq;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces;
using GenFree.Model;
using GenFree.Helper;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace GenFree.Data;


public class CPlace : CUsesRecordSet<int>, IPlace
{
    private Func<IRecordset> _value;

    public CPlace(Func<IRecordset> value)
    {
        _value = value;
    }

    protected override IRecordset _db_Table => _value();

    protected override string _keyIndex => nameof(PlaceIndex.OrtNr);

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


    public override IRecordset? Seek(int key, out bool xBreak)
    {
        IRecordset dB_PlaceTable = _db_Table;
        dB_PlaceTable.Index = _keyIndex;
        dB_PlaceTable.Seek("=", key);
        xBreak = dB_PlaceTable.NoMatch;
        return xBreak ? null : dB_PlaceTable;
    }

    public bool ReadData(int key, out IPlaceData? data)
    {
        var dB_PlaceTable = Seek(key, out bool xBreak);
        data = xBreak ? null : new CPlaceData(dB_PlaceTable);
        return !xBreak;

    }

    public IEnumerable<IPlaceData> ReadAll()
    {
        IRecordset dB_PlaceTable = _db_Table;
        dB_PlaceTable.Index = _keyIndex;
        dB_PlaceTable.MoveFirst();
        while (!dB_PlaceTable.EOF)
        {
            yield return new CPlaceData(dB_PlaceTable);
            dB_PlaceTable.MoveNext();
        }
    }

    public void SetData(int key, IPlaceData data, string[]? asProps = null)
    {
        var dB_FamilyTable = Seek(key);
        if (dB_FamilyTable != null)
        {
            dB_FamilyTable.Edit();
            data.SetDBValue(dB_FamilyTable, asProps);
            dB_FamilyTable.Update();
        }
    }

    protected override int GetID(IRecordset recordset)
    {
        return _db_Table.Fields[nameof(PlaceFields.OrtNr)].AsInt();
    }
}
