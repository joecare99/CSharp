using System.Collections.Generic;

namespace RnzTrauer.Core.Services;

/// <summary>Processes normalized parser tokens against a RNZ schema line state machine.</summary>
public interface ISchemaFilter
{
    /// <summary>Gets whether subsequent parser output is currently enabled.</summary>
    bool FilterMode { get; }

    /// <summary>Gets the zero-based schema line currently being tested.</summary>
    int TestLine { get; }

    /// <summary>Replaces the schema and resets its line pointer.</summary>
    void SetSchema(IReadOnlyList<string> schema);

    /// <summary>Resets only the line pointer and output mode.</summary>
    void Reset();

    /// <summary>Tests one parser token and returns any legacy <c>+</c> emissions.</summary>
    IReadOnlyList<SchemaFilterEmission> Test(string token);
}
