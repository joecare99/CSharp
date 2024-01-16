using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;

namespace GenFree.Data
{
    internal class CWitnessData : CRSData<EWitnessProp, (int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>, IWitnessData
    {
        public CWitnessData(IRecordset db_Table) : base(db_Table)
        {
        }

        public int iPers { get; private set; }
        public int iWKennz { get; private set; }
        public int iLink { get; private set; }
        public EEventArt eArt { get; private set; }
        public short iLfNr { get; private set; }

        public override (int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr) ID =>
            (iLink, iPers, iWKennz, eArt, iLfNr);

        public override void FillData(IRecordset dB_Table)
        {
            iPers = dB_Table.Fields[nameof(WitnessFields.PerNr)].AsInt();
            iWKennz = dB_Table.Fields[nameof(WitnessFields.Kennz)].AsInt();
            iLink = dB_Table.Fields[nameof(WitnessFields.FamNr)].AsInt();
            eArt = dB_Table.Fields[nameof(WitnessFields.Art)].AsEnum<EEventArt>();
            iLfNr = (short)dB_Table.Fields[nameof(WitnessFields.LfNr)].AsInt();
        }

        public override Type GetPropType(EWitnessProp prop)
        {
            return prop switch
            {
                EWitnessProp.iPers => typeof(int),
                EWitnessProp.iWKennz => typeof(int),
                EWitnessProp.iLink => typeof(int),
                EWitnessProp.eArt => typeof(EEventArt),
                EWitnessProp.iLfNr => typeof(short),
                _ => throw new NotImplementedException(),
            };
        }

        public override object GetPropValue(EWitnessProp prop)
        {
            return prop switch
            {
                EWitnessProp.iPers => iPers,
                EWitnessProp.iWKennz => iWKennz,
                EWitnessProp.iLink => iLink,
                EWitnessProp.eArt => eArt,
                EWitnessProp.iLfNr => iLfNr,
                _ => throw new NotImplementedException(),
            };
        }

        protected override IRecordset? Seek((int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr) iD)
        {
            _db_Table.Index = nameof(WitnessIndex.Fampruef);
            _db_Table.Seek("=", iD.iLink, iD.iPers, iD.iWKennz, iD.eArt, iD.iLfNr);
            return _db_Table.NoMatch ? null : _db_Table;
        }

        public override void SetDBValue(IRecordset dB_Table, string[]? asProps)
        {
            throw new NotImplementedException();
        }

        public override void SetPropValue(EWitnessProp prop, object value)
        {
            if (EqualsProp(prop, value))
                return;
            AddChangedProp(prop);

            switch (prop)
            {
                case EWitnessProp.iPers:
                    iPers = (int)value;
                    break;
                case EWitnessProp.iWKennz:
                    iWKennz = (int)value;
                    break;
                case EWitnessProp.iLink:
                    iLink = (int)value;
                    break;
                case EWitnessProp.eArt:
                    eArt = (EEventArt)value;
                    break;
                case EWitnessProp.iLfNr:
                    iLfNr = (short)value;
                    break;
                default:
                    throw new NotImplementedException();
            }

        }
    }
}