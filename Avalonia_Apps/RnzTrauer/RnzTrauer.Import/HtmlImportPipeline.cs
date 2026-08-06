using System;
using System.Collections.Generic;
using System.Linq;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Import;

/// <summary>
/// Component boundary around the tested Pascal-compatible Core import pipeline.
/// The component intentionally exposes data contracts instead of UI controls.
/// </summary>
public sealed class HtmlImportPipeline : IHtmlImportPipeline
{
    private readonly IHtmlSchemaImporter _importer;

    /// <summary>Creates a pipeline from the lower-level Core importer.</summary>
    public HtmlImportPipeline(IHtmlSchemaImporter importer)
    {
        _importer = importer ?? throw new ArgumentNullException(nameof(importer));
    }

    /// <inheritdoc />
    public HtmlImportReport Import(ReadOnlyMemory<byte> htmlBytes, IReadOnlyList<string> schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        var result = _importer.Import(htmlBytes, schema);
        return new HtmlImportReport(
            result.CompletedRows.Select(row => row.Columns.ToArray()).ToArray(),
            result.CurrentRow?.Columns.ToArray(),
            result.NewFiles.ToArray());
    }
}
