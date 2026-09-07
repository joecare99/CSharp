namespace ConsoleLib.Showcase.Desktop.Capabilities;

/// <summary>Optional host-provided alert output for alarms and status feedback.</summary>
public interface IShowcaseAlertService
{
    /// <summary>Whether the selected host can produce an audible or equivalent alert.</summary>
    bool IsAvailable { get; }

    /// <summary>Requests one host alert; unavailable hosts must expose <see cref="IsAvailable"/> as false.</summary>
    void Alert();
}
