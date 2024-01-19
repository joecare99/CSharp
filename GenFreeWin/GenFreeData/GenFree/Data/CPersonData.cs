using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using GenFree.Model.Data;

namespace GenFree.Data;

public class CPersonData : CRSData<EPersonProp, int>, IPersonData
{
    #region static Properties
    private static Func<IRecordset> GetPersonTable { get; set; }
    private static Func<int, string> _GetText;
    private static Func<int, (string, string)> _GetText2;
    #endregion
    protected override Enum _keyIndex => PersonIndex.PerNr;

    public DateTime dEditDat { get; set; }
    public DateTime dAnlDatum { get; private set; }

    private int _iPersNr;

    public Guid gUid { get; internal set; }
    public string SurName { get; private set; }
    public string Givennames { get; internal set; }
    public IList<string> Givenname { get; } = new List<string>();
    public IList<string> Nickname { get; } = new List<string>();
    public IList<string> Callname { get; } = new List<string>();
    public string FullSurName { get; private set; }
  //  public string FullName { get; private set; }
    public string sSex { get; private set; }
    public string sKonv { get; private set; }

    public string Prefix { get; internal set; }
    public string Prae { get; internal set; }
    public string Suffix { get; internal set; }
    public string Alias { get; internal set; }
    public string Clan { get; internal set; }

    public DateTime dBirth { get; private set; }
    public string Birthday { get; internal set; }
    public DateTime dBaptised { get; private set; }
    public string Baptised { get; internal set; }
    public DateTime dDeath { get; private set; }
    public string Death { get; internal set; }
    public bool xDead { get; internal set; }
    public DateTime dBurial { get; private set; }
    public string Burial { get; internal set; }
    public string sBurried { get; internal set; }
    public string sOFB { get; private set; }
    public string[] sSuch { get; } = new string[7];
    public string Stat { get; private set; }
    public int iReligi { get; set; }
    public string[] sBem { get; } = new string[4];
    public string sAge { get; internal set; }
    public float fAge { get; internal set; }
    public string sPruefen { get; set; }


    public CPersonData(IRecordset dB_PersonTable) : base(dB_PersonTable) { }

    public override void FillData(IRecordset dB_PersonTable)
    {
        _ID = dB_PersonTable.Fields[nameof(PersonFields.PersNr)].AsInt();
        sOFB = dB_PersonTable.Fields[nameof(PersonFields.OFB)].AsString();
        sPruefen = dB_PersonTable.Fields[nameof(PersonFields.Pruefen)].AsString();
        sSuch[1] = dB_PersonTable.Fields[nameof(PersonFields.Such1)].AsString();
        sSuch[2] = dB_PersonTable.Fields[nameof(PersonFields.Such2)].AsString();
        sSuch[3] = dB_PersonTable.Fields[nameof(PersonFields.Such3)].AsString();
        sSuch[4] = dB_PersonTable.Fields[nameof(PersonFields.Such4)].AsString();
        sSuch[5] = dB_PersonTable.Fields[nameof(PersonFields.Such5)].AsString();
        sSuch[6] = dB_PersonTable.Fields[nameof(PersonFields.Such6)].AsString();
        sSex = dB_PersonTable.Fields[nameof(PersonFields.Sex)].AsString();
        sKonv = dB_PersonTable.Fields[nameof(PersonFields.Konv)].AsString();
        iReligi = dB_PersonTable.Fields[nameof(PersonFields.religi)].AsInt();
        sBem[1] = dB_PersonTable.Fields[nameof(PersonFields.Bem1)].AsString();
        sBem[2] = dB_PersonTable.Fields[nameof(PersonFields.Bem2)].AsString();
        sBem[3] = dB_PersonTable.Fields[nameof(PersonFields.Bem3)].AsString();
        dEditDat = dB_PersonTable.Fields[nameof(PersonFields.EditDat)].AsDate();
        dAnlDatum = dB_PersonTable.Fields[nameof(PersonFields.AnlDatum)].AsDate();
        gUid = dB_PersonTable.Fields[nameof(PersonFields.PUid)].AsGUID();
    }

    static CPersonData()
    {
        Reset();
    }

    public static void Reset()
    {
        GetPersonTable = () => DataModul.DB_PersonTable;
        _GetText = (i) => "";
        _GetText2 = (i) => ("", "");
    }

    public CPersonData(int iPersonNr) : base(GetPersonTable())
    {
        var _dB_PersonTable = Seek(iPersonNr);
        if (_dB_PersonTable != null)
            FillData(_dB_PersonTable);
        else
        {
            _iPersNr = iPersonNr;
            gUid = Guid.NewGuid();
        }
    }

    public CPersonData() : base(GetPersonTable())
    {
        _iPersNr = 0;
        gUid = Guid.NewGuid();
    }


    public static void SetDataFkc(Func<IRecordset> value)
    {
        GetPersonTable = value;
    }

    public static void SetGetText(Func<int, string> getText)
    {
        _GetText = getText;
    }
    public static void SetGetText2(Func<int, (string, string)> getText)
    {
        _GetText2 = getText;
    }

    public override int ID => _iPersNr;
    private int _ID
    {
        set
        {
            if (_iPersNr != value)
            {
                if (_iPersNr != 0)
                {
                    Clear();
                }
                _iPersNr = value;
            }
        }
    }

    public bool isEmpty =>
        string.IsNullOrWhiteSpace(Givennames)
        && string.IsNullOrWhiteSpace(SurName)
        && string.IsNullOrWhiteSpace(Suffix)
        && string.IsNullOrWhiteSpace(Prefix)
        && string.IsNullOrWhiteSpace(sOFB)
        && string.IsNullOrWhiteSpace(SurName)
        && string.IsNullOrWhiteSpace(sBem[1])
        && string.IsNullOrWhiteSpace(sBem[2])
        && string.IsNullOrWhiteSpace(sBem[3]);

    public void SetPersonNr(int i) { _ID = i; }

    public void SetFullSurname(string value)
    {
        FullSurName = value;
    }
    public void SetFull(string value) => SetPropValue(EPersonProp.SurName, value);

    public void SetDates(string[] value, Func<string, string, string>? SetAge = null)
    {
        for (int i = 11; i <= 14; i++)
        {
            if (value.Length > i)
            {
                switch ((EEventArt)(i + 90))
                {
                    default:
                        Birthday = value[i];
                        dBirth = value[i].AsDate();
                        break;
                    case EEventArt.eA_Baptism:
                        Baptised = value[i];
                        dBaptised = value[i].AsDate();
                        break;
                    case EEventArt.eA_Death:
                        Death = value[i];
                        dDeath = value[i].AsDate();
                        break;
                    case EEventArt.eA_Burial:
                        Burial = value[i];
                        dBurial = value[i].AsDate();
                        break;
                }
            }
        }
        sAge = SetAge?.Invoke(Birthday, Death) ?? "";
    }

    public void SetDates(DateTime[] value, Func<DateTime, DateTime, float>? SetAge = null)
    {
        for (EEventArt i = EEventArt.eA_Birth; i <= EEventArt.eA_Burial; i++)
        {
            var iX = (int)i - 100;
            if (value.Length > iX)
            {
                switch (i)
                {
                    default:
                        dBirth = value[iX];
                        break;
                    case EEventArt.eA_Baptism:
                        dBaptised = value[iX];
                        break;
                    case EEventArt.eA_Death:
                        dDeath = value[iX];
                        break;
                    case EEventArt.eA_Burial:
                        dBurial = value[iX];
                        break;
                }
            }
        }
        fAge = SetAge?.Invoke(dBirth, dDeath) ?? default;
    }


    public void Clear()
    {
        foreach (PropertyInfo p in GetType().GetProperties())
        {
            if (p.CanWrite)
                try
                {
                    if (p.PropertyType == typeof(string))
                        p.SetValue(this, "");
                    else if (p.PropertyType == typeof(DateTime))
                        p.SetValue(this, default(DateTime));
                    else if (p.PropertyType == typeof(int))
                        p.SetValue(this, 0);
                    else if (p.PropertyType == typeof(Guid))
                        p.SetValue(this, Guid.Empty);
                }
                catch { }
            else
              if (p.PropertyType == typeof(IList<string>))
                ((IList<string>)p.GetValue(this)).Clear();
            else if (p.PropertyType == typeof(string[]))
               new[] {"","","","" }.IntoString( ((string[])p.GetValue(this)));


        }
    }

    public void SetPersonNames(int[] iName, (int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN)
    {
        (SurName, sBurried) = _GetText2(iName[(int)ENameKennz.nkName]); // ??
        Prefix = _GetText(iName[(int)ENameKennz.nkPrefix]);
        Suffix = _GetText(iName[(int)ENameKennz.nkSuffix]);
        Clan = _GetText(iName[(int)ENameKennz.nkClanName]);
        Prae = _GetText(iName[(int)ENameKennz.nkPraeName]);  //Kont[7]
        Alias = _GetText(iName[(int)ENameKennz.nkAlias]);
        Stat = _GetText(iName[(int)ENameKennz.nkStatus]); // Person.Stat

        WorkNames(aiVorns, xInclLN);
    }

    private void WorkNames((int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN)
    {
        string sGivennames = "";
        Givenname.Clear();
        var num5 = 1;
        while (num5 < aiVorns.Length && aiVorns[num5].iName != 0)
        {
            var txts = _GetText2(aiVorns[num5].iName);
            Givenname.Add(txts.Item1);
            var sQuoteS = (aiVorns[num5].xRuf, aiVorns[num5].xNick) switch
            {
                (false, true) => "'",
                (true, _) => "\"",
                _ => ""
            };
            if (aiVorns[num5].xRuf)
            {
                Callname.Add(txts.Item1);
            }
            if (aiVorns[num5].xNick)
            {
                Nickname.Add(txts.Item1);
            }
            sGivennames += txts.Item1.TrimEnd().FrameIfNEoW(sQuoteS)+" ";
            if (!string.IsNullOrWhiteSpace(txts.Item2) && xInclLN)
            {
                sGivennames += $">{txts.Item2.Trim()}< ";
            }
            num5 += 1;
        }
        Givennames = sGivennames.TrimEnd();
    }

    public void SetData(IEventData cEvt)
    {
        switch (cEvt.eArt)
        {
            case EEventArt.eA_Birth:
                dBirth = cEvt.dDatumV;
                break;
            case EEventArt.eA_Baptism:
                dBaptised = cEvt.dDatumV;
                break;
            case EEventArt.eA_Death:
                xDead = cEvt.xIsDead || cEvt.dDatumV != default;
                dDeath = cEvt.dDatumV;
                break;
            case EEventArt.eA_Burial:
                sBurried = "J";
                dBurial = cEvt.dDatumV;
                break;
            default:
                break;
        }
    }
    public void Update()
    {
        throw new NotImplementedException();
    }

    public void SetSex(string sSex) => SetPropValue(EPersonProp.sSex, sSex);

    public override void SetDBValue(IRecordset dB_PersonTable, string[]? asProps)
    {
        asProps ??= _changedPropsList.Select((e) => e.ToString()).ToArray();
        foreach (var prop in asProps)
        {
            switch (prop.AsEnum<EPersonProp>())
            {
                case EPersonProp.ID:
                    dB_PersonTable.Fields[nameof(PersonFields.PersNr)].Value = ID;
                    break;
                case EPersonProp.sSex:
                    dB_PersonTable.Fields[nameof(PersonFields.Sex)].Value = sSex;
                    break;
                case EPersonProp.gUid:
                    dB_PersonTable.Fields[nameof(PersonFields.PUid)].Value = gUid;
                    break;
                case EPersonProp.SurName:
                    break;
                case EPersonProp.Givennames:
                    break;
                case EPersonProp.dBirth:
                    break;
                case EPersonProp.dBaptised:
                    break;
                case EPersonProp.dDeath:
                    break;
                case EPersonProp.dBurial:
                    break;
                case EPersonProp.sOFB:
                    dB_PersonTable.Fields[nameof(PersonFields.OFB)].Value = sOFB;
                    break;
                case EPersonProp.sSuch:
                    dB_PersonTable.Fields[nameof(PersonFields.Such1)].Value = sSuch[1];
                    dB_PersonTable.Fields[nameof(PersonFields.Such2)].Value = sSuch[2];
                    dB_PersonTable.Fields[nameof(PersonFields.Such3)].Value = sSuch[3];
                    dB_PersonTable.Fields[nameof(PersonFields.Such4)].Value = sSuch[4];
                    dB_PersonTable.Fields[nameof(PersonFields.Such5)].Value = sSuch[5];
                    dB_PersonTable.Fields[nameof(PersonFields.Such6)].Value = sSuch[6];
                    break;
                case EPersonProp.iReligi:
                    dB_PersonTable.Fields[nameof(PersonFields.religi)].Value = iReligi;
                    break;
                case EPersonProp.sKonv:
                    dB_PersonTable.Fields[nameof(PersonFields.Konv)].Value = sKonv;
                    break;
                case EPersonProp.sBem:
                    dB_PersonTable.Fields[nameof(PersonFields.Bem1)].Value = sBem[1];
                    dB_PersonTable.Fields[nameof(PersonFields.Bem2)].Value = sBem[2];
                    dB_PersonTable.Fields[nameof(PersonFields.Bem3)].Value = sBem[3];
                    break;
                case EPersonProp.sPruefen:
                    dB_PersonTable.Fields[nameof(PersonFields.Pruefen)].Value = sPruefen;
                    break;
                case EPersonProp.dAnlDatum:
                    dB_PersonTable.Fields[nameof(PersonFields.AnlDatum)].Value = dAnlDatum;
                    break;
                case EPersonProp.dEditDat:
                    dB_PersonTable.Fields[nameof(PersonFields.EditDat)].Value = dEditDat;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public override Type GetPropType(EPersonProp prop) => prop switch
    {
        EPersonProp.ID => typeof(int),
        EPersonProp.sSex => typeof(string),
        EPersonProp.sBem => typeof(string[]),
        EPersonProp.gUid => typeof(Guid),
        EPersonProp.SurName => typeof(string),
        EPersonProp.Givennames => typeof(string),
        EPersonProp.dBirth => typeof(DateTime),
        EPersonProp.dBaptised => typeof(DateTime),
        EPersonProp.dDeath => typeof(DateTime),
        EPersonProp.dBurial => typeof(DateTime),
        EPersonProp.sOFB => typeof(string),
        EPersonProp.sSuch => typeof(string[]),
        EPersonProp.iReligi => typeof(int),
        EPersonProp.sPruefen => typeof(string),
        EPersonProp.sKonv => typeof(string),
        EPersonProp.dAnlDatum => typeof(DateTime),
        EPersonProp.dEditDat => typeof(DateTime),
        _ => throw new NotImplementedException(),
    };

    public override object GetPropValue(EPersonProp prop) => prop switch
    {
        EPersonProp.ID => ID,
        EPersonProp.gUid => gUid,
        EPersonProp.SurName => SurName,
        EPersonProp.Givennames => Givennames,
        EPersonProp.sSex => sSex,
        EPersonProp.dBirth => dBirth,
        EPersonProp.dBaptised => dBaptised,
        EPersonProp.dDeath => dDeath,
        EPersonProp.dBurial => dBurial,
        EPersonProp.sOFB => sOFB,
        EPersonProp.sSuch => sSuch,
        EPersonProp.iReligi => iReligi,
        EPersonProp.sBem => sBem,
        EPersonProp.sKonv => sKonv,
        EPersonProp.sPruefen => sPruefen,
        EPersonProp.dAnlDatum => dAnlDatum,
        EPersonProp.dEditDat => dEditDat,
        _ => throw new NotImplementedException(),
    };

    public override void SetPropValue(EPersonProp prop, object value)
    {
        if (EqualsProp(prop,value)) return;
        AddChangedProp(prop);
        object _ = prop switch
        {
            EPersonProp.sSex => sSex = (string)value,
            EPersonProp.sBem when value is ListItem<int> l => sBem[l.ItemData] = l.ItemString,
            EPersonProp.sBem when value is string[] aS => aS.IntoString(sBem),
            EPersonProp.sSuch when value is ListItem<int> l => sSuch[l.ItemData] = l.ItemString,
            EPersonProp.sPruefen => sPruefen = (string)value,
            EPersonProp.sKonv => sKonv = (string)value,
            EPersonProp.gUid => gUid = (Guid)value,
            EPersonProp.dBirth => dBirth = (DateTime)value,
            EPersonProp.dBaptised => dBaptised = (DateTime)value,
            EPersonProp.dDeath => dDeath = (DateTime)value,
            EPersonProp.dBurial => dBurial = (DateTime)value,
            EPersonProp.dAnlDatum => dAnlDatum = (DateTime)value,
            EPersonProp.dEditDat => dEditDat = (DateTime)value,
            EPersonProp.SurName => SurName = (string)value,
            EPersonProp.Givennames => Givennames = (string)value,
            EPersonProp.iReligi => iReligi = (int)value,
            EPersonProp.sOFB => sOFB = (string)value,
            _ => throw new NotImplementedException(),
        };
    }

}