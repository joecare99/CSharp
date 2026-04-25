namespace GenFreeBrowser;

public sealed class PersonenQuery
{
    public string? NameContains { get; init; }
    public int? BirthYearFrom { get; init; }
    public int? BirthYearTo   { get; init; }
    public int PageIndex { get; init; } = 0; // 0-basiert
    public int PageSize  { get; init; } = 50;
}
