namespace Config.Service;

/// <summary>
/// A typed configuration value together with its effective source and policy state.
/// </summary>
public sealed record ConfigValueResolution(
    object? Value,
    ConfigValueSource Source,
    bool IsEnforced);
