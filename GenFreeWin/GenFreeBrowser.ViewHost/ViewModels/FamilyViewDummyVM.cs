using System.Collections.ObjectModel;
using GenFreeBrowser.Views.Models;

namespace GenFreeBrowser.ViewHost.ViewModels;

public class FamilyViewDummyVM
{
    public FamilyGraph Graph { get; } = new();

    public FamilyViewDummyVM()
    {
        // Build a small graph around a selected person (id: P3)
        // Generation -1: parents
        Graph.Nodes.Add(new FamilyNode { Id = "P1", DisplayName = "Emil E Weist", Subtitle = "1840-1874", Generation = -1, Order = 0 });
        Graph.Nodes.Add(new FamilyNode { Id = "P2", DisplayName = "Pauline Talkne", Subtitle = "1837-1875", Generation = -1, Order = 1 });
        Graph.Partners.Add(new PartnerLink { A = "P1", B = "P2" });

        // 
        Graph.Nodes.Add(new FamilyNode { Id = "P5", DisplayName = "Paul Winter", Subtitle = "1840-1874", Generation = -1, Order = 3 });
        Graph.Nodes.Add(new FamilyNode { Id = "P6", DisplayName = "Pauline Talkne", Subtitle = "1837-1875", Generation = -1, Order = 4 });
        Graph.Partners.Add(new PartnerLink { A = "P5", B = "P6" });

        // Generation 0: selected + partner
        Graph.Nodes.Add(new FamilyNode { Id = "P3", DisplayName = "Wilhelm Weist", Subtitle = "1886-1910", Generation = 0, Order = 0 });
        Graph.Nodes.Add(new FamilyNode { Id = "P4", DisplayName = "Luise Winter", Subtitle = "1878-1951", Generation = 0, Order = 1 });
        Graph.Partners.Add(new PartnerLink { A = "P3", B = "P4" });

        // Parent->child link to selected
        Graph.ParentChild.Add(new ParentChildLink { ParentA = "P1", ParentB = "P2", Child = "P3" });
        Graph.ParentChild.Add(new ParentChildLink { ParentA = "P5", ParentB = "P6", Child = "P4" });

        // Generation +1: children
        for (int i = 0; i < 10; i++)
        {
            var id = $"C{i}";
            Graph.Nodes.Add(new FamilyNode { Id = id, DisplayName = $"Kind {i + 1}", Subtitle = $"19{10 + i}-19{80 + i}", Generation = 1, Order = i });
            Graph.ParentChild.Add(new ParentChildLink { ParentA = "P3", ParentB = "P4", Child = id });
        }
    }
}
