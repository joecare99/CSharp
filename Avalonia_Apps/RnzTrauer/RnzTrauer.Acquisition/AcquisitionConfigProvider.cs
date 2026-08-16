using System;
using Config.Service;

namespace RnzTrauer.Acquisition;

/// <summary>
/// Provider for the Acquisition configuration section. Registers itself with the config service.
/// </summary>
public sealed class AcquisitionConfigProvider : IConfigSectionProvider
{
    public string Name => "Acquisition";

    public string DisplayName => "Daten-Akquisitionseinstellungen";

    public string? Description => 
        "Limits, Verzögerungen und Timeout für Webseiten-Scraping.";

    public int Order => 2;

    public Type ModelType => typeof(AcquisitionConfig);

    public AcquisitionConfig CreateModel() => new();

    object IConfigSectionProvider.CreateModel() => CreateModel();
}
