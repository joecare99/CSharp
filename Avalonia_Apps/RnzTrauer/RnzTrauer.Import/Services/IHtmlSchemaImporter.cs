using System;
using System.Collections.Generic;

namespace RnzTrauer.Import.Services;

/// <summary>Runs the RNZ HTML token, schema filter, and row accumulation pipeline.</summary>
public interface IHtmlSchemaImporter
{
    /// <summary>Imports one HTML document using the supplied legacy schema lines.</summary>
    HtmlSchemaImportResult Import(string html, IReadOnlyList<string> schema);

    /// <summary>Decodes and imports one byte-oriented HTML document.</summary>
    HtmlSchemaImportResult Import(ReadOnlyMemory<byte> bytes, IReadOnlyList<string> schema);
}
