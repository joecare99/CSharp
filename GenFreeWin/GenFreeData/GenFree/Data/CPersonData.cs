using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GenFree.Data;

public class CPersonData : IPersonData
{
    private static Func<IRecordset> GetPersonTable { get; set; } = () => DataModul.DB_PersonTable;

    private List<EPersonProp> _changedPropList = new();
    private IRecordset _dB_PersonTable;
    public DateTime dEditDat { get; set; }
    public DateTime dAnlDatum { get; private set; }

    private int _iPersNr;

    public Guid gUID { get; internal set; }
    public string SurName { get; private set; }
    public string Givennames { get; internal set; }
    public IList<string> Givenname { get; } = new List<string>();
    public IList<string> Nickname { get; } = new List<string>();
    public IList<string> Callname { get; } = new List<string>();
    public string FullSurName { get; private set; }
    public string FullName { get; private set; }
    public string sSex { get; private set; }
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
    public string sPruefen { get; set; }


    public CPersonData(IRecordset dB_PersonTable)
    {
        _dB_PersonTable = dB_PersonTable;
        FillData(dB_PersonTable);
    }

    public void FillData(IRecordset dB_PersonTable)
    {
        ID = dB_PersonTable.Fields[nameof(PersonFields.PersNr)].AsInt();
        sOFB = dB_PersonTable.Fields[nameof(PersonFields.OFB)].AsString();
        sPruefen = dB_PersonTable.Fields[nameof(PersonFields.Pruefen)].AsString();
        sSuch[1] = dB_PersonTable.Fields[nameof(PersonFields.Such1)].AsString();
        sSuch[2] = dB_PersonTable.Fields[nameof(PersonFields.Such2)].AsString();
        sSuch[3] = dB_PersonTable.Fields[nameof(PersonFields.Such3)].AsString();
        sSuch[4] = dB_PersonTable.Fields[nameof(PersonFields.Such4)].AsString();
        sSuch[5] = dB_PersonTable.Fields[nameof(PersonFields.Such5)].AsString();
        sSuch[6] = dB_PersonTable.Fields[nameof(PersonFields.Such6)].AsString();
        sSex = dB_PersonTable.Fields[nameof(PersonFields.Sex)].AsString();
        iReligi = dB_PersonTable.Fields[nameof(PersonFields.religi)].AsInt();
        sBem[1] = dB_PersonTable.Fields[nameof(PersonFields.Bem1)].AsString();
        sBem[2] = dB_PersonTable.Fields[nameof(PersonFields.Bem2)].AsString();
        sBem[3] = dB_PersonTable.Fields[nameof(PersonFields.Bem3)].AsString();
        dEditDat = dB_PersonTable.Fields[nameof(PersonFields.EditDat)].AsDate();
        dAnlDatum = dB_PersonTable.Fields[nameof(PersonFields.AnlDatum)].AsDate();
    }

    public CPersonData(int iPersonNr)
    {
        _dB_PersonTable = GetPersonTable();
        _dB_PersonTable.Index = nameof(PersonIndex.PerNr);
        _dB_PersonTable.Seek("=", iPersonNr);
        if (!_dB_PersonTable.NoMatch)
            FillData(_dB_PersonTable);
        else
        {
            _iPersNr = iPersonNr;
            gUID = Guid.NewGuid();
        }
    }

    public CPersonData()
    {
        _dB_PersonTable = GetPersonTable();
        _iPersNr = 0;
        gUID = Guid.NewGuid();
    }

    public int ID
    {
        get => _iPersNr; private set
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

    public bool isEmpty => throw new NotImplementedException();

    public IReadOnlyList<EPersonProp> ChangedProps => throw new NotImplementedException();

    public void SetPersonNr(int i) { ID = i; }

    public void SetFullSurname(string value)
    {
        FullSurName = value;
    }
    public void SetFull(string value)
    {
        FullName = value;
    }

    public void SetDates(string[] value, Func<string, string, string>? SetAge = null)
    {
        for (int i = 11; i <= 14; i++)
        {
            if (value.Length > i)
            {
                switch ((EEventArt)(i + 90))
                {
                    case EEventArt.eA_Birth:
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
    public void Clear()
    {
        foreach (PropertyInfo p in GetType().GetProperties())
        {
            if (p.CanWrite)
                try
                {
                    p.SetValue(this, "");
                }
                catch { }
        }
    }

    public void SetPersonNames(int[] iName, (int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN)
    {
        var Person = this;
        (Person.SurName, Person.sBurried) = DataModul.TextLese2(iName[(int)ENameKennz.nkName]);
        Person.Prefix = DataModul.TextLese1(iName[(int)ENameKennz.nkPrefix]);
        Person.Suffix = DataModul.TextLese1(iName[(int)ENameKennz.nkSuffix]);
        Person.Clan = DataModul.TextLese1(iName[(int)ENameKennz.nkClanName]);
        Person.Prae = DataModul.TextLese1(iName[(int)ENameKennz.nkPraeName]);  //Kont[7]
        Person.Alias = DataModul.TextLese1(iName[(int)ENameKennz.nkAlias]);
        Person.Stat = DataModul.TextLese1(iName[(int)ENameKennz.nkStatus]); // Person.Stat

        WorkNames(aiVorns, xInclLN, Person);
    }

    private static void WorkNames((int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN, CPersonData Person)
    {
        string sGivennames = "";
        Person.Givenname.Clear();
        var num5 = 1;
        while (num5 <= 15 && aiVorns[num5].iName != 0)
        {
            var txts = DataModul.TextLese2(aiVorns[num5].iName);
            Person.Givenname.Add(txts.Item1);
            var sQuoteS = (aiVorns[num5].xRuf, aiVorns[num5].xNick) switch
            {
                (false, true) => "'",
                (true, _) => "\"",
                _ => ""
            };
            if (aiVorns[num5].xRuf)
            {
                Person.Callname.Add(txts.Item1);
            }
            if (aiVorns[num5].xNick)
            {
                Person.Nickname.Add(txts.Item1);
            }
            sGivennames += txts.Item1.TrimEnd().FrameIfNEoW(sQuoteS);
            if (!string.IsNullOrWhiteSpace(txts.Item2) && xInclLN)
            {
                sGivennames += $">{txts.Item2.Trim()}< ";
            }
            num5 += 1;
        }
        Person.Givennames = sGivennames;
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
        }
    }
    public void Update()
    {
        throw new NotImplementedException();
    }

    public void SetSex(string sSex)
    {
        throw new NotImplementedException();
    }

    public void SetDBValue(IRecordset dB_PersonTable, string[]? asProps)
    {
        asProps ??= _changedPropList.Select((e) => e.ToString()).ToArray();
        foreach (var prop in asProps)
        {
            switch (prop)
            {
                case nameof(EPersonProp.gUid):
                    dB_PersonTable.Fields[nameof(PersonFields.PUid)].Value = gUID;
                    break;
                case nameof(EPersonProp.SurName):
                    break;
                case nameof(EPersonProp.Givennames):
                    break;
                case nameof(EPersonProp.sSex):
                    dB_PersonTable.Fields[nameof(PersonFields.Sex)].Value = sSex;
                    break;
                case nameof(EPersonProp.dBirth):
                    break;
                case nameof(EPersonProp.dBaptised):
                    break;
                case nameof(EPersonProp.dDeath):
                    break;
                case nameof(EPersonProp.dBurial):
                    break;
                case nameof(EPersonProp.sOFB):
                    dB_PersonTable.Fields[nameof(PersonFields.OFB)].Value = sOFB;
                    break;
                case nameof(EPersonProp.sSuch):
                    dB_PersonTable.Fields[nameof(PersonFields.Such1)].Value = sSuch[1];
                    dB_PersonTable.Fields[nameof(PersonFields.Such2)].Value = sSuch[2];
                    dB_PersonTable.Fields[nameof(PersonFields.Such3)].Value = sSuch[3];
                    dB_PersonTable.Fields[nameof(PersonFields.Such4)].Value = sSuch[4];
                    dB_PersonTable.Fields[nameof(PersonFields.Such5)].Value = sSuch[5];
                    dB_PersonTable.Fields[nameof(PersonFields.Such6)].Value = sSuch[6];
                    break;
                case nameof(EPersonProp.iReligi):
                    dB_PersonTable.Fields[nameof(PersonFields.religi)].Value = iReligi;
                    break;
                case nameof(EPersonProp.sBem):
                    dB_PersonTable.Fields[nameof(PersonFields.Bem1)].Value = sBem[1];
                    dB_PersonTable.Fields[nameof(PersonFields.Bem2)].Value = sBem[2];
                    dB_PersonTable.Fields[nameof(PersonFields.Bem3)].Value = sBem[3];
                    break;
                case nameof(EPersonProp.sPruefen):
                    dB_PersonTable.Fields[nameof(PersonFields.Pruefen)].Value = sPruefen;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public Type GetPropType(EPersonProp prop)
    {
        return prop switch
        {
            EPersonProp.gUid => typeof(Guid),
            EPersonProp.SurName => typeof(string),
            EPersonProp.Givennames => typeof(string),
            EPersonProp.sSex => typeof(string),
            EPersonProp.dBirth => typeof(DateTime),
            EPersonProp.dBaptised => typeof(DateTime),
            EPersonProp.dDeath => typeof(DateTime),
            EPersonProp.dBurial => typeof(DateTime),
            EPersonProp.sOFB => typeof(string),
            EPersonProp.sSuch => typeof(string[]),
            EPersonProp.iReligi => typeof(int),
            EPersonProp.sBem => typeof(string[]),
            EPersonProp.sPruefen => typeof(string),
            _ => throw new NotImplementedException(),
        };
    }

    public object GetPropValue(EPersonProp prop)
    {
        return prop switch
        {
            EPersonProp.gUid => gUID,
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
            EPersonProp.sPruefen => sPruefen,
            _ => throw new NotImplementedException(),
        };
    }

    public T2 GetPropValue<T2>(EPersonProp prop)
    {
        return (T2)GetPropValue(prop);
    }

    public void SetPropValue(EPersonProp prop, object value)
    {
        if (GetPropType(prop).GetMethod("Equals")?.Invoke(GetPropValue(prop), new[] { value }) as bool? ?? false)
            return;
        _changedPropList.Add(prop);
        object _ = prop switch
        {
            EPersonProp.gUid => gUID = (Guid)value,
            EPersonProp.SurName => SurName = (string)value,
            EPersonProp.Givennames => Givennames = (string)value,
            EPersonProp.sSex => sSex = (string)value,
            EPersonProp.dBirth => dBirth = (DateTime)value,
            EPersonProp.dBaptised => dBaptised = (DateTime)value,
            EPersonProp.dDeath => dDeath = (DateTime)value,
            EPersonProp.dBurial => dBurial = (DateTime)value,
            EPersonProp.sOFB => sOFB = (string)value,
            EPersonProp.sSuch when value is ListItem<int> l => sSuch[l.ItemData] = l.ItemString,
            EPersonProp.iReligi => iReligi = (int)value,
            EPersonProp.sBem when value is ListItem<int> l => sBem[l.ItemData] = l.ItemString,
            EPersonProp.sBem when value is string[] aS => aS.IntoString(sBem),
            EPersonProp.sPruefen => sPruefen = (string)value,
            _ => throw new NotImplementedException(),
        };
    }

    public void ClearChangedProps()
    {
        _changedPropList.Clear();
    }

    public void AddChangedProp(EPersonProp prop)
    {
        if (!_changedPropList.Contains(prop))
            _changedPropList.Add(prop);
    }
}