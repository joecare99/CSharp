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

public sealed class HejSourData
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Abk { get; set; } = string.Empty;
    public string Ereignisse { get; set; } = string.Empty;
    public string Von { get; set; } = string.Empty;
    public string Bis { get; set; } = string.Empty;
    public string Standort { get; set; } = string.Empty;
    public string Publ { get; set; } = string.Empty;
    public string Rep { get; set; } = string.Empty;
    public string Bem { get; set; } = string.Empty;
    public string Bestand { get; set; } = string.Empty;
    public string Med { get; set; } = string.Empty;
}
