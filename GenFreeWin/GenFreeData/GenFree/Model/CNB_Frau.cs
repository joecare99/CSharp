//using DAO;
using BaseLib.Helper;
using GenFree.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using System;

namespace GenFree.Model;

public class CNB_Frau(Func<IRecordset> recordset) : CUsesRecordSet<int>, INB_Frau
{
    protected override string __keyIndex => nameof(NB_Frau1Index.LfNr);

    protected override IRecordset _db_Table => recordset();

    public override IRecordset? Seek(int key, out bool xBreak)
    {
        IRecordset nB_Frau1Table = _db_Table;
        nB_Frau1Table.Index = __keyIndex;
        nB_Frau1Table.Seek("=", key);
        xBreak=nB_Frau1Table.NoMatch;
        return xBreak ? null : nB_Frau1Table;
    }
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="recordset">The recordset.</param>
    /// <returns>System.Int32.</returns>
    protected override int GetID(IRecordset recordset) 
        => recordset.Fields[NB_Frau1Fields.LfNr].AsInt();

    /// <summary>
    /// Reads the data from a specific record from Frauen-Tab (by ID).
    /// </summary>
    /// <param name="iNr">The LFD nr.</param>
    /// <param name="persNr">The pers nr.</param>
    /// <param name="famNr">The fam nr.</param>
    /// <param name="gen">The gen.</param>
    /// <param name="kek2">The kek2.</param>
    /// <param name="kek1">The kek1.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ReadData(int iNr, out int persNr, out int famNr, out int gen, out int kek2, out int kek1)
    {
        bool flag;
        gen = default;
        kek2 = default;
        kek1 = default;
        persNr = default;
        famNr = default;
        if (flag = Exists(iNr))
        {
            (gen, persNr, famNr) = RawReadBaseData();
            kek1 = _db_Table.Fields[NB_Frau1Fields.Kek1].AsInt();
            kek2 = _db_Table.Fields[NB_Frau1Fields.Kek2].AsInt();
        }

        return flag;
    }
    public bool CReadData(int iNr, out (int iGen, int iPers, int iFam)? value)
    {
        bool result;
        value = default;
        if (result = Exists(iNr))
        {
            value = RawReadBaseData();
        }
        return result;
    }
    public bool PersonExists(int iPersNr)
    {
        IRecordset nB_Frau1Table = _db_Table;
        nB_Frau1Table.Index = nameof(NB_Frau1Index.PerNR);
        nB_Frau1Table.Seek("=", iPersNr);
        return !nB_Frau1Table.NoMatch;
    }
    private (int iGen, int iPers, int iFam) RawReadBaseData()
    {
        IRecordset nB_Frau1Table = _db_Table;
        var num5 = nB_Frau1Table.Fields[NB_Frau1Fields.Gen].Value.AsInt();
        var iPers = nB_Frau1Table.Fields[NB_Frau1Fields.Nr].AsInt();
        var iFam = nB_Frau1Table.Fields[NB_Frau1Fields.Alt].AsInt();
        return (num5, iPers, iFam);
    }
    public void AddRow(int iNr, int iGen, int iPersNr, int iFamNr, int iKek1, int iKek2)
    {
        IRecordset nB_Frau1Table = _db_Table;
        nB_Frau1Table.AddNew();
        nB_Frau1Table.Fields[NB_Frau1Fields.Nr].Value = iPersNr;
        nB_Frau1Table.Fields[NB_Frau1Fields.Kek1].Value = iKek1;
        nB_Frau1Table.Fields[NB_Frau1Fields.Kek2].Value = iKek2;
        nB_Frau1Table.Fields[NB_Frau1Fields.Gen].Value = iGen;
        nB_Frau1Table.Fields[NB_Frau1Fields.LfNr].Value = iNr;
        nB_Frau1Table.Fields[NB_Frau1Fields.Alt].Value = iFamNr;
        nB_Frau1Table.Update();
    }

}