namespace Avln_CommonDialogs.Base.Interfaces;

public sealed record FileTypeFilter(string Name, IReadOnlyList<string> Patterns);
