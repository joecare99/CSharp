using System.Collections.Generic;

namespace RnzTrauer.Import;

/// <summary>Serializable result of one RNZ import pass.</summary>
public sealed record HtmlImportReport(
    IReadOnlyList<IReadOnlyList<string>> CompletedRows,
    IReadOnlyList<string>? CurrentRow,
    IReadOnlyList<string> NewFiles);
