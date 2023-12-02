//using DAO;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Helper;
using System;
using System.Collections.Generic;

namespace GenFree.Data;
#nullable enable


public class CEvent : CUsesRecordSet<(EEventArt eArt, int iLink, short iLfNr)>, IEvent
{
    private Func<IRecordset> _DB_EventTable;

    protected override IRecordset _db_Table => _DB_EventTable();

    protected override string _keyIndex => nameof(EventIndex.ArtNr);

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
                 && !(dB_EventTable.Fields[nameof(EventFields.PerFamNr)].AsInt() != iFamPers)
                 && !(dB_EventTable.Fields[nameof(EventFields.Art)].AsEnum<EEventArt>() != i))
            {
                try { action(new CEventData(dB_EventTable), dB_EventTable); } catch { }
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

    public DateTime GetPersonBirthOrBapt(int persInArb)
    {
        var dB_EventTable = Seek((EEventArt.eA_Birth, persInArb, 0));
        if ((dB_EventTable?.Fields[nameof(EventFields.DatumV)]).AsDate() == default)
            dB_EventTable = Seek((EEventArt.eA_Baptism, persInArb, 0));

        return (dB_EventTable?.Fields[nameof(EventFields.DatumV)]).AsDate();
    }

    public DateTime GetDate(int iFamPers, EEventArt eArt)
        => GetDate(iFamPers, eArt, out _);
    public DateTime GetDate(int iFamPers, EEventArt eArt, out string sDateV_S)
    {
        var dB_EventTable = Seek((eArt, iFamPers, 0), out var xBreak);
        sDateV_S = "";
        if (!xBreak)
        {
            sDateV_S = dB_EventTable.Fields[nameof(EventFields.DatumV_S)].AsString();
            return dB_EventTable.Fields[nameof(EventFields.DatumV)].AsDate();
        }
        else
            return default;

    }

    public DateTime GetDateB(int iFamPers, EEventArt eArt)
        => GetDateB(iFamPers, eArt, out _);
    public DateTime GetDateB(int iFamPers, EEventArt eArt, out string sDateB_S)
    {
        var dB_EventTable = Seek((eArt, iFamPers, 0), out var xBreak);
        sDateB_S = "";
        if (!xBreak)
        {
            sDateB_S = dB_EventTable.Fields[nameof(EventFields.DatumB_S)].AsString();
            return dB_EventTable.Fields[nameof(EventFields.DatumB)].AsDate();
        }
        else
            return default;

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

    public IEventData? ReadDataPl(int persInArb, EEventArt eEventArt, out bool xBreak, short iLfNr = 0)
    {
        xBreak = ReadData(persInArb, eEventArt, out var cEvn, iLfNr) && cEvn?.iOrt != 0;
        return cEvn;
    }

    public bool ReadData(int persInArb, EEventArt eEventArt, out IEventData? cEvt, short iLfNr = 0) =>
        ReadData((eEventArt, persInArb, iLfNr), out cEvt);

    public bool Exists(EEventArt eArt, int iLink, int iLfNR = 0) => Exists((eArt, iLink, (short)iLfNR));

    public void DeleteBeSu(int iPerFamNr, EEventArt eArt)
    {
        SeekBeSu(iPerFamNr, eArt, out _)?.Delete();        
    }

    public void DeleteAll(int iPerFamNr, EEventArt eArt)
    {
        var db_Table = SeekBeSu(iPerFamNr, eArt, out var xBreak);
        while (!xBreak
           && !db_Table.EOF
           && !(db_Table.Fields[nameof(EventFields.PerFamNr)].AsInt() != iPerFamNr)
           && !(db_Table.Fields[nameof(EventFields.Art)].AsEnum<EEventArt>() != eArt))
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
        var dB_EventTable = SeekBeSu(iPerFamNr, eArt, out var xBreak);
        if (!xBreak && (eArt != eArt2 || (iFam2 != 0 && iFam2 != iPerFamNr)))
        {
            dB_EventTable.Edit();
            dB_EventTable.Fields[nameof(EventFields.Art)].Value = eArt2;
            if (iFam2 != 0)
                dB_EventTable.Fields[nameof(EventFields.PerFamNr)].Value = iFam2;
            dB_EventTable.Update();
        }

    }

    public IRecordset? SeekBeSu(int iPerFamnr, EEventArt eArt, out bool xBreak)
    {
        _db_Table.Index = nameof(EventIndex.BeSu);
        _db_Table.Seek("=", eArt, iPerFamnr);
        xBreak = _db_Table.NoMatch;
        return xBreak ? null : _db_Table;
    }

    public IEnumerable<IEventData> ReadEventsBeSu(int iFamPers, EEventArt iArt)
    => ReadEventDataDB(Idx: EventIndex.BeSu, SeekAct: (rs) => rs.Seek("=", iArt, iFamPers), StopPred: (ed) => ed.eArt != iArt || ed.iPerFamNr != iFamPers);

    private IEnumerable<IEventData> ReadEventDataDB(EventIndex Idx, Action<IRecordset> SeekAct, Predicate<CEventData> StopPred)
    {
        var dB_EventTable = _db_Table;
        dB_EventTable.Index = $"{Idx}";
        SeekAct(dB_EventTable);
        while (!dB_EventTable.NoMatch
            && !dB_EventTable.EOF)
        {
            var _event = new CEventData(dB_EventTable);
            if (StopPred(_event))
                break;
            yield return _event;
            dB_EventTable.MoveNext();
        }
    }

    public IEnumerable<IEventData> ReadAll()
        => ReadEventDataDB(EventIndex.ArtNr, (rs) => rs.Seek(">=", 0), (e) => false);

    public bool DeleteEmptyFam(int ifamInArb, EEventArt eArt)
    {
        var xInfoFound = false;
        if (ReadData(ifamInArb, eArt, out var cEvt1)
                                                && !(cEvt1!.sVChr != "0"))
        {
            if (cEvt1.dDatumV != default
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
                || cEvt1.sBem[3].Trim() != "")
            {
                xInfoFound = true;
            }
            if (!xInfoFound)
            {
                cEvt1.Delete();
            }
        }
        return !xInfoFound;
    }

    public void PersLebDatles(int PersInArb, IPersonData person)
    {
        for (var _Iter = EEventArt.eA_Burial; _Iter >= EEventArt.eA_Birth; _Iter--)
        {
            if (ReadData(PersInArb, _Iter, out var cEvt, 0))
            {
                person.SetData(cEvt!);
            }
        }
    }

    public override IRecordset? Seek((EEventArt eArt, int iLink, short iLfNr) key, out bool xBreak)
    {
        var dB_EventTable = _db_Table;
        dB_EventTable.Index = _keyIndex;
        dB_EventTable.Seek("=", key.eArt, key.iLink, key.iLfNr);
        xBreak = dB_EventTable.NoMatch;
        return xBreak ? null : dB_EventTable;
    }

    public bool ReadData((EEventArt eArt, int iLink, short iLfNr) key, out IEventData? cEvt)
    {
        var dB_EventTable = Seek(key, out bool xBreak);
        cEvt = xBreak ? null : (IEventData)new CEventData(dB_EventTable);
        return !xBreak;
    }

    public void SetData((EEventArt eArt, int iLink, short iLfNr) key, IEventData data, string[]? asProps = null)
    {
        throw new NotImplementedException();
    }

    protected override (EEventArt eArt, int iLink, short iLfNr) GetID(IRecordset recordset)
    {
        return (recordset.Fields[nameof(EventFields.Art)].AsEnum<EEventArt>(),
            recordset.Fields[nameof(EventFields.PerFamNr)].AsInt(),
            (short)recordset.Fields[nameof(EventFields.LfNr)].AsInt());
    }
}


