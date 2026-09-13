using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>One tab label and its optional content control.</summary>
public sealed class TabItem
{
    public TabItem(string header, IControl? content = null)
    {
        Header = header ?? throw new ArgumentNullException(nameof(header));
        Content = content;
    }

    public string Header { get; set; }
    public IControl? Content { get; }
}
