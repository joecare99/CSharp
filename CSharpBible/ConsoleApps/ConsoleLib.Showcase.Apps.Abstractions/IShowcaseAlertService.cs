namespace ConsoleLib.Showcase.Apps;

/// <summary>Optional host-provided alert output for showcase features.</summary>
public interface IShowcaseAlertService
{
    bool IsAvailable { get; }

    void Alert();
}
