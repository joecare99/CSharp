using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using AA40_Wizzard.Model;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Document.Axaml;

namespace AA40_Wizzard;

/// <summary>
/// Resolves localized texts and embedded assets for the wizard application.
/// </summary>
public sealed class WizardContentService : IWizardContentService
{
    private const string ResourceBaseName = "AA40_Wizzard.Properties.Resources";
    private const string AssetBasePath = "avares://AA40_Wizzard/Assets/Resources";
    private const string DocumentResourceBaseName = "AA40_Wizzard.Assets.Resources";

    private static readonly ResourceManager ResourceManager = new(ResourceBaseName, typeof(WizardContentService).Assembly);
    private static readonly FlowDocumentAxamlConverter DocumentConverter = new();

    /// <inheritdoc />
    public string GetText(string key)
        => ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? string.Empty;

    /// <inheritdoc />
    public IReadOnlyList<ListEntry> GetOptions(IEnumerable<int> optionIds, string resourcePrefix)
        => optionIds.Select(optionId => new ListEntry(optionId, GetText($"{resourcePrefix}{optionId}"))).ToList();

    /// <inheritdoc />
    public Control? GetDocumentPreview(int? selectionId)
    {
        if (selectionId is null || selectionId < 0)
        {
            return null;
        }

        var resourceName = $"MainSelection{selectionId}.xaml";
        var documentXaml = ReadEmbeddedDocument(resourceName);
        return string.IsNullOrWhiteSpace(documentXaml)
            ? null
            : DocumentConverter.CreatePreviewControl(documentXaml);
    }

    /// <inheritdoc />
    public Bitmap? GetImage(int? selectionId)
    {
        if (selectionId is null || selectionId < 0)
        {
            return null;
        }

        var resourceName = $"MainSelection{selectionId}.png";
        var uri = ResolveAssetUri(resourceName);
        if (uri is null)
        {
            return null;
        }

        using var stream = AssetLoader.Open(uri);
        return new Bitmap(stream);
    }

    private static Uri? ResolveAssetUri(string resourceName)
    {
        foreach (var candidate in GetCultureCandidates(resourceName))
        {
            var uri = new Uri(candidate, UriKind.Absolute);
            if (AssetLoader.Exists(uri))
            {
                return uri;
            }
        }

        return null;
    }

    private static string ReadEmbeddedDocument(string resourceName)
    {
        foreach (var candidate in GetDocumentResourceCandidates(resourceName))
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(candidate);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        return string.Empty;
    }

    private static IEnumerable<string> GetCultureCandidates(string resourceName)
    {
        var culture = CultureInfo.CurrentUICulture;
        if (!string.IsNullOrWhiteSpace(culture.Name))
        {
            yield return $"{AssetBasePath}/{culture.Name}/{resourceName}";
        }

        if (!string.IsNullOrWhiteSpace(culture.TwoLetterISOLanguageName) && !string.Equals(culture.Name, culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
        {
            yield return $"{AssetBasePath}/{culture.TwoLetterISOLanguageName}/{resourceName}";
        }

        yield return $"{AssetBasePath}/{resourceName}";
    }

    private static IEnumerable<string> GetDocumentResourceCandidates(string resourceName)
    {
        var culture = CultureInfo.CurrentUICulture;
        if (!string.IsNullOrWhiteSpace(culture.Name))
        {
            yield return $"{DocumentResourceBaseName}.{culture.Name}.{resourceName}";
        }

        if (!string.IsNullOrWhiteSpace(culture.TwoLetterISOLanguageName) && !string.Equals(culture.Name, culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
        {
            yield return $"{DocumentResourceBaseName}.{culture.TwoLetterISOLanguageName}.{resourceName}";
        }

        yield return $"{DocumentResourceBaseName}.{resourceName}";
    }
}
