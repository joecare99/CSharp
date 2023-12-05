using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Data
{
    public class CNames : CUsesRecordSet<(int, ETextKennz, int)>, INames
    {
        private const string cNameIndex = "NABD2CU";

        private Func<IRecordset> __db_Table;

        public CNames(Func<IRecordset> value)
        {
            __db_Table = value;
        }

        protected override string _keyIndex => nameof(NameIndex.Vollname);

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

        public IEnumerable<INamesData> ReadAll()
        {
            IRecordset dB_NamesTable = _db_Table;
            dB_NamesTable.Index = _keyIndex;
            dB_NamesTable.MoveFirst();
            while (!dB_NamesTable.EOF)
            {
                yield return new CNamesData(dB_NamesTable);
                dB_NamesTable.MoveNext();
            }
        }

        public bool ReadData((int, ETextKennz, int) key, out INamesData? data)
        {
            var dB_NamesTable = Seek(key, out var xBreak);
            data = xBreak ? null : new CNamesData(dB_NamesTable);
            return !xBreak;
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
            dB_NamesTable.Index = _keyIndex;
            dB_NamesTable.Seek("=", key.Item1, key.Item2, key.Item3);
            xBreak = dB_NamesTable.NoMatch;
            return xBreak ? null : dB_NamesTable;
        }

        public void SetData((int, ETextKennz, int) key, INamesData data, string[]? asProps = null)
        {
            var dB_NamesTable = Seek(key);
            if (dB_NamesTable != null)
            {
                dB_NamesTable.Edit();
                data.SetDBValue(dB_NamesTable, asProps);
                dB_NamesTable.Update();
            }
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
                if (iNx == ENameKennz.nkGivnName && sNamKennz is "F" or "V" && iNamLfdNr <= 15)
                {
                    aiVorns[iNamLfdNr] = (iNamTextNr, xNamRufN, xNamNickN);
                }
                dB_NameTable.MoveNext();
            }
            return true;
        }


        public void Update(int nPersNr, int nText, ETextKennz kennz, int lfNR = 0, byte calln = 0, byte nickn = 0)
        {
            IRecordset DB_NameTable = _db_Table;
            DB_NameTable.Index = _keyIndex;
            DB_NameTable.Seek("=", nPersNr, kennz, lfNR);
            if (!DB_NameTable.NoMatch)
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

    }
}
