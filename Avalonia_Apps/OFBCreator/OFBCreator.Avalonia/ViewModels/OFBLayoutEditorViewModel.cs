using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using OFBCreator.Projects.Models;

namespace OFBCreator.Avalonia.ViewModels;

public sealed partial class OFBLayoutEditorViewModel : ObservableObject
{
    private static readonly OFBProjectLayout DefaultLayout = new();

    [ObservableProperty]
    private double _topMarginMm = 20;

    [ObservableProperty]
    private double _bottomMarginMm = 20;

    [ObservableProperty]
    private double _leftMarginMm = 20;

    [ObservableProperty]
    private double _rightMarginMm = 20;

    [ObservableProperty]
    private int _familyDirectoryColumns = 2;

    [ObservableProperty]
    private int _personIndexColumns = 3;

    [ObservableProperty]
    private int _occupationIndexColumns = 3;

    [ObservableProperty]
    private int _propertyIndexColumns = 3;

    [ObservableProperty]
    private int _hierarchicalPlaceIndexColumns = 3;

    [ObservableProperty]
    private int _alphabeticalPlaceIndexColumns = 3;

    public ObservableCollection<OFBParagraphStyleEditorViewModel> Styles { get; } =
    [
        new("FamilyEntry", "Familieneintrag", DefaultLayout.FamilyEntry),
        new("ParentEntry", "Elterneintrag", DefaultLayout.ParentEntry),
        new("ParentLifeEvents", "Lebensereignisse Eltern", DefaultLayout.ParentLifeEvents),
        new("ChildHeader", "Kind(er)-Überschrift", DefaultLayout.ChildHeader),
        new("ChildEntry", "Kindereintrag", DefaultLayout.ChildEntry),
        new("ChildLifeEvents", "Lebensereignisse Kinder", DefaultLayout.ChildLifeEvents),
        new("IndexEntry", "Indexeintrag", DefaultLayout.IndexEntry)
    ];

    public void Load(OFBProjectLayout layout)
    {
        TopMarginMm = layout.TopMarginMm;
        BottomMarginMm = layout.BottomMarginMm;
        LeftMarginMm = layout.LeftMarginMm;
        RightMarginMm = layout.RightMarginMm;
        FamilyDirectoryColumns = layout.FamilyDirectoryColumns;
        PersonIndexColumns = layout.PersonIndexColumns;
        OccupationIndexColumns = layout.OccupationIndexColumns;
        PropertyIndexColumns = layout.PropertyIndexColumns;
        HierarchicalPlaceIndexColumns = layout.HierarchicalPlaceIndexColumns;
        AlphabeticalPlaceIndexColumns = layout.AlphabeticalPlaceIndexColumns;
        foreach (var style in Styles)
            style.Load(GetStyle(layout, style.Key));
    }

    public OFBProjectLayout ToModel() => new()
    {
        TopMarginMm = TopMarginMm,
        BottomMarginMm = BottomMarginMm,
        LeftMarginMm = LeftMarginMm,
        RightMarginMm = RightMarginMm,
        FamilyDirectoryColumns = FamilyDirectoryColumns,
        PersonIndexColumns = PersonIndexColumns,
        OccupationIndexColumns = OccupationIndexColumns,
        PropertyIndexColumns = PropertyIndexColumns,
        HierarchicalPlaceIndexColumns = HierarchicalPlaceIndexColumns,
        AlphabeticalPlaceIndexColumns = AlphabeticalPlaceIndexColumns,
        FamilyEntry = GetEditor("FamilyEntry").ToModel(),
        ParentEntry = GetEditor("ParentEntry").ToModel(),
        ParentLifeEvents = GetEditor("ParentLifeEvents").ToModel(),
        ChildHeader = GetEditor("ChildHeader").ToModel(),
        ChildEntry = GetEditor("ChildEntry").ToModel(),
        ChildLifeEvents = GetEditor("ChildLifeEvents").ToModel(),
        IndexEntry = GetEditor("IndexEntry").ToModel()
    };

    private OFBParagraphStyleEditorViewModel GetEditor(string key) =>
        Styles.Single(style => style.Key == key);

    private static OFBParagraphStyle GetStyle(OFBProjectLayout layout, string key) => key switch
    {
        "FamilyEntry" => layout.FamilyEntry,
        "ParentEntry" => layout.ParentEntry,
        "ParentLifeEvents" => layout.ParentLifeEvents,
        "ChildHeader" => layout.ChildHeader,
        "ChildEntry" => layout.ChildEntry,
        "ChildLifeEvents" => layout.ChildLifeEvents,
        "IndexEntry" => layout.IndexEntry,
        _ => throw new System.ArgumentOutOfRangeException(nameof(key), key, "Unknown layout style.")
    };
}
