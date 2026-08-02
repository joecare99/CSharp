using System.Collections.Generic;

namespace RnzTrauer.Core.Services;

/// <summary>Result of one schema-driven HTML import pass.</summary>
public sealed record HtmlSchemaImportResult(
    IReadOnlyList<SchemaImportRow> CompletedRows,
    SchemaImportRow? CurrentRow,
    IReadOnlyList<string> NewFiles);
