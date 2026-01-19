namespace Avln_CommonDialogs.Base.Interfaces;

public interface IPrintPageCanvas
{
    double Width { get; }
    double Height { get; }

    /// <summary>
    /// Underlying platform-specific drawing target.
    /// Implementations may expose an Avalonia DrawingContext, SKCanvas, etc.
    /// </summary>
    object Target { get; }
}
