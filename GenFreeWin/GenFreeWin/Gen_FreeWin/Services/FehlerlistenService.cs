using Gen_FreeWin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services;

/// <summary>
/// Service für Fehllisten-Abfragen (EVA-Prinzip: Eingabe → Verarbeitung → Ausgabe).
/// Extrahiert Datenlogik aus dem Legacy-View und bietet wiederverwend- und testbare Schnittstellen.
/// </summary>
public interface IFehlerlistenService
{
    Task<ErrorListResult> GetPersonenOhneElternAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback);

    Task<ErrorListResult> GetPersonenErrorsAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback);

    Task<ErrorListResult> GetFamilienErrorsAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback);

    Task<ErrorListResult> GetOerterErrorsAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback);
}

/// <summary>
/// Standard-Implementierung mit Demo-Daten.
/// Im produktiven Einsatz werden echte DataModul-Zugriffe implementiert.
/// </summary>
public class FehlerlistenService : IFehlerlistenService
{
    private static readonly List<(int id, string name, string error)> DemoPersonen = new()
    {
        (1, "Müller, John", "Keine Eltern-Verbindung"),
        (42, "Schmidt, Maria", "Keine Eltern-Verbindung"),
        (99, "Neumann, Hans", "Keine Eltern-Verbindung"),
    };

    private static readonly List<(int id, string name, string error)> DemoFamilien = new()
    {
        (100, "Familie Müller (1850-1920)", "Keine Hochzeitsdatum"),
        (101, "Familie Schneider (1880-?)", "Keine Sterbdatum"),
    };

    private static readonly List<(int id, string name, string error)> DemoOerter = new()
    {
        (1001, "Geislingen", "Koordinaten fehlen"),
        (1002, "Böhlingen", "Koordinaten fehlen"),
    };

    public FehlerlistenService()
    {
        // Platzhalter für echte DI-Initialisierung
    }

    public async Task<ErrorListResult> GetPersonenOhneElternAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback)
    {
        var result = new ErrorListResult { Title = "Personen ohne Eltern (tote Punkte)" };

        try
        {
            await Task.Run(() =>
            {
                // EINGABE: DataModul-Aufruf für Personenzahl
                // TODO: Ersetze durch echtsystemon, wenn DataModul verfügbar
                // int maxPersonen = DataModul.Person.Count;
                // Für jetzt: Demo-Logik

                int maxPersonen = DemoPersonen.Count;
                progressCallback?.Invoke(0, maxPersonen);

                // VERARBEITUNG: Iteriere über Personen und prüfe auf fehlende Eltern
                List<ErrorListItem> fehloPersonen = new();

                for (int personId = 1; personId <= maxPersonen; personId++)
                {
                    // TODO: Echte Abfrage:
                    // if (DataModul.Person.Exists(personId))
                    // {
                    //     Modul1.PersInArb = personId;
                    //     Modul1.Person_ReadNames(personId, Modul1.Person);
                    //     // Check: Hat diese Person Vater UND Mutter?
                    //     if (Modul1.Family.Father == 0 && Modul1.Family.Mother == 0)  // Beide fehlend = "tote Punkt"
                    //     {
                    //         var displayText = $"{Modul1.Person.SurName}, {Modul1.Person.Givennames}";
                    //         itemAddCallback?.Invoke(new ErrorListItem(...));
                    //     }
                    // }

                    // Demo-Fallback während Entwicklung:
                    if (personId <= DemoPersonen.Count)
                    {
                        var (id, name, error) = DemoPersonen[personId - 1];
                        var item = new ErrorListItem
                        {
                            Id = id,
                            DisplayText = $"{name}  {id}",
                            AdditionalData = new()
                            {
                                { "ErrorType", "MissingParent" },
                                { "ErrorDescription", error },
                            }
                        };
                        itemAddCallback?.Invoke(item);
                    }

                    progressCallback?.Invoke(personId, maxPersonen);
                }

                result.IsSuccess = true;
            });
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            // Protokolliere Fehler zu Debug-Output für diagnostische Zwecke
            Debug.WriteLine($"FehlerlistenService.GetPersonenOhneElternAsync() Fehler: {ex.Message}", "FehlerlistenService");
        }

        return result;
    }

    public async Task<ErrorListResult> GetPersonenErrorsAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback)
    {
        return await ProcessErrorListAsync(
            "Fehlliste Personen (fehlende Daten)",
            DemoPersonen,
            progressCallback,
            itemAddCallback);
    }

    public async Task<ErrorListResult> GetFamilienErrorsAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback)
    {
        return await ProcessErrorListAsync(
            "Fehlliste Familien",
            DemoFamilien,
            progressCallback,
            itemAddCallback);
    }

    public async Task<ErrorListResult> GetOerterErrorsAsync(
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback)
    {
        return await ProcessErrorListAsync(
            "Fehlliste Orte",
            DemoOerter,
            progressCallback,
            itemAddCallback);
    }

    /// <summary>
    /// Zentrale Verarbeitungs-Methode für alle Fehllisten (EVA-Prinzip).
    /// </summary>
    private async Task<ErrorListResult> ProcessErrorListAsync(
        string title,
        List<(int id, string name, string error)> items,
        Action<int, int> progressCallback,
        Action<ErrorListItem> itemAddCallback)
    {
        var result = new ErrorListResult { Title = title };

        try
        {
            await Task.Run(() =>
            {
                progressCallback?.Invoke(0, items.Count);

                for (int i = 0; i < items.Count; i++)
                {
                    var (id, name, error) = items[i];
                    var item = new ErrorListItem
                    {
                        Id = id,
                        DisplayText = $"{name}  {id}",
                        AdditionalData = new()
                        {
                            { "ErrorType", DeriveErrorType(error) },
                            { "ErrorDescription", error },
                        }
                    };

                    itemAddCallback?.Invoke(item);
                    progressCallback?.Invoke(i + 1, items.Count);
                }
            });

            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            // Protokolliere Fehler zu Debug-Output für diagnostische Zwecke
            Debug.WriteLine($"FehlerlistenService.ProcessErrorListAsync('{title}') Fehler: {ex.Message}", "FehlerlistenService");
        }

        return result;
    }

    private static string DeriveErrorType(string errorDescription)
    {
        return errorDescription.Contains("Eltern", StringComparison.OrdinalIgnoreCase) ? "MissingParent"
             : errorDescription.Contains("Datum", StringComparison.OrdinalIgnoreCase) ? "MissingDate"
             : errorDescription.Contains("Koordinat", StringComparison.OrdinalIgnoreCase) ? "MissingCoordinates"
             : "Unknown";
    }
}
