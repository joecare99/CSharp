using GenFree.Interfaces;
using GenFree.Data;
using GenFree.Helper;
using System;
using GenFree.Interfaces.DB;
using System.Collections.Generic;
using System.Linq;

namespace GenFree.Data;

public class CEventData : IEventData
{
    private string? _sArtText;
    private string? _sHausNr;
    private string? _sKBem;
    private string? _sDatumText;
    private string? _sPlatz;
    private string? _sCausal;
    private string? _sGrabNr;
    private string? _sAn;
    private List<EEventProp> _changedPropList = new();

    private static Func<IRecordset> __dB_EventTable = () => DataModul.DB_EventTable;
    private IRecordset _dB_EventTable;
    private static Func<int, string> _GetText = DataModul.TextLese1;

    public static void SetTable(Func<IRecordset> dB_EventTable)
    {
        __dB_EventTable = dB_EventTable;
    }

    public static void SetGetText(Func<int, string> getText)
    {
        _GetText = getText;
    }
    public CEventData()
    {
        _dB_EventTable = __dB_EventTable();
    }

    public CEventData(IRecordset dB_EventTable)
    {
        FillDataFields(dB_EventTable);
        _dB_EventTable = dB_EventTable;
    }

    public CEventData(EventIndex index, int[] ints, out bool xBreak)
    {
        var dB_EventTable = _dB_EventTable = __dB_EventTable();
        dB_EventTable.Index = $"{index}";
        dB_EventTable.Seek("=", ints);
        if (!(xBreak = dB_EventTable.NoMatch))
        {
            FillDataFields(dB_EventTable);
        }
    }

    public void FillDataFields(IRecordset dB_EventTable)
    {
        FillIntFields(dB_EventTable);

        eArt = dB_EventTable.Fields[nameof(EventFields.Art)].AsEnum<EEventArt>();

        FillStringFields(dB_EventTable);

        FillDateFields(dB_EventTable);

        _sArtText = null;
        _sHausNr = null;
        _sKBem = null;
        _sDatumText = null;
        _sPlatz = null;
        _sCausal = null;
        _sGrabNr = null;
        _sAn = null;
    }

    private void FillDateFields(IRecordset dB_EventTable)
    {
        dDatumV = dB_EventTable.Fields[nameof(EventFields.DatumV)].AsDate();
        dDatumB = dB_EventTable.Fields[nameof(EventFields.DatumB)].AsDate();
    }

    private void FillStringFields(IRecordset dB_EventTable)
    {
        sDeath = dB_EventTable.Fields[nameof(EventFields.tot)].AsString();
        sBem[1] = dB_EventTable.Fields[nameof(EventFields.Bem1)].AsString();
        sBem[2] = dB_EventTable.Fields[nameof(EventFields.Bem2)].AsString();
        sBem[3] = dB_EventTable.Fields[nameof(EventFields.Bem3)].AsString();
        sBem[4] = dB_EventTable.Fields[nameof(EventFields.Bem4)].AsString();
        sZusatz = dB_EventTable.Fields[nameof(EventFields.Zusatz)].AsString();
        sReg = dB_EventTable.Fields[nameof(EventFields.Reg)].AsString();
        sVChr = dB_EventTable.Fields[nameof(EventFields.VChr)].AsString();
        sDatumV_S = dB_EventTable.Fields[nameof(EventFields.DatumV_S)].AsString();
        sDatumB_S = dB_EventTable.Fields[nameof(EventFields.DatumB_S)].AsString();
        sOrt_S = dB_EventTable.Fields[nameof(EventFields.Ort_S)].AsString();
    }

    private void FillIntFields(IRecordset dB_EventTable)
    {
        iPrivacy = dB_EventTable.Fields[nameof(EventFields.priv)].AsInt();
        iArtText = dB_EventTable.Fields[nameof(EventFields.ArtText)].AsInt();
        iLfNr = dB_EventTable.Fields[nameof(EventFields.LfNr)].AsInt();
        iPerFamNr = dB_EventTable.Fields[nameof(EventFields.PerFamNr)].AsInt();
        iAn = dB_EventTable.Fields[nameof(EventFields.an)].AsInt();
        iHausNr = dB_EventTable.Fields[nameof(EventFields.Hausnr)].AsInt();
        iCausal = dB_EventTable.Fields[nameof(EventFields.Causal)].AsInt();
        iDatumText = dB_EventTable.Fields[nameof(EventFields.DatumText)].AsInt();
        iOrt = dB_EventTable.Fields[nameof(EventFields.Ort)].AsInt();
        iKBem = dB_EventTable.Fields[nameof(EventFields.KBem)].AsInt();
        iPlatz = dB_EventTable.Fields[nameof(EventFields.Platz)].AsInt();
        iGrabNr = dB_EventTable.Fields[nameof(EventFields.GrabNr)].AsInt();
    }

    public void Delete()
    {
        CheckOrSetRecord(eArt, iPerFamNr, iLfNr)?.Delete();
    }

    private IRecordset? CheckOrSetRecord(EEventArt eArt, int iPerFamNr, int iLfNr)
    {
        IRecordset rs = _dB_EventTable;
        if (eArt == rs.Fields[nameof(EventFields.Art)].AsEnum<EEventArt>()
            && iPerFamNr == rs.Fields[nameof(EventFields.PerFamNr)].AsInt()
            && iLfNr == rs.Fields[nameof(EventFields.LfNr)].AsInt())
        {
            return rs;
        }
        else
        {
            rs.Index = nameof(EventIndex.ArtNr);
            rs.Seek("=", eArt, iPerFamNr, iLfNr);
            return !rs.NoMatch ? rs : null;
        }
    }

    public void Update(string[]? strings = null)
    {
        IRecordset? rs = CheckOrSetRecord(eArt, iPerFamNr, iLfNr);
        if (rs == null) return;
        IField f;
        string d;
        if (strings != null)
            foreach (var s in strings)
                switch (s)
                {
                    case nameof(IEventData.sBem):
                        if ((f = rs.Fields[nameof(EventFields.Bem1)]).AsString() == (d = sBem[1]))
                            SetData(rs, f, d);
                        if ((f = rs.Fields[nameof(EventFields.Bem2)]).AsString() == (d = sBem[2]))
                            SetData(rs, f, d);
                        if ((f = rs.Fields[nameof(EventFields.Bem3)]).AsString() == (d = sBem[3]))
                            SetData(rs, f, d);
                        if ((f = rs.Fields[nameof(EventFields.Bem4)]).AsString() == (d = sBem[4]))
                            SetData(rs, f, d);
                        break;
                }
        else
        {
            rs.Edit();
            SetDBData(rs);
        }
        // TODO: Update Record
        if (rs!.EditMode != 0)
            rs.Update();

        static void SetData(IRecordset? rs, IField f, string d)
        {
            if (rs.EditMode == 0)
                rs.Edit();
            f.Value = d;
        }
    }

    public Type GetPropType(EEventProp prop)
    {
        return prop switch
        {
            EEventProp.eArt => typeof(EEventArt),
            EEventProp.iPerFamNr => typeof(int),
            EEventProp.iLfNr => typeof(int),
            EEventProp.iArtText => typeof(int),
            EEventProp.iPrivacy => typeof(int),
            EEventProp.xIsDead => typeof(bool),
            EEventProp.dDatumV => typeof(DateTime),
            EEventProp.sReg => typeof(string),
            EEventProp.sDatumV_S => typeof(string),
            EEventProp.sVChr => typeof(string),
            EEventProp.dDatumB => typeof(DateTime),
            EEventProp.sDatumB_S => typeof(string),
            EEventProp.sZusatz => typeof(string),
            EEventProp.sOrt_S => typeof(string),
            EEventProp.iOrt => typeof(int),
            EEventProp.iDatumText => typeof(int),
            EEventProp.iKBem => typeof(int),
            EEventProp.iPlatz => typeof(int),
            EEventProp.iCausal => typeof(int),
            EEventProp.iGrabNr => typeof(int),
            EEventProp.iAn => typeof(int),
            EEventProp.iHausNr => typeof(int),
            EEventProp.sBem => typeof(string[]),
            _ => throw new NotImplementedException(),
        };
    }

    public object GetPropValue(EEventProp prop)
    {
        return prop switch
        {
            EEventProp.eArt => eArt,
            EEventProp.iPerFamNr => iPerFamNr,
            EEventProp.iLfNr => iLfNr,
            EEventProp.iArtText => iArtText,
            EEventProp.iPrivacy => iPrivacy,
            EEventProp.xIsDead => xIsDead,
            EEventProp.dDatumV => dDatumV,
            EEventProp.sReg => sReg,
            EEventProp.sDatumV_S => sDatumV_S,
            EEventProp.sVChr => sVChr,
            EEventProp.dDatumB => dDatumB,
            EEventProp.sDatumB_S => sDatumB_S,
            EEventProp.sZusatz => sZusatz,
            EEventProp.sOrt_S => sOrt_S,
            EEventProp.iOrt => iOrt,
            EEventProp.iDatumText => iDatumText,
            EEventProp.iKBem => iKBem,
            EEventProp.iPlatz => iPlatz,
            EEventProp.iCausal => iCausal,
            EEventProp.iGrabNr => iGrabNr,
            EEventProp.iAn => iAn,
            EEventProp.iHausNr => iHausNr,
            EEventProp.sBem => sBem,
            _ => throw new NotImplementedException(),
        };
    }

    public T2 GetPropValue<T2>(EEventProp prop)
    {
        return (T2)GetPropValue(prop);
    }

    public void SetPropValue(EEventProp prop, object value)
    {
        if (GetPropValue(prop).Equals(value)) return;
        AddChangedProp(prop);
        switch (prop)
        {
            case EEventProp.eArt:
                eArt = (EEventArt)value;
                break;
            case EEventProp.iPerFamNr:
                iPerFamNr = (int)value;
                break;
            case EEventProp.iLfNr:
                iLfNr = (int)value;
                break;
            case EEventProp.iArtText:
                iArtText = (int)value;
                break;
            case EEventProp.iPrivacy:
                iPrivacy = (int)value;
                break;
            case EEventProp.xIsDead:
                sDeath = (bool)value ? "J" : " ";
                break;
            case EEventProp.dDatumV:
                dDatumV = (DateTime)value;
                break;
            case EEventProp.sReg:
                sReg = (string)value;
                break;
            case EEventProp.sDatumV_S:
                sDatumV_S = (string)value;
                break;
            case EEventProp.sVChr:
                sVChr = (string)value;
                break;
            case EEventProp.dDatumB:
                dDatumB = (DateTime)value;
                break;
            case EEventProp.sDatumB_S:
                sDatumB_S = (string)value;
                break;
            case EEventProp.sZusatz:
                sZusatz = (string)value;
                break;
            case EEventProp.sOrt_S:
                sOrt_S = (string)value;
                break;
            case EEventProp.iOrt:
                iOrt = (int)value;
                break;
            case EEventProp.iDatumText:
                iDatumText = (int)value;
                break;
            case EEventProp.iKBem:
                iKBem = (int)value;
                break;
            case EEventProp.iPlatz:
                iPlatz = (int)value;
                break;
            case EEventProp.iCausal:
                iCausal = (int)value;
                break;
            case EEventProp.iGrabNr:
                iGrabNr = (int)value;
                break;
            case EEventProp.iAn:
                iAn = (int)value;
                break;
            case EEventProp.iHausNr:
                iHausNr = (int)value;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void ClearChangedProps()
    {
        _changedPropList.Clear();
    }

    public void AddChangedProp(EEventProp prop)
    {
        _changedPropList.Add(prop);
    }

    public void SetDBData(IRecordset dB_EventTable, string[]? asProps = null)
    {
        if (asProps == null)
            asProps = _changedPropList.Select(p => p.ToString()).ToArray();
        foreach (var prop in asProps)
            switch (prop)
            {
                case nameof(IEventData.eArt):
                    dB_EventTable.Fields[nameof(EventFields.Art)].Value = eArt;
                    break;
                case nameof(IEventData.iPerFamNr):
                    dB_EventTable.Fields[nameof(EventFields.PerFamNr)].Value = iPerFamNr;
                    break;
                case nameof(IEventData.iLfNr):
                    dB_EventTable.Fields[nameof(EventFields.LfNr)].Value = iLfNr;
                    break;
                case nameof(IEventData.iArtText):
                    dB_EventTable.Fields[nameof(EventFields.ArtText)].Value = iArtText;
                    break;
                case nameof(IEventData.iPrivacy):
                    dB_EventTable.Fields[nameof(EventFields.priv)].Value = iPrivacy;
                    break;
                case nameof(IEventData.xIsDead):
                    dB_EventTable.Fields[nameof(EventFields.tot)].Value = sDeath;
                    break;
                case nameof(IEventData.dDatumV):
                    dB_EventTable.Fields[nameof(EventFields.DatumV)].Value = dDatumV;
                    break;
                case nameof(IEventData.sReg):
                    dB_EventTable.Fields[nameof(EventFields.Reg)].Value = sReg;
                    break;
                case nameof(IEventData.sDatumV_S):
                    dB_EventTable.Fields[nameof(EventFields.DatumV_S)].Value = sDatumV_S;
                    break;
                case nameof(IEventData.sVChr):
                    dB_EventTable.Fields[nameof(EventFields.VChr)].Value = sVChr;
                    break;
                case nameof(IEventData.dDatumB):
                    dB_EventTable.Fields[nameof(EventFields.DatumB)].Value = dDatumB;
                    break;
                case nameof(IEventData.sDatumB_S):
                    dB_EventTable.Fields[nameof(EventFields.DatumB_S)].Value = sDatumB_S;
                    break;
                case nameof(IEventData.sZusatz):
                    dB_EventTable.Fields[nameof(EventFields.Zusatz)].Value = sZusatz;
                    break;
                case nameof(IEventData.sOrt_S):
                    dB_EventTable.Fields[nameof(EventFields.Ort_S)].Value = sOrt_S;
                    break;
                case nameof(IEventData.iOrt):
                    dB_EventTable.Fields[nameof(EventFields.Ort)].Value = iOrt;
                    break;
                case nameof(IEventData.iDatumText):
                    dB_EventTable.Fields[nameof(EventFields.DatumText)].Value = iDatumText;
                    break;
                case nameof(IEventData.iKBem):
                    dB_EventTable.Fields[nameof(EventFields.KBem)].Value = iKBem;
                    break;
                case nameof(IEventData.iPlatz):
                    dB_EventTable.Fields[nameof(EventFields.Platz)].Value = iPlatz;
                    break;
                case nameof(IEventData.iCausal):
                    dB_EventTable.Fields[nameof(EventFields.Causal)].Value = iCausal;
                    break;
                case nameof(IEventData.iGrabNr):
                    dB_EventTable.Fields[nameof(EventFields.GrabNr)].Value = iGrabNr;
                    break;
                case nameof(IEventData.iAn):
                    dB_EventTable.Fields[nameof(EventFields.an)].Value = iAn;
                    break;
                case nameof(IEventData.iHausNr):
                    dB_EventTable.Fields[nameof(EventFields.Hausnr)].Value = iHausNr;
                    break;
                case nameof(IEventData.sBem):
                    dB_EventTable.Fields[nameof(EventFields.Bem1)].Value = sBem[1];
                    dB_EventTable.Fields[nameof(EventFields.Bem2)].Value = sBem[2];
                    dB_EventTable.Fields[nameof(EventFields.Bem3)].Value = sBem[3];
                    dB_EventTable.Fields[nameof(EventFields.Bem4)].Value = sBem[4];
                    break;
            }
    }

    public EEventArt eArt { get; private set; }
    public int iPerFamNr { get; private set; }
    public int iLfNr { get; private set; }

    public int iArtText { get; private set; }
    public string sArtText => _sArtText ??= _GetText(iArtText);
    public int iPrivacy { get; internal set; }
    public string sDeath { get; internal set; }
    public bool xIsDead => sDeath == "J";
    public DateTime dDatumV { get; internal set; }
    public string sReg { get; internal set; }
    public string sDatumV_S { get; internal set; }
    public string sVChr { get; internal set; }
    public DateTime dDatumB { get; internal set; }
    public string sDatumB_S { get; internal set; }
    public string sZusatz { get; internal set; }
    public string sOrt_S { get; internal set; }
    public int iOrt { get; internal set; }
    public int iDatumText { get; internal set; }
    public string sDatumText => _sDatumText ??= _GetText(iDatumText);
    public int iKBem { get; internal set; }
    public string sKBem => _sKBem ??= _GetText(iKBem);
    public int iPlatz { get; internal set; }
    public string sPlatz => _sPlatz ??= _GetText(iPlatz);
    public int iCausal { get; internal set; }
    public string sCausal => _sCausal ??= _GetText(iCausal);
    public int iGrabNr {get; private set;}
    public string sGrabNr => _sGrabNr ??= _GetText(iGrabNr);
    public int iAn { get; internal set; }
    public string sAn => _sAn ??= _GetText(iAn);
    public int iHausNr { get; private set; }
    public string sHausNr => _sHausNr ??= _GetText(iHausNr);

    public string[] sBem { get; } = new string[5];

    public (EEventArt eArt, int iLink, short iLfNr) ID => (eArt, iPerFamNr, (short)iLfNr);

    public IReadOnlyList<EEventProp> ChangedProps => _changedPropList;

}