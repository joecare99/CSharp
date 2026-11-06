namespace Config.Service;

/// <summary>
/// Represents a value explicitly present in a configuration layer.
/// A present null value is distinct from an absent key.
/// </summary>
public sealed record ConfigLayerValue(object? Value, bool IsEnforced = false);
