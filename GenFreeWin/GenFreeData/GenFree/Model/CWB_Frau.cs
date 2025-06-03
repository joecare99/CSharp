using BaseLib.Helper;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Model;
using System;

namespace GenFree.Model;

public class CWB_Frau(Func<IRecordset> recordset) : CUsesRecordSet<int>, IWB_Frau
{
    protected override string __keyIndex => nameof(NB_Frau1Index.LfNr);

    protected override IRecordset _db_Table => recordset();

    /// <summary>
    /// Fügt die Elternteile einer Familie als neue Datensätze in die WB_Frau-Tabelle ein.
    /// </summary>
    /// <param name="family">Die Familienpersonen, deren Eltern hinzugefügt werden sollen.</param>
    public void AddParent(IFamilyPersons family)
    {
        IRecordset wB_FrauTable = _db_Table;
        foreach (var iParent in new[] { family.Mann, family.Frau })
            if (iParent > 0)
            {
                AddRow(iParent);
            }
    }
    /// <summary>
    /// Fügt eine neue Zeile mit der angegebenen Personen-/Familiennummer in die WB_Frau-Tabelle ein.
    /// </summary>
    /// <param name="iPerFamNr">Die Personen- oder Familiennummer.</param>
    public void AddRow(int iPerFamNr)
    {
        IRecordset wB_FrauTable = DataModul.WB_FrauTable;
        wB_FrauTable.AddNew();
        wB_FrauTable.Fields[NB_Frau1Fields.LfNr].Value = iPerFamNr;
        wB_FrauTable.Fields[NB_Frau1Fields.Nr].Value = 0;
        wB_FrauTable.Update();
    }
    /// <summary>
    /// Setzt das Feld "Nr" für alle Datensätze in der WB_Frau-Tabelle auf 0 zurück.
    /// </summary>
    public void ClearNr()
    {
        IRecordset wB_FrauTable = DataModul.WB_FrauTable;
        wB_FrauTable.MoveFirst();
        wB_FrauTable.Seek(">", 0);
        while (!wB_FrauTable.EOF)
        {
            wB_FrauTable.Edit();
            wB_FrauTable.Fields["Nr"].Value = 0;
            wB_FrauTable.Update();
            wB_FrauTable.MoveNext();
        }
    }
    /// <summary>
    /// Setzt das Feld "Nr" für einen bestimmten Datensatz auf 1 oder fügt einen neuen Datensatz hinzu.
    /// </summary>
    /// <param name="iPers">Die Personen-ID.</param>
    public void Commit(int iPers)
    {
        IRecordset wB_FrauTable = DataModul.WB_FrauTable;
        wB_FrauTable.Index = NB_Frau1Index.LfNr.AsFld();
        wB_FrauTable.Seek("=", iPers);
        if (!wB_FrauTable.NoMatch)
        {
            wB_FrauTable.Edit();
            wB_FrauTable.Fields[NB_Frau1Fields.Nr].Value = 1;
            wB_FrauTable.Update();
        }
        else
        {
            wB_FrauTable.AddNew();
            wB_FrauTable.Fields[NB_Frau1Fields.LfNr].Value = iPers;
            wB_FrauTable.Update();
        }
    }

    /// <summary>
    /// Löscht alle Datensätze aus der WB_Frau-Tabelle, bei denen das Feld "Nr" gleich 0 ist.
    /// </summary>
    public void DeleteEmpty()
    {
        IRecordset wB_FrauTable = DataModul.WB_FrauTable;
        wB_FrauTable.MoveFirst();
        wB_FrauTable.Seek(">", 0);

        while (!wB_FrauTable.EOF)
        {
            if (wB_FrauTable.Fields[NB_Frau1Fields.Nr].AsInt() == 0)
            {
                wB_FrauTable.Delete();
            }
            wB_FrauTable.MoveNext();
        }
    }

    /// <summary>
    /// Führt eine Aktion für alle Datensätze in der WB_Frau-Tabelle aus und aktualisiert das Namensfeld.
    /// </summary>
    /// <param name="DataModul_SearchTab_GetNameDate">Funktion, die Name und Datum für eine gegebene Personen-ID zurückgibt.</param>
    public void ForAll(Func<int, (string, string)> DataModul_SearchTab_GetNameDate)
    {
        IRecordset wB_FrauTable = DataModul.WB_FrauTable;
        wB_FrauTable.MoveFirst();
        while (!wB_FrauTable.EOF)
        {
            int iPerFamNr = wB_FrauTable.Fields[NB_Frau1Fields.LfNr].AsInt();
            if (0 != iPerFamNr)
            {
                var (sName, sDatum) = DataModul_SearchTab_GetNameDate(iPerFamNr);

                wB_FrauTable.Edit();
                wB_FrauTable.Fields[NB_Frau1Fields.Name].Value = sName.Left(40) + sDatum;
                wB_FrauTable.Update();
            }
            wB_FrauTable.MoveNext();
        }
    }

    /// <summary>
    /// Sucht einen Datensatz anhand des Schlüssels und gibt das zugehörige Recordset zurück.
    /// </summary>
    /// <param name="key">Der Schlüsselwert.</param>
    /// <param name="xBreak">Gibt an, ob kein passender Datensatz gefunden wurde.</param>
    /// <returns>Das gefundene Recordset oder null, wenn kein Treffer vorliegt.</returns>
    public override IRecordset? Seek(int key, out bool xBreak)
    {
        IRecordset wB_FrauenTab = _db_Table;
        wB_FrauenTab.Index = __keyIndex;
        wB_FrauenTab.Seek("=", key);
        xBreak = wB_FrauenTab.NoMatch;
        return xBreak ? null : wB_FrauenTab;
    }

    /// <summary>
    /// Setzt das Feld "NR" für die Eltern einer Familie auf 1, sofern vorhanden.
    /// </summary>
    /// <param name="family">Die Familiendaten.</param>
    public void SetParentTo1(IFamilyData family)
    {
        IRecordset wB_FrauTable = DataModul.WB_FrauTable;
        wB_FrauTable.Index = "LfNR";
        foreach (var iParent in new[] { family.Mann, family.Frau })
            if (iParent > 0)
            {
                wB_FrauTable.Seek("=", iParent);
                if (!wB_FrauTable.NoMatch)
                {
                    wB_FrauTable.Edit();
                    wB_FrauTable.Fields["NR"].Value = 1;
                    wB_FrauTable.Update();
                }
            }
    }

    
    ///<summary>
    /// Aktualisiert das Feld "Nr" für einen bestimmten Datensatz auf 1, falls vorhanden.
    /// </summary>
    /// <param name="iPerNr">Die Personen-ID.</param>
    /// <returns>True, wenn der Datensatz gefunden und aktualisiert wurde, sonst false.</returns>
    public bool Update(int iPerNr)
    {
        bool flag;
        IRecordset wB_FrauTable = _db_Table;
        wB_FrauTable.Index = NB_Frau1Index.LfNr.AsFld();
        wB_FrauTable.Seek("=", iPerNr);
        if (flag = !wB_FrauTable.NoMatch)
        {
            wB_FrauTable.Edit();
            wB_FrauTable.Fields[NB_Frau1Fields.Nr].Value = 1;
            wB_FrauTable.Update();
        }
        return flag;
    }

    protected override int GetID(IRecordset recordset)
        => recordset.Fields[NB_Frau1Fields.LfNr].AsInt();
}