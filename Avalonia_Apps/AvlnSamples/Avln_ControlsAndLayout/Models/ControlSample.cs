namespace Avln_ControlsAndLayout.Models;

/// <summary>
/// Describes one controls or layout sample with display metadata and editable source text.
/// </summary>
/// <param name="Title">The title displayed in the sample list.</param>
/// <param name="Description">The description displayed above the preview.</param>
/// <param name="AxamlText">The Avalonia XAML representation displayed in the code editor.</param>
public sealed record ControlSample(
    string Title,
    string Description,
    string AxamlText);
