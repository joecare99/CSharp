//using DAO;
using BaseLib.Helper;
using GenFree.Data;
using GenFree.Models;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Models;

public class CNB_Ahnen(Func<IRecordset> recordset) : CUsesRecordSet<int>, INB_Ahnen
{
    protected override string __keyIndex => nameof(NB_AhnenIndex.LfNr);

    protected override IRecordset _db_Table => recordset();

    public override IRecordset? Seek(int key, out bool xBreak)
    {
        IRecordset nB_AhnenTable = _db_Table;
        nB_AhnenTable.Index = __keyIndex;
        nB_AhnenTable.Seek("=", key);
        xBreak = nB_AhnenTable.NoMatch;
        return xBreak ? null : nB_AhnenTable;
    }

    protected override int GetID(IRecordset recordset)
        => recordset.Fields[NB_AhnenFields.Weiter].AsInt();

    /// <summary>
    /// Reads the data from a specific record from Frauen-Tab (by ID).
    /// </summary>
    /// <param name="LfdNr">The LFD nr.</param>
    /// <param name="persNr">The pers nr.</param>
    /// <param name="famNr">The fam nr.</param>
    /// <param name="gen">The gen.</param>
    /// <param name="kek2">The kek2.</param>
    /// <param name="kek1">The kek1.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ReadData(int LfdNr, out int persNr, out int famNr, out int gen, out int kek2, out int kek1)
    {
        bool flag;
        gen = default;
        kek2 = default;
        kek1 = default;
        persNr = default;
        famNr = default;
        if (flag = Exists(LfdNr))
        {
            (gen, persNr, famNr) = RawReadBaseData();
            kek1 = _db_Table.Fields[NB_AhnenFields.Ahn1].AsInt();
            kek2 = _db_Table.Fields[NB_AhnenFields.Ahn2].AsInt();
        }

        return flag;
    }
    public bool CReadData(int num6, out (int iGen, int iPers, int iFam)? value)
    {
        bool result;
        value = default;
        if (result = Exists(num6))
        {
            value = RawReadBaseData();
        }
        return result;
    }
    public bool PersonExists(int iPersNr)
    {
        IRecordset nB_AhnenTable = _db_Table;
        nB_AhnenTable.Index = nameof(NB_AhnenIndex.PerNR);
        nB_AhnenTable.Seek("=", iPersNr);
        return !nB_AhnenTable.NoMatch;
    }
    private (int iGen, int iPers, int iFam) RawReadBaseData()
    {
        IRecordset nB_AhnenTable = _db_Table;
        var iGene = nB_AhnenTable.Fields[NB_AhnenFields.Gene].AsInt();
        var iPers = nB_AhnenTable.Fields[NB_AhnenFields.PerNr].AsInt();
        var iFam = nB_AhnenTable.Fields[NB_AhnenFields.Ehe].AsInt();
        return (iGene, iPers, iFam);
    }

    public void Commit(int persInArb, int famInArb, int iGen, int iAhn1, int iAhn2, string name)
    {
        bool noMatch = !PersonExists(persInArb);
        if (noMatch)
        {
            AddRow(persInArb, iGen, iAhn1, iAhn2, 0, famInArb);
        }
        else if (_db_Table.Fields[NB_AhnenFields.Ahn1].AsInt() != 0)
        {
            SetWeiterRaw();
            AddRow(persInArb, iGen, iAhn1, iAhn2, 1, famInArb);
        }
        else
        {
            EditRaw(iGen, iAhn1, iAhn2, famInArb, name);
        }
    }

    public void AddRow(int iPersNr, int iGen, int iWtr, int iAhn1, int iAhn2, int iFamNr)
    {
        IRecordset nB_AhnenTable = _db_Table;
        nB_AhnenTable.AddNew();
        nB_AhnenTable.Fields[NB_AhnenFields.PerNr].Value = iPersNr;
        nB_AhnenTable.Fields[NB_AhnenFields.Gene].Value = iGen;
        nB_AhnenTable.Fields[NB_AhnenFields.Ahn1].Value = iAhn1;
        nB_AhnenTable.Fields[NB_AhnenFields.Ahn2].Value = iAhn2;
        nB_AhnenTable.Fields[NB_AhnenFields.Ahn3].Value = 0;
        nB_AhnenTable.Fields[NB_AhnenFields.Weiter].Value = iWtr;
        nB_AhnenTable.Fields[NB_AhnenFields.Ehe].Value = iFamNr;
        nB_AhnenTable.Update();
    }
    public void EditRaw(int iGene, int iAhn1, int iAhn2, int famInArb, string surName)
    {
        IRecordset nB_AhnenTable = _db_Table;
        nB_AhnenTable.Edit();
        nB_AhnenTable.Fields[NB_AhnenFields.Gene].Value = iGene;
        nB_AhnenTable.Fields[NB_AhnenFields.Ahn1].Value = iAhn1;
        nB_AhnenTable.Fields[NB_AhnenFields.Ahn2].Value = iAhn2;
        nB_AhnenTable.Fields[NB_AhnenFields.Ehe].Value = famInArb;
        nB_AhnenTable.Fields[NB_AhnenFields.Name].Value = surName;
        nB_AhnenTable.Update();
    }
    public void SetWeiterRaw()
    {
        IRecordset nB_AhnenTable = _db_Table;
        nB_AhnenTable.Edit();
        nB_AhnenTable.Fields[NB_AhnenFields.Weiter].Value = 1;
        nB_AhnenTable.Update();
    }
}