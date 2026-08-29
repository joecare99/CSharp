using System;
using System.Collections.Generic;
using Avalonia.Controls;
using ConsoleLib.Interfaces;
using AControl = Avalonia.Controls.Control;

namespace ConsoleLib.Cxaml.Designer.Preview;

public sealed class PreviewControlMapping
{
    internal PreviewControlMapping(
        string id,
        IReadOnlyList<int> sourcePath,
        IControl consoleControl,
        AControl previewControl)
    {
        Id = id;
        SourcePath = sourcePath;
        ConsoleControl = consoleControl;
        PreviewControl = previewControl;
    }

    public string Id { get; }
    public string ControlId => Id;
    public string SourceElementPath => Id;
    public IReadOnlyList<int> SourcePath { get; }
    public IControl ConsoleControl { get; }
    public AControl PreviewControl { get; }
    public string? SourceName { get; internal set; }
    public string ElementName => SourceName is null
        ? ConsoleControl.GetType().Name
        : ConsoleControl.GetType().Name + " (" + SourceName + ")";
}
