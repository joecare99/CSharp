namespace Config.Service;

/// <summary>
/// Identifies which layer supplied a resolved configuration value.
/// </summary>
public enum ConfigValueSource
{
    ProductDefault,
    System,
    Group,
    User,
}
