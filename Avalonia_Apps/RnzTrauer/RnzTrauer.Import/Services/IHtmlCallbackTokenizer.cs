using System.Collections.Generic;

namespace RnzTrauer.Import.Services;

/// <summary>Incrementally tokenizes HTML using the callback vocabulary of <c>ThtmlParser</c>.</summary>
public interface IHtmlCallbackTokenizer
{
    /// <summary>Feeds a chunk and returns callbacks for complete constructs.</summary>
    IReadOnlyList<HtmlCallbackEvent> Feed(string chunk);

    /// <summary>Flushes trailing standard text at end of input.</summary>
    IReadOnlyList<HtmlCallbackEvent> Complete();

    /// <summary>Clears buffered partial input.</summary>
    void Reset();
}
