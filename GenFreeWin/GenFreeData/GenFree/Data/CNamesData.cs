using GenFree.Model.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenFree.Data;

public class CNamesData : CRSDataC<ENamesProp, (int, ETextKennz, int)>, INamesData
{
    public CNamesData(IRecordset dB_NamesTable) : base(dB_NamesTable)
    {
    }

    public override (int, ETextKennz, int) ID => (iPersNr, eTKennz, iLfNr);

    public int iPersNr { get; private set; }
    public ETextKennz eTKennz { get; private set; }
    public int iTextNr { get; private set; }
    public int iLfNr { get; private set; }
    public bool bRuf { get; private set; }
    public bool bSpitz { get; private set; }

    public override void FillData(IRecordset dB_Table)
    {
        iPersNr = dB_Table.Fields[NameFields.PersNr.AsFld()].AsInt();
        eTKennz = dB_Table.Fields[NameFields.Kennz.AsFld()].AsEnum<ETextKennz>();
        iTextNr = dB_Table.Fields[NameFields.Text.AsFld()].AsInt();
        iLfNr = dB_Table.Fields[NameFields.LfNr.AsFld()].AsInt();
        bRuf = dB_Table.Fields[NameFields.Ruf.AsFld()].AsBool();
        bSpitz = dB_Table.Fields[NameFields.Spitz.AsFld()].AsBool();
    }

    public override Type GetPropType(ENamesProp prop) => prop switch
    {
        ENamesProp.iPersNr => typeof(int),
        ENamesProp.eTKennz => typeof(ETextKennz),
        ENamesProp.iTextNr => typeof(int),
        ENamesProp.iLfNr => typeof(int),
        ENamesProp.bRuf => typeof(bool),
        ENamesProp.bSpitz => typeof(bool),
        _ => throw new NotImplementedException(),
    };

    public override object GetPropValue(ENamesProp prop)
    {
        return prop switch
        {
            ENamesProp.iPersNr => iPersNr,
            ENamesProp.eTKennz => eTKennz,
            ENamesProp.iTextNr => iTextNr,
            ENamesProp.iLfNr => iLfNr,
            ENamesProp.bRuf => bRuf,
            ENamesProp.bSpitz => bSpitz,
            _ => throw new NotImplementedException(),
        };
    }

    public override void SetDBValue(IRecordset dB_Table, string[]? asProps)
    {
        asProps ??= _changedPropsList.Select(i => i.ToString()).ToArray();
        foreach (var prop in asProps)
            switch (prop.AsEnum<ENamesProp>())
            {
                case ENamesProp.iPersNr:
                    dB_Table.Fields[NameFields.PersNr.AsFld()].Value = iPersNr;
                    break;
                case ENamesProp.eTKennz:
                    dB_Table.Fields[NameFields.Kennz.AsFld()].Value = eTKennz;
                    break;
                case ENamesProp.iTextNr:
                    dB_Table.Fields[NameFields.Text.AsFld()].Value = iTextNr;
                    break;
                case ENamesProp.iLfNr:
                    dB_Table.Fields[NameFields.LfNr.AsFld()].Value = iLfNr;
                    break;
                case ENamesProp.bRuf:
                    dB_Table.Fields[NameFields.Ruf.AsFld()].Value = bRuf;
                    break;
                case ENamesProp.bSpitz:
                    dB_Table.Fields[NameFields.Spitz.AsFld()].Value = bSpitz;
                    break;
                default:
                    throw new NotImplementedException();
            }
    }

    public override void SetPropValue(ENamesProp prop, object value)
    {
        if (EqualsProp(prop, value)) return;
        AddChangedProp(prop);
        object _ = prop switch
        {
            ENamesProp.iPersNr => iPersNr = (int)value,
            ENamesProp.eTKennz => eTKennz = (ETextKennz)value,
            ENamesProp.iTextNr => iTextNr = (int)value,
            ENamesProp.iLfNr => iLfNr = (int)value,
            ENamesProp.bRuf => bRuf = (bool)value,
            ENamesProp.bSpitz => bSpitz = (bool)value,
            _ => throw new NotImplementedException(),
        };
    }

    protected override IRecordset? Seek((int, ETextKennz, int) iD)
    {
        _db_Table.Index = NameIndex.NamKenn.AsFld();
        _db_Table.Seek("=", iD.Item1, iD.Item2, iD.Item3);
        return _db_Table.NoMatch ? null : _db_Table;
    }
}