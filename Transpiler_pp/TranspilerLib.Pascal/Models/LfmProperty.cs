namespace TranspilerLib.Pascal.Models;

/// <summary>
/// Represents a property in an LFM object.
/// </summary>
public class LfmProperty
{
    public string Name { get; set; } = string.Empty;
    public object? Value { get; set; }
    public LfmPropertyType PropertyType { get; set; } = LfmPropertyType.Simple;

    public override string ToString()
    {
        return $"{Name} = {Value}";
    }
}

/// <summary>
/// The type of LFM property value.
/// </summary>
public enum LfmPropertyType
{
    /// <summary>Simple value (string, number, boolean, identifier)</summary>
    Simple,
    /// <summary>Set value like [biMinimize, biMaximize]</summary>
    Set,
    /// <summary>Binary data enclosed in { }</summary>
    Binary,
    /// <summary>Multi-line string continuation with +</summary>
    StringContinuation
}
