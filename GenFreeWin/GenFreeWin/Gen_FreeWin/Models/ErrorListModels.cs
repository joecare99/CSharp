using System.Collections.Generic;

namespace Gen_FreeWin.Models;

/// <summary>
/// Enumeration für Fehllisten-Typen.
/// </summary>
public enum ErrorListType
{
    None,
    Persons,
    Families,
    Places,
    LinkedPersons,
    PersonsWithoutParents,
    IncompleteFamilies,
    FamiliesWithoutDates,
    LivingPersons,
}

/// <summary>
/// Repräsentiert einen Fehllisten-Eintrag (Daten-Modell).
/// </summary>
public class ErrorListItem
{
    /// <summary>
    /// Text des Eintrags (für die Anzeige).
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// ID (Personen-, Familien-, oder Orts-Nr.).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Zusatzdaten als Key-Value Paare.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// Für Listbox benötigte Tag-Information.
    /// </summary>
    public object Tag { get; set; }
}

/// <summary>
/// Ergebnis nach dem Laden einer Fehlliste.
/// </summary>
public class ErrorListResult
{
    /// <summary>
    /// Haupttitel (Typ der Liste).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Untertitel / Beschreibung.
    /// </summary>
    public string Subtitle { get; set; } = string.Empty;

    /// <summary>
    /// Spalten-Kopfzeile(n).
    /// </summary>
    public string ColumnHeaders { get; set; } = string.Empty;

    /// <summary>
    /// Alle Elemente.
    /// </summary>
    public List<ErrorListItem> Items { get; set; } = new();

    /// <summary>
    /// Erfolg oder Fehler?
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Fehlermeldung, falls vorhanden.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
