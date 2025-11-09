// Pseudocode/Plan:
// - Definiere Datenklasse `HejPlaceData` mit Properties: ID, PlaceName, ZIPCode, State, District, GOV, Country, PolName, Parish, County, ShortName, Longitude, Magnitude, MaidenheadLoc.
// - Erzeuge statische Klasse `PlaceTestData` mit `public static readonly HejPlaceData[] cPlace`, die die Pascal-Constants 1:1 als Objekte enthält.
// - Stelle sicher, dass leere Pascal-Strings als string.Empty abgebildet werden und Standardwerte in Properties initialisiert sind.
// - Nutze UTF-8 für Umlaute (z. B. "München", "Baden-Württemberg").
// - Keine externen Abhängigkeiten; kompatibel zu .NET Framework 4.6.2+ und .NET 6-9.

namespace WinAhnenCls.Data;

public class HejPlaceData
{
    public int ID { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public string ZIPCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string GOV { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PolName { get; set; } = string.Empty;
    public string Parish { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string Magnitude { get; set; } = string.Empty;
    public string MaidenheadLoc { get; set; } = string.Empty;
}
