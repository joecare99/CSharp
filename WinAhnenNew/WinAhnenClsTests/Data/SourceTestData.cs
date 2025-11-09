// Pseudocode (Plan):
// - Create namespace "WinAhnenCls" to mirror the Pascal unit context.
// - Define a data model class "HejSourData" representing the Pascal record "THejSourData":
//   - Properties: ID (int), Title, Abk, Ereignisse, Von, Bis, Standort, Publ, Rep, Bem, Bestand, Med (all string).
//   - Initialize all string properties with string.Empty to mimic Pascal's default ('') for unspecified fields.
// - Define a static class "SourceTestData" that exposes the Pascal typed constant "cSource":
//   - Implement as a public static readonly array "cSource" of "HejSourData" with 17 elements (indices 0..16).
//   - Populate each element by directly mapping the Pascal initializer values to C# object initializers.
//   - For entries where only ID was specified in Pascal, leave other fields as empty strings (default).

namespace WinAhnenCls.Data;

public static class SourceTestData
{
    public static readonly HejSourData[] cSource = new HejSourData[]
    {
        new HejSourData { ID = 0 },

        new HejSourData
        {
            ID = 1,
            Title = "hörensagen",
            Abk = "1",
            Ereignisse = "2",
            Von = "3",
            Bis = "4",
            Standort = "5",
            Publ = "6",
            Rep = "7",
            Bem = "8",
            Bestand = "9",
            Med = "10"
        },

        new HejSourData
        {
            ID = 2,
            Title = "Sterbeanzeige",
            Abk = "Strb.Anz.",
            Ereignisse = "Sterbefälle",
            Von = "2015",
            Bis = "2018",
            Standort = "Heidelberg",
            Publ = "RNZ",
            Rep = "Druckergasse 15",
            Bem = "Kann Online abgefragt werden",
            Bestand = "Nur die letzten 14 Tage",
            Med = "online"
        },

        new HejSourData { ID = 3, Title = "Friedhof Mosbach" },

        new HejSourData { ID = 4, Title = "2" },

        new HejSourData { ID = 5, Title = "Taufbuch" },

        new HejSourData { ID = 6, Title = "Geburtsurkunde" },

        new HejSourData { ID = 7, Title = "Rechnung" },

        new HejSourData { ID = 8, Title = "1" },

        new HejSourData { ID = 9, Title = "3" },

        new HejSourData { ID = 10, Title = "Quelle1" },

        new HejSourData { ID = 11, Title = "Quelle2" },

        new HejSourData { ID = 12, Title = "Quelle3" },

        new HejSourData
        {
            ID = 13,
            Title = "Ancestry.com",
            Abk = "Anc.",
            Ereignisse = "divers",
            Von = "1500",
            Bis = "1930",
            Standort = "Salt Lake City, Utah, USA",
            Publ = "",
            Rep = "",
            Bem = "ggf. Anmeldung",
            Bestand = "",
            Med = "online"
        },

        new HejSourData
        {
            ID = 14,
            Title = "Eigenes Wissen",
            Abk = "e.Ws.",
            Ereignisse = "diverse",
            Von = "1980",
            Bis = "2019",
            Standort = "BW.",
            Publ = "-",
            Rep = "Gehirn",
            Bem = "u.U. nicht objektiv",
            Bestand = "",
            Med = "verbal"
        },

        new HejSourData
        {
            ID = 15,
            Title = "OSB Meißenheim",
            Abk = "OSB-Mh.",
            Ereignisse = "G,H,T",
            Von = "1560",
            Bis = "1969",
            Standort = "Meißenheim",
            Publ = "A. Köbele",
            Rep = "-",
            Bem = "",
            Bestand = "",
            Med = "Buch"
        },

        new HejSourData
        {
            ID = 16,
            Title = "Test",
            Abk = "1",
            Ereignisse = "2",
            Von = "3",
            Bis = "4",
            Standort = "5",
            Publ = "6",
            Rep = "7",
            Bem = "8",
            Bestand = "9",
            Med = "10"
        }
    };
}