﻿//using DAO;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using BaseLib.Helper;
using GenFree.Interfaces.Data;
using GenFree.Models;

namespace GenFree.Data;
#nullable enable


public class CEvent : CUsesIndexedRSet<(EEventArt eArt, int iLink, short iLfNr), EventIndex, EventFields, IEventData>, IEvent
{
    private Func<IRecordset> _DB_EventTable;

    protected override IRecordset _db_Table => _DB_EventTable();

    protected override EventIndex _keyIndex => EventIndex.ArtNr;

    public CEvent(Func<IRecordset> dB_EventTable)
    {
        _DB_EventTable = dB_EventTable;
    }
    private void ForeachEvent(int iFamPers, EEventArt eArtStart, EEventArt eArtEnd, Action<IEventData, IRecordset> action)
    {
        var dB_EventTable = _db_Table;
        dB_EventTable.Index = nameof(EventIndex.BeSu);
        for (EEventArt i = eArtStart; i <= eArtEnd; i++)
        {
            dB_EventTable.Seek("=", i, iFamPers);
            while (!dB_EventTable.NoMatch
                 && !dB_EventTable.EOF
                 && !(dB_EventTable.Fields[EventFields.PerFamNr].AsInt() != iFamPers)
                 && !(dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != i))
            {
                try { action(GetData(dB_EventTable), dB_EventTable); } catch { }
                dB_EventTable.MoveNext();
            }
        }
    }

    public DateTime[] GetPersonDates(int persInArb, out bool xBC1, Action<EEventArt, int, string>? onPlace = null)
    {
        DateTime[] array = new DateTime[5];
        array.Initialize();
        var xBC = false;
        ForeachEvent(persInArb, EEventArt.eA_Birth, EEventArt.eA_Burial, (ed, rs) =>
        {
            xBC |= ed.iOrt != 0;
            if (ed.iLfNr == 0 || array[(int)ed.eArt - 100] == default)
                array[(int)ed.eArt - 100] = ed.dDatumV;
            onPlace?.Invoke(ed.eArt, ed.iOrt, ed.sOrt_S);
        });
        xBC1 = xBC;
        return array;
    }

    public DateTime[] ReadFamDates(int famInArb)
    {
        DateTime[] array = new DateTime[7];
        array.Initialize();
        ForeachEvent(famInArb, EEventArt.eA_Marriage, EEventArt.eA_507, (ed, rs) =>
        {
            if (ed.iLfNr == 0 || array[(int)ed.eArt - 500] == default)
                array[(int)ed.eArt - 500] = ed.dDatumV;
        });
        return array;
    }

    public DateTime GetPersonBirthOrBapt(int persInArb, bool xPrefBap = false)
    {
        var dB_EventTable = Seek((xPrefBap ? EEventArt.eA_Birth : EEventArt.eA_Baptism, persInArb, 0));
        if ((dB_EventTable?.Fields[EventFields.DatumV]).AsDate() == default)
            dB_EventTable = Seek((xPrefBap ? EEventArt.eA_Baptism : EEventArt.eA_Birth, persInArb, 0));

        return (dB_EventTable?.Fields[EventFields.DatumV]).AsDate();
    }

    public DateTime GetDate(EEventArt eArt, int iFamPers)
        => GetDate(eArt, iFamPers, out _);
    public DateTime GetDate(EEventArt eArt, int iFamPers, out string sDateV_S)
    {
        var dB_EventTable = Seek((eArt, iFamPers, 0), out var xBreak);
        sDateV_S = "";
        if (!xBreak)
        {
            sDateV_S = dB_EventTable!.Fields[EventFields.DatumV_S].AsString();
            return dB_EventTable.Fields[EventFields.DatumV].AsDate();
        }
        else
            return default;

    }

    public DateTime GetDateB(EEventArt eArt, int iFamPers)
        => GetDateB(eArt, iFamPers, out _);
    public DateTime GetDateB(EEventArt eArt, int iFamPers, out string sDateB_S)
    {
        var dB_EventTable = Seek((eArt, iFamPers, 0), out var xBreak);
        sDateB_S = "";
        if (!xBreak)
        {
            sDateB_S = dB_EventTable!.Fields[EventFields.DatumB_S].AsString();
            return dB_EventTable.Fields[EventFields.DatumB].AsDate();
        }
        else
            return default;

    }

    public T GetValue<T>((EEventArt eArt, int iLink, short iLfNR) key, EventFields eDataField, T dDef)
    {
        return (Seek(key) is IRecordset dB_EventTable
            && !dB_EventTable.NoMatch
            && dB_EventTable.Fields[eDataField] is T data) ? data : dDef;
    }

    public void PersonDat(int iPersNr, out DateTime down1, out DateTime up1)
    {
        DateTime down = default;
        DateTime up = default;
        ForeachEvent(iPersNr, EEventArt.eA_Birth, EEventArt.eA_Burial, (ed, rs) =>
        {
            if (ed.eArt is EEventArt.eA_Birth or EEventArt.eA_Baptism && (ed.iLfNr == 0 || down == default))
                down = ed.dDatumV;
            if (ed.eArt is EEventArt.eA_Death or EEventArt.eA_Burial && (ed.iLfNr == 0 || up == default))
                up = ed.dDatumV;
        });
        (down1, up1) = (down, up);
    }

    public IEventData? ReadDataPl(EEventArt eEventArt, int persInArb, out bool xBreak, short iLfNr = 0)
    {
        xBreak = !ReadData(eEventArt, persInArb, out var cEvn, iLfNr) || cEvn!.iOrt == 0;
        return cEvn;
    }

    public bool ReadData(EEventArt eEventArt, int persInArb, out IEventData? cEvt, short iLfNr = 0) =>
        ReadData((eEventArt, persInArb, iLfNr), out cEvt);

    public bool Exists(EEventArt eArt, int iLink, int iLfNR = 0) => Exists((eArt, iLink, (short)iLfNR));

    public bool ExistsBeSu(EEventArt eArt, int iLink)
    {
        _ = SeekBeSu(eArt, iLink, out var xBreak);
        return !xBreak;
    }

    public bool DeleteBeSu(EEventArt eArt, int iPerFamNr)
    {
        SeekBeSu(eArt, iPerFamNr, out var xB)?.Delete();
        return !xB;
    }

    public void DeleteAll(EEventArt eArt, int iPerFamNr)
    {
        var db_Table = SeekBeSu(eArt, iPerFamNr, out var xBreak);
        while (!xBreak
           && !db_Table!.EOF
           && !(db_Table.Fields[EventFields.PerFamNr].AsInt() != iPerFamNr)
           && !(db_Table.Fields[EventFields.Art].AsEnum<EEventArt>() != eArt))
        {
            if (!db_Table.NoMatch)
            {
                db_Table.Delete();
            }
            db_Table.MoveNext();
        }
    }

    public void ChgEvent(EEventArt eArt, int iPerFamNr, EEventArt eArt2, int iFam2 = 0)
    {
        var dB_EventTable = SeekBeSu(eArt, iPerFamNr, out var xBreak);
        if (!xBreak && (eArt != eArt2 || (iFam2 != 0 && iFam2 != iPerFamNr)))
        {
            dB_EventTable!.Edit();
            dB_EventTable.Fields[EventFields.Art].Value = eArt2;
            if (iFam2 != 0 && iFam2 != iPerFamNr)
                dB_EventTable.Fields[EventFields.PerFamNr].Value = iFam2;
            dB_EventTable.Update();
        }
    }

    public IRecordset? SeekBeSu(EEventArt eArt, int iPerFamnr, out bool xBreak)
    {
        _db_Table.Index = nameof(EventIndex.BeSu);
        _db_Table.Seek("=", (int)eArt, iPerFamnr);
        xBreak = _db_Table.NoMatch;
        return xBreak ? null : _db_Table;
    }

    public IEnumerable<IEventData> ReadEventsBeSu(int iFamPers, EEventArt iArt)
        => ReadAllDataDB(Idx: EventIndex.BeSu, SeekAct: (rs) => rs.Seek("=", (int)iArt, iFamPers), StopPred: (ed) => ed.eArt != iArt || ed.iPerFamNr != iFamPers);

    public IEnumerable<IEventData> ReadAllPlaces(int iPlace)
        => ReadAllDataDB(EventIndex.EOrt, (rs) => rs.Seek("=", iPlace), (e) => e.iOrt != iPlace);

    public bool DeleteEmptyFam(int ifamInArb, EEventArt eArt)
    {
        var xInfoFound = false;
        if (ReadData(eArt, ifamInArb, out var cEvt1)
           && cEvt1!.sVChr == "0")
        {
            xInfoFound = cEvt1.dDatumV != default
                || cEvt1.dDatumB != default
                || cEvt1.iOrt != 0
                || cEvt1.iPlatz != 0
                || cEvt1.iLfNr != 0
                || cEvt1.iKBem != 0
                || cEvt1.sDatumV_S.Trim() != ""
                || cEvt1.sDatumB_S.Trim() != ""
                || cEvt1.sOrt_S.Trim() != ""
                || cEvt1.sReg.Trim() != ""
                || cEvt1.sBem[1].Trim() != ""
                || cEvt1.sBem[2].Trim() != ""
                || cEvt1.sBem[3].Trim() != "";
            if (!xInfoFound)
            {
                cEvt1.Delete();
            }
            return !xInfoFound;
        }
        return !xInfoFound;
    }

    public void PersLebDatles(int PersInArb, IPersonData person)
    {
        for (var _Iter = EEventArt.eA_Burial; _Iter >= EEventArt.eA_Birth; _Iter--)
        {
            if (ReadData(_Iter, PersInArb, out var cEvt, 0))
            {
                person.SetData(cEvt!);
            }
        }
    }

    public override IRecordset? Seek((EEventArt eArt, int iLink, short iLfNr) key, out bool xBreak)
    {
        var dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{_keyIndex}";
        dB_EventTable.Seek("=", (int)key.eArt, key.iLink, key.iLfNr);
        xBreak = dB_EventTable.NoMatch;
        return xBreak ? null : dB_EventTable;
    }

    public bool ReadBeSu(EEventArt eArt, int iLink, out IEventData? cEv)
    {
        var _r = SeekBeSu(eArt, iLink, out var xB);
        cEv = _r is null ? null : GetData(_r);
        return !xB;
    }

    public void SetValues((EEventArt eArt, int iLink, short iLfNr) key, (EventFields, object)[] values)
    {
        var dB_EventTable = Seek(key);
        if (dB_EventTable?.NoMatch != false)
        {
            dB_EventTable = AppendRaw(key);
        }
        else
            dB_EventTable.Edit();
        foreach (var (field, value) in values)
        {
            dB_EventTable.Fields[field].Value = value; // Todo: Error-Handling
        }
        dB_EventTable.Update();
    }

    public IRecordset AppendRaw((EEventArt eArt, int iLink, short iLfNr) key)
    {
        var recordset = _db_Table;
        recordset.AddNew();
        recordset.Fields[EventFields.Art].Value = key.eArt;
        recordset.Fields[EventFields.PerFamNr].Value = key.iLink;
        recordset.Fields[EventFields.DatumV].Value = 0;
        recordset.Fields[EventFields.DatumV_S].Value = " ";
        recordset.Fields[EventFields.DatumB].Value = 0;
        recordset.Fields[EventFields.DatumB_S].Value = " ";
        recordset.Fields[EventFields.DatumText].Value = "0";
        recordset.Fields[EventFields.Ort].Value = 0;
        recordset.Fields[EventFields.Ort_S].Value = " ";
        recordset.Fields[EventFields.KBem].Value = 0;
        recordset.Fields[EventFields.Reg].Value = " ";
        recordset.Fields[EventFields.Bem1].Value = " ";
        recordset.Fields[EventFields.Bem2].Value = " ";
        recordset.Fields[EventFields.Platz].Value = 0;
        recordset.Fields[EventFields.LfNr].Value = key.iLfNr;
        recordset.Fields[EventFields.VChr].Value = "0";
        recordset.Fields[EventFields.Zusatz].Value = "";
        recordset.Fields[EventFields.GrabNr].Value = 0;
        recordset.Fields[EventFields.tot].Value = " ";
        recordset.Update();
        return recordset;
    }

    protected override (EEventArt eArt, int iLink, short iLfNr) GetID(IRecordset recordset)
    {
        return (recordset.Fields[EventFields.Art].AsEnum<EEventArt>(),
            recordset.Fields[EventFields.PerFamNr].AsInt(),
            (short)recordset.Fields[EventFields.LfNr].AsInt());
    }

    public override EventFields GetIndex1Field(EventIndex eIndex)
   => eIndex switch
   {
       EventIndex.NText => EventFields.ArtText,
       EventIndex.PText => EventFields.Platz,
       EventIndex.CText => EventFields.Causal,
       EventIndex.KText => EventFields.KBem,
       EventIndex.EOrt => EventFields.Ort,
       EventIndex.HaNu => EventFields.Hausnr,
       EventIndex.Reg => EventFields.Reg,
       EventIndex.JaTa => EventFields.Art,
       _ => throw new NotImplementedException(),
   };

    protected override IEventData GetData(IRecordset rs, bool xNoInit = false) => new CEventData(rs, xNoInit);

    public void UpdateClearPred(EventIndex eIndex, EventFields eIndexField, int iIndexVal, Predicate<IEventData> predicate)
    {
        var dB_EventTable = Seek(eIndex, iIndexVal);
        if (dB_EventTable?.EOF == false
            && dB_EventTable.Fields[eIndexField].AsInt() == iIndexVal)
        {
            IEventData cEv = GetData(dB_EventTable);
            if (predicate(cEv))
            {
                dB_EventTable.Edit();
                dB_EventTable.Fields[EventFields.ArtText].Value = 0;
                dB_EventTable.Update();
            }
        }

    }
    public void UpdateAllSetVal(EventIndex eIndex, EventFields eIndexField, int iIndexVal, int iNewVal)
    {
        var dB_EventTable = Seek(eIndex, iIndexVal);
        while (dB_EventTable?.EOF == false
            && !dB_EventTable.NoMatch
            && dB_EventTable.Fields[eIndexField].AsInt() == iIndexVal)
        {
            dB_EventTable.Edit();
            dB_EventTable.Fields[eIndexField].Value = iNewVal;
            dB_EventTable.Update();
            dB_EventTable.MoveNext();
        }
    }

    public bool UpdateValues((EEventArt eArt, int iLink, short iLfNr) key, (EventFields, object)[] values)
    {
        if (Seek(key) is IRecordset dB_EventTable
            && !dB_EventTable.NoMatch)
        {
            dB_EventTable.Edit();
            foreach (var (field, value) in values)
            {
                dB_EventTable.Fields[field].Value = value; // Todo: Error-Handling
            }
            dB_EventTable.Update();
            return true;
        }
        return false;
    }

    public void UpdateAllMvAppend(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, string sNewText)
    {
        IRecordset dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{eIndex}";
        dB_EventTable.Seek("=", iIndexVal);
        while (!dB_EventTable.NoMatch
            && !dB_EventTable.EOF
            && dB_EventTable.Fields[eIndexField].AsInt() == iIndexVal)
        {
            dB_EventTable.Edit();

            IField field = dB_EventTable.Fields[eModField];

            field.Value = field.AsString().Trim() == ""
                ? sNewText
                : field.AsString() + " " + sNewText;

            dB_EventTable.Fields[eIndexField].Value = 0;

            dB_EventTable.Update();

            dB_EventTable.MoveNext();
        }
    }

    public void UpdateAllMvVal(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, int iClearVal = 0)
    {
        IRecordset dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{eIndex}";
        dB_EventTable.Seek("=", iIndexVal);
        while (!dB_EventTable.NoMatch
            && !dB_EventTable.EOF
            && dB_EventTable.Fields[eIndexField].AsInt() == iIndexVal)
        {
            dB_EventTable.Edit();
            dB_EventTable.Fields[eModField].Value = iIndexVal;
            dB_EventTable.Fields[eIndexField].Value = iClearVal;
            dB_EventTable.Update();
            dB_EventTable.MoveNext();
        }
    }
    public void UpdateAllSetValPred(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, int iNewVal, Predicate<IEventData> predicate, int iClearVal = 0)
    {
        IRecordset dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{eIndex}";
        dB_EventTable.Seek("=", iIndexVal);
        while (!dB_EventTable.NoMatch
            && !dB_EventTable.EOF
            && dB_EventTable.Fields[eIndexField].AsInt() == iIndexVal)
        {
            if (predicate(GetData(dB_EventTable)))
            {
                dB_EventTable.Edit();
                dB_EventTable.Fields[eModField].Value = iNewVal;
                if (eModField != eIndexField)
                    dB_EventTable.Fields[eIndexField].Value = iClearVal;
                dB_EventTable.Update();
            }
            dB_EventTable.MoveNext();
        }
    }

    public bool ExistsPred(EventIndex eIndex, EventFields eIndexField, int iTndexVal, Predicate<IEventData> predicate)
    {
        IRecordset dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{eIndex}";
        dB_EventTable.Seek("=", iTndexVal);
        while (!dB_EventTable.NoMatch
            && !dB_EventTable.EOF
            && dB_EventTable.Fields[eIndexField].AsInt() == iTndexVal)
        {
            if (predicate(GetData(dB_EventTable)))
            {
                return true;
            }
            dB_EventTable.MoveNext();
        }
        return false;
    }

    public void ClearAllRemText(EventIndex eIndex, EventFields eIdxField, int iIdxVal)
    {
        IRecordset dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{eIndex}";
        dB_EventTable.Seek("=", iIdxVal);
        while (!dB_EventTable.EOF
            && !dB_EventTable.NoMatch
            && dB_EventTable.Fields[eIdxField].AsInt() == iIdxVal)
        {
            dB_EventTable.Edit();
            dB_EventTable.Fields[eIdxField].Value = 0;
            dB_EventTable.Update();

            dB_EventTable.MoveNext();
        }
    }

    public void SetValAppend((EEventArt eArt, int iLink, short iLfNr) key, EventFields eSetField, string sNewVal)
    {
        var dB_EventTable = Seek(key);
        if (dB_EventTable?.NoMatch != false)
        {
            dB_EventTable = AppendRaw(key);
        }
        dB_EventTable.Edit();
        var field = dB_EventTable.Fields[eSetField];
        field.Value = field.AsString().Trim() == ""
            ? sNewVal + " "
            : field.AsString() + " " + sNewVal;
        dB_EventTable.Update();
    }

    public T GetValue<T>(int persInArb, EEventArt iEventType, EventFields eGetField, Func<IField, T> conv)
    {
        var dB_EventTable = _db_Table;
        dB_EventTable.Index = nameof(EventIndex.ArtNr);
        dB_EventTable.Seek("=", (int)iEventType, persInArb, 0);
        T sEvtBem4 = conv(dB_EventTable.Fields[eGetField]);
        return sEvtBem4;
    }

    public void DeleteAllNonVitalE(int num18)
    {
        int num19 = 300;
        while (num19 <= 302)
        {
            while (DeleteBeSu((EEventArt)num19, num18)) ;
            num19++;
        }
    }

    public void DeleteAllVitalE(int num18)
    {
        int num19 = 101;
        while (num19 <= 120)
        {
            while (DeleteBeSu((EEventArt)num19, num18)) ;
            num19++;
        }
    }

    public void UpdateReplFams(int Fam1, int Fam2, EEventArt eArt)
    {
        IRecordset? dB_EventTable;
        if ((dB_EventTable = SeekBeSu(eArt, Fam1, out _)) != null)
        {
            dB_EventTable.Edit();
            dB_EventTable.Fields[EventFields.PerFamNr].Value = Fam2;
            dB_EventTable.Update();
        }
    }
    public IEnumerable<IEventData> ReadAllGt(EventIndex eIndex, int iIndexVal)
    {
        var dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{eIndex}";
        dB_EventTable.Seek(">=", iIndexVal);

        while (!dB_EventTable.EOF
            && !dB_EventTable.NoMatch)
        {
            yield return GetData(dB_EventTable);
            dB_EventTable.MoveNext();
        }
    }

    public IEnumerable<IEventData> ReadAllBeSu(EEventArt eEventArt, int persInArb)
    {
        var dB_EventTable = SeekBeSu(eEventArt, persInArb, out var xBreak);
        if (!xBreak)
        {
            while (!dB_EventTable!.EOF
                && !dB_EventTable.NoMatch
                && dB_EventTable.Fields[EventFields.PerFamNr].AsInt() == persInArb
                && dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() == eEventArt)
            {
                yield return GetData(dB_EventTable);
                dB_EventTable.MoveNext();
            }
        }
    }

    public IEnumerable<IEventData> ReadEntityEvents(int iD, bool xFamily)
    {
        var dB_EventTable = _db_Table;

        dB_EventTable.Index = nameof(EventIndex.BeSu2);
        dB_EventTable.Seek(">=", iD);
        while (!dB_EventTable.NoMatch
                    && !dB_EventTable.EOF
                    && dB_EventTable.Fields[EventFields.PerFamNr].AsInt() == iD)
        {
            if (xFamily == (dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() >= EEventArt.eA_500))
                yield return GetData(dB_EventTable);
            dB_EventTable.MoveNext();
        }

    }
}


