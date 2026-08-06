using System.Collections.Generic;

namespace RnzTrauer.Persistence.MySql;

/// <summary>Parameterized SQL text prepared by the MySQL persistence adapter.</summary>
public sealed record SqlStatement(
    string CommandText,
    IReadOnlyDictionary<string, object?> Parameters);
