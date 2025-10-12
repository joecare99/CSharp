using System.Collections.ObjectModel;

namespace GenFreeBrowser.Views.Models;

public class FamilyNode
{
    public required string Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Subtitle { get; set; }
    public int Generation { get; set; } // 0 = selected person's generation
    public int Order { get; set; } // order inside generation

    // layout (set by FamilyView)
    public double X { get; set; }
    public double Y { get; set; }
}

public class PartnerLink
{
    public required string A { get; set; }
    public required string B { get; set; }
}

public class ParentChildLink
{
    public required string ParentA { get; set; }
    public required string ParentB { get; set; }
    public required string Child { get; set; }
}

public class FamilyGraph
{
    public ObservableCollection<FamilyNode> Nodes { get; } = new();
    public ObservableCollection<PartnerLink> Partners { get; } = new();
    public ObservableCollection<ParentChildLink> ParentChild { get; } = new();
}
