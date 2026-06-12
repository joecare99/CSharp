using System.Collections.Generic;
using AA40_Wizzard.Model;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace AA40_Wizzard;

/// <summary>
/// Provides localized texts and asset-backed content for the wizard UI.
/// </summary>
public interface IWizardContentService
{
    /// <summary>
    /// Gets a localized text for the specified resource key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized text or an empty string.</returns>
    string GetText(string key);

    /// <summary>
    /// Creates localized list entries for the specified identifiers.
    /// </summary>
    /// <param name="optionIds">The option identifiers.</param>
    /// <param name="resourcePrefix">The resource key prefix.</param>
    /// <returns>The localized list entries.</returns>
    IReadOnlyList<ListEntry> GetOptions(IEnumerable<int> optionIds, string resourcePrefix);

    /// <summary>
    /// Gets the localized document preview control for the selected main option.
    /// </summary>
    /// <param name="selectionId">The selected main option identifier.</param>
    /// <returns>The preview control or <see langword="null"/>.</returns>
    Control? GetDocumentPreview(int? selectionId);

    /// <summary>
    /// Gets the localized image for the selected main option.
    /// </summary>
    /// <param name="selectionId">The selected main option identifier.</param>
    /// <returns>The loaded bitmap or <see langword="null"/>.</returns>
    Bitmap? GetImage(int? selectionId);
}
