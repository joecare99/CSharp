using System;
using Config.Service;

namespace RnzTrauer.Places;

/// <summary>
/// Provider for the Places configuration section. Registers itself with the config service.
/// </summary>
public sealed class PlaceConfigProvider : IConfigSectionProvider
{
    public string Name => "Places";

    public string DisplayName => "Orte-Konfiguration";

    public string? Description => 
        "Geocoding-API-Schlüssel, Cache-Pfad und Timeout für Ortsdaten.";

    public int Order => 1;

    public Type ModelType => typeof(PlaceConfig);

    public PlaceConfig CreateModel() => new();

    object IConfigSectionProvider.CreateModel() => CreateModel();
}
