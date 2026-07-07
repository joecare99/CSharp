using System;

namespace AA98.MarkDown.UI.Controls;

/// <summary>
/// Carries the markdown link target extracted from the preview surface.
/// </summary>
public sealed class MarkdownLinkInvokedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownLinkInvokedEventArgs"/> class.
    /// </summary>
    /// <param name="linkTarget">The markdown link target.</param>
    public MarkdownLinkInvokedEventArgs(string linkTarget)
    {
        LinkTarget = linkTarget ?? throw new ArgumentNullException(nameof(linkTarget));
    }

    /// <summary>
    /// Gets the link target found in the markdown source.
    /// </summary>
    public string LinkTarget { get; }
}
