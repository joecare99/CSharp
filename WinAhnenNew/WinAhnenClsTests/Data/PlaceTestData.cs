// Pseudocode/Plan:
// - Definiere Datenklasse `HejPlaceData` mit Properties: ID, PlaceName, ZIPCode, State, District, GOV, Country, PolName, Parish, County, ShortName, Longitude, Magnitude, MaidenheadLoc.
// - Erzeuge statische Klasse `PlaceTestData` mit `public static readonly HejPlaceData[] cPlace`, die die Pascal-Constants 1:1 als Objekte enthält.
// - Stelle sicher, dass leere Pascal-Strings als string.Empty abgebildet werden und Standardwerte in Properties initialisiert sind.
// - Nutze UTF-8 für Umlaute (z. B. "München", "Baden-Württemberg").
// - Keine externen Abhängigkeiten; kompatibel zu .NET Framework 4.6.2+ und .NET 6-9.

namespace WinAhnenCls.Data;

public static class PlaceTestData
{
    public static readonly HejPlaceData[] cPlace = new[]
    {
        new HejPlaceData { ID = 0 },

        new HejPlaceData
        {
            ID = 1,
            PlaceName = "Adelsheim",
            ZIPCode = string.Empty,
            State = string.Empty,
            District = string.Empty,
            GOV = string.Empty,
            Country = string.Empty,
            PolName = string.Empty,
            Parish = string.Empty,
            County = string.Empty,
            ShortName = string.Empty,
            Longitude = string.Empty,
            Magnitude = string.Empty,
            MaidenheadLoc = string.Empty
        },

        new HejPlaceData
        {
            ID = 2,
            PlaceName = "Binau",
            ZIPCode = "74862",
            State = "Deutschland",
            District = "Mosbach",
            GOV = "GOV",
            Country = "Baden-Württemberg",
            PolName = "Binau (Gemeinde)",
            Parish = "Neckargerach",
            County = "Neckar-Odenwald-Kreis",
            ShortName = "Binau",
            Longitude = "L90",
            Magnitude = "B21",
            MaidenheadLoc = "Maidenhead"
        },

        new HejPlaceData
        {
            ID = 3,
            PlaceName = "Eppingen"
        },

        new HejPlaceData
        {
            ID = 4,
            PlaceName = "Mosbach"
        },

        new HejPlaceData
        {
            ID = 5,
            PlaceName = "Neckarbischofsheim"
        },

        new HejPlaceData
        {
            ID = 6,
            PlaceName = "Nimmerland"
        },

        new HejPlaceData
        {
            ID = 7,
            PlaceName = "Sulzfeld"
        },

        new HejPlaceData
        {
            ID = 8,
            PlaceName = "Hamburg",
            ZIPCode = "22609",
            State = "Germany",
            Country = "Hamburg",
            County = "Hamburg"
        },

        new HejPlaceData
        {
            ID = 9,
            PlaceName = "Bremen",
            ZIPCode = "20000",
            State = "Germany"
        },

        new HejPlaceData
        {
            ID = 10,
            PlaceName = "München",
            ZIPCode = "30000",
            State = "Germany"
        },

        new HejPlaceData
        {
            ID = 11,
            PlaceName = "Berlin",
            ZIPCode = "80000",
            State = "Germany"
        }
    };
}