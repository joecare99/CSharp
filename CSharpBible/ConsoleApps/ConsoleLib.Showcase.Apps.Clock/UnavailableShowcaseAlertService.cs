using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.Clock;

internal sealed class UnavailableShowcaseAlertService : IShowcaseAlertService
{
    public bool IsAvailable => false;

    public void Alert()
    {
    }
}
