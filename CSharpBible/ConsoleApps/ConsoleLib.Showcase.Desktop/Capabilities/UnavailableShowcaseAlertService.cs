namespace ConsoleLib.Showcase.Desktop.Capabilities;

/// <summary>Explicit no-op alert capability for hosts without alert output.</summary>
public sealed class UnavailableShowcaseAlertService : IShowcaseAlertService
{
    public bool IsAvailable => false;

    public void Alert()
    {
    }
}
