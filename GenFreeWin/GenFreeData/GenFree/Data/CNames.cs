﻿using GenFree.Model;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;
using BaseLib.Helper;

namespace GenFree.Data
{
    public class CNames : CUsesIndexedRSet<(int, ETextKennz, int),NameIndex,NameFields,INamesData>, INames
    {
        private const string cNameIndex = "NABD2CU";

        private Func<IRecordset> __db_Table;

        public CNames(Func<IRecordset> value)
        {
            __db_Table = value;
        }

        protected override NameIndex _keyIndex => NameIndex.Vollname;

        protected override IRecordset _db_Table => __db_Table();

        public void DeleteAllP(int persInArb)
        {
            IRecordset DB_NameTable = _db_Table;
            DB_NameTable.Index = nameof(NameIndex.PNamen);
            DB_NameTable.Seek("=", persInArb);
            while (!DB_NameTable.NoMatch
                    && !DB_NameTable.NoMatch
                    && !DB_NameTable.EOF
                    && DB_NameTable.Fields[nameof(NameFields.PersNr)].AsInt() == persInArb)
            {
                DB_NameTable.Delete();
                DB_NameTable.MoveNext();
            }
        }
        public bool DeleteNK(int persInArb, ETextKennz kennz)
        {
            SeekNK(persInArb, kennz, out var xBreak)?.Delete();
            return !xBreak;
        }

        public bool ExistText(int textNr)
        {
            IRecordset DB_NameTable = _db_Table;
            DB_NameTable.Index = nameof(NameIndex.TxNr);
            DB_NameTable.Seek("=", textNr);
            return !DB_NameTable.NoMatch;
        }
        public bool ExistsNK(int persInArb, ETextKennz eTKennz)
        {
            return SeekNK(persInArb, eTKennz, out _) != null;
        }

        private IRecordset? SeekNK(int persInArb, ETextKennz eTKennz, out bool xBreak)
        {
            IRecordset DB_NameTable = _db_Table;
            DB_NameTable.Index = nameof(NameIndex.NamKenn);
            DB_NameTable.Seek("=", persInArb, eTKennz);
            xBreak = DB_NameTable.NoMatch;
            return xBreak ? null : DB_NameTable;
        }

        public override IRecordset? Seek((int, ETextKennz, int) key, out bool xBreak)
        {
            IRecordset dB_NamesTable = _db_Table;
            dB_NamesTable.Index = $"{_keyIndex}";
            dB_NamesTable.Seek("=", key.Item1, key.Item2, key.Item3);
            xBreak = dB_NamesTable.NoMatch;
            return xBreak ? null : dB_NamesTable;
        }

        protected override (int, ETextKennz, int) GetID(IRecordset recordset)
        {
            return (recordset.Fields[nameof(NameFields.PersNr)].AsInt(),
                    recordset.Fields[nameof(NameFields.Kennz)].AsEnum<ETextKennz>(),
                    recordset.Fields[nameof(NameFields.LfNr)].AsInt()
                                                                            );
        }

        public bool ReadPersonNames(int PersonNr, out int[] aiName, out (int iName, bool xRuf, bool xNick)[] aiVorns)
        {
            IRecordset dB_NameTable = _db_Table;
            aiName = new int[16];
            aiName.Initialize();

            aiVorns = new (int, bool, bool)[16];
            aiVorns.Initialize();

            dB_NameTable.Index = nameof(NameIndex.PNamen);
            dB_NameTable.Seek("=", PersonNr);
            if (dB_NameTable.NoMatch)
            {
                return false;
            }
            var i = 0;
            while (!dB_NameTable.EOF
                && !dB_NameTable.NoMatch
                && ++i <= 99
                && !(dB_NameTable.Fields[nameof(NameFields.PersNr)].AsInt() != PersonNr))
            {
                string sNamKennz = dB_NameTable.Fields[nameof(NameFields.Kennz)].AsString();
                int iNamTextNr = dB_NameTable.Fields[nameof(NameFields.Text)].AsInt();
                int iNamLfdNr = dB_NameTable.Fields[nameof(NameFields.LfNr)].AsInt();
                bool xNamRufN = dB_NameTable.Fields[nameof(NameFields.Ruf)].AsInt() == 1;
                bool xNamNickN = dB_NameTable.Fields[nameof(NameFields.Spitz)].AsInt() == 1;
                var iNx = (ENameKennz)cNameIndex.IndexOf(sNamKennz) + 1;
                aiName[(int)iNx] = iNamTextNr;
                if (iNx == ENameKennz.nkGivnName
)
                {
                    if (sNamKennz is "F" or "V"
)
                    {
                        if (iNamLfdNr <= 15)
                        {
                            aiVorns[iNamLfdNr] = (iNamTextNr, xNamRufN, xNamNickN);
                        }
                    }
                }

                dB_NameTable.MoveNext();
            }
            return true;
        }


        public void Update(int nPersNr, int nText, ETextKennz kennz, int lfNR = 0, byte calln = 0, byte nickn = 0)
        {
            var DB_NameTable = Seek((nPersNr, kennz, lfNR));
            if (DB_NameTable?.NoMatch == false)
            {
                if (DB_NameTable.Fields[nameof(NameFields.Text)].AsInt() != nText)
                {
                    DB_NameTable.Edit();
                    DB_NameTable.Fields[nameof(NameFields.Text)].Value = nText;
                    DB_NameTable.Fields[nameof(NameFields.LfNr)].Value = lfNR;
                    DB_NameTable.Fields[nameof(NameFields.Ruf)].Value = calln;
                    DB_NameTable.Fields[nameof(NameFields.Spitz)].Value = nickn;
                    DB_NameTable.Update();
                }
            }
            else
            {
                DB_NameTable = _db_Table;
                DB_NameTable.AddNew();
                DB_NameTable.Fields[nameof(NameFields.PersNr)].Value = nPersNr;
                DB_NameTable.Fields[nameof(NameFields.Kennz)].Value = kennz;
                DB_NameTable.Fields[nameof(NameFields.Text)].Value = nText;
                DB_NameTable.Fields[nameof(NameFields.Ruf)].Value = calln;
                DB_NameTable.Fields[nameof(NameFields.Spitz)].Value = nickn;
                DB_NameTable.Fields[nameof(NameFields.LfNr)].Value = kennz is ETextKennz.V_ or ETextKennz.F_ ? lfNR : 0;
                DB_NameTable.Update();
            }
        }

        public override NameFields GetIndex1Field(NameIndex eIndex)
        {
            return eIndex switch
            {
                NameIndex.PNamen => NameFields.PersNr,
                NameIndex.TxNr => NameFields.Text,
                NameIndex.NamKenn => NameFields.Kennz,
                NameIndex.Vollname => NameFields.PersNr,
                _ => throw new ArgumentException(nameof(eIndex)),
            };
        }

        public void UpdateAllSetVal(NameIndex eIndex, NameFields eIndexField, int iIndexVal, int iNewVal)
        {
            IRecordset dB_NameTable = _db_Table;
            dB_NameTable.Index = $"{eIndex}";
            dB_NameTable.Seek("=", iIndexVal);
            while (!dB_NameTable.EOF
                    && !dB_NameTable.NoMatch
                    && !(dB_NameTable.Fields[$"{eIndexField}"].AsInt() != iIndexVal))
            {
                dB_NameTable.Edit();
                dB_NameTable.Fields[$"{eIndexField}"].Value = iNewVal;
                dB_NameTable.Update();
                dB_NameTable.MoveNext();
            }
        }

        public void DeleteAllPers(int num18)
        {
            IRecordset? _r;
            while ((_r = Seek(NameIndex.PNamen, num18)) != null)
            {
                _r.Delete();
            }
        }
        protected override INamesData GetData(IRecordset rs) => new CNamesData(rs);
    }
}
