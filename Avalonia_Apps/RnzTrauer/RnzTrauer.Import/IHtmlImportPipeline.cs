using System;
using System.Collections.Generic;

namespace RnzTrauer.Import;

/// <summary>Runs one independent, UI-free RNZ HTML/schema import.</summary>
public interface IHtmlImportPipeline
{
    /// <summary>Imports raw HTML bytes without losing the original encoding boundary.</summary>
    HtmlImportReport Import(ReadOnlyMemory<byte> htmlBytes, IReadOnlyList<string> schema);
}
