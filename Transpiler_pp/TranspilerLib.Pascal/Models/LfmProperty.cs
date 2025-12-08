using System.Collections.Generic;

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
/// Represents an item in an LFM item list (e.g., Panels = &lt;item...end&gt;).
/// </summary>
public class LfmItem
{
    /// <summary>
    /// Properties of this item.
    /// </summary>
    public List<LfmProperty> Properties { get; } = new();

    public override string ToString()
    {
        return $"item ({Properties.Count} properties)";
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
    StringContinuation,
    /// <summary>Item list enclosed in &lt; &gt; with item...end blocks</summary>
    ItemList,
    /// <summary>Collection enclosed in ( )</summary>
    Collection
}
