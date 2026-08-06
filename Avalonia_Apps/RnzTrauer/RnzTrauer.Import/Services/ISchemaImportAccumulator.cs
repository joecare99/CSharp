using System.Collections.Generic;

namespace RnzTrauer.Import.Services;

/// <summary>Maps legacy filter callback modes into import rows and media events.</summary>
public interface ISchemaImportAccumulator
{
    /// <summary>Completed rows in arrival order.</summary>
    IReadOnlyList<SchemaImportRow> CompletedRows { get; }

    /// <summary>Current partially collected row, if an <c>A</c> mode is active.</summary>
    SchemaImportRow? CurrentRow { get; }

    /// <summary>New source filenames raised by legacy <c>N</c> mode.</summary>
    IReadOnlyList<string> NewFiles { get; }

    /// <summary>Clears all rows, media events, and the current partial row.</summary>
    void Reset();

    /// <summary>Processes one legacy callback type and text payload.</summary>
    void Process(byte callbackType, string text);
}
