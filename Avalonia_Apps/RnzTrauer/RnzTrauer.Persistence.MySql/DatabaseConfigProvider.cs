using System;
using Config.Service;

namespace RnzTrauer.Persistence.MySql;

/// <summary>
/// Provider for the Database configuration section. Registers itself with the config service.
/// </summary>
public sealed class DatabaseConfigProvider : IConfigSectionProvider
{
    public string Name => "Database";

    public string DisplayName => "Datenbank-Einstellungen";

    public string? Description => 
        "MySQL-Server, Port, Benutzer, Passwort und Datenbankname für Traueranzeige-Daten.";

    public int Order => 0;

    public Type ModelType => typeof(DatabaseConfig);

    public DatabaseConfig CreateModel() => new();

    object IConfigSectionProvider.CreateModel() => CreateModel();
}
