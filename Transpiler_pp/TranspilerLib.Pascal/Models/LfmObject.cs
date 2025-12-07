using System.Collections.Generic;

namespace TranspilerLib.Pascal.Models;

public class LfmObject
{
    public string Name { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public bool IsInherited { get; set; }
    public List<LfmProperty> Properties { get; } = [];
    public List<LfmObject> Children { get; } = [];

    public override string ToString()
    {
        var keyword = IsInherited ? "inherited" : "object";
        return $"{keyword} {Name}: {TypeName} (Properties: {Properties.Count}, Children: {Children.Count})";
    }
}
