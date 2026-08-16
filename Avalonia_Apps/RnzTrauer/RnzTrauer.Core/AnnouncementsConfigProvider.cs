using System;
using Config.Service;

namespace RnzTrauer.Core;

/// <summary>
/// Provider for the Announcements configuration section. Registers itself with the config service.
/// </summary>
public sealed class AnnouncementsConfigProvider : IConfigSectionProvider
{
    public string Name => "Announcements";

    public string DisplayName => "Anzeigen-Kernfunktionen";

    public string? Description => 
        "Import/Export/Media-Pfade und automatische Exporte für Anzeigen.";

    public int Order => 3;

    public Type ModelType => typeof(AnnouncementsConfig);

    public AnnouncementsConfig CreateModel() => new();

    object IConfigSectionProvider.CreateModel() => CreateModel();
}
