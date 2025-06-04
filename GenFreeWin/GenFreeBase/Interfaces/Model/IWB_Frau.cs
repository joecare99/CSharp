using GenFree.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces.Model;

public interface IWB_Frau :
   IUsesRecordset<int>, // Interface for recordset access
   IUsesID<int> // Interface for ID access
{
    /// <summary>
    /// Fügt die Elternteile einer Familie als neue Datensätze in die WB_Frau-Tabelle ein.
    /// </summary>
    /// <param name="family">Die Familienpersonen, deren Eltern hinzugefügt werden sollen.</param>
    void AddParent(IFamilyData family);
    /// <summary>
    /// Fügt eine neue Zeile mit der angegebenen Personen-/Familiennummer in die WB_Frau-Tabelle ein.
    /// </summary>
    /// <param name="iPerFamNr">Die Personen- oder Familiennummer.</param>
    void AddRow(int iPerFamNr);
    /// <summary>
    /// Setzt das Feld "Nr" für alle Datensätze in der WB_Frau-Tabelle auf 0 zurück.
    /// </summary>
    void ClearNr();
    /// <summary>
    /// Setzt das Feld "Nr" für einen bestimmten Datensatz auf 1 oder fügt einen neuen Datensatz hinzu.
    /// </summary>
    /// <param name="iPers">Die Personen-ID.</param>
    void Commit(int iPers);
    /// <summary>
    /// Löscht alle Datensätze aus der WB_Frau-Tabelle, bei denen das Feld "Nr" gleich 0 ist.
    /// </summary>
    void DeleteEmpty();
    /// <summary>
    /// Führt eine Aktion für alle Datensätze in der WB_Frau-Tabelle aus und aktualisiert das Namensfeld.
    /// </summary>
    /// <param name="GetNameDate">Funktion, die Name und Datum für eine gegebene Personen-ID zurückgibt.</param>
    void ForAll(Func<int, (string, string)> GetNameDate);
    /// <summary>
    /// Setzt das Feld "NR" für die Eltern einer Familie auf 1, sofern vorhanden.
    /// </summary>
    /// <param name="family">Die Familiendaten.</param>
    void SetParentTo1(IFamilyData family);
    ///<summary>
    /// Aktualisiert das Feld "Nr" für einen bestimmten Datensatz auf 1, falls vorhanden.
    /// </summary>
    /// <param name="iPerNr">Die Personen-ID.</param>
    /// <returns>True, wenn der Datensatz gefunden und aktualisiert wurde, sonst false.</returns>
    bool Update(int iPerNr);
}
