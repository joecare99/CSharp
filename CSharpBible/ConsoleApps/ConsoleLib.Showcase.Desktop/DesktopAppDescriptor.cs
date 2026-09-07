using System.Drawing;

namespace ConsoleLib.Showcase.Desktop;

/// <summary>Static metadata needed to create one desktop application window.</summary>
public sealed record DesktopAppDescriptor(
    string Id,
    string Title,
    ShowcasePage Page,
    Point InitialPosition,
    Size InitialSize);
