using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Apps.Gallery;

/// <summary>Loads the CXAML shell and augments it with controls not expressible by the minimal CXAML runtime.</summary>
public static class GalleryPage
{
    /// <summary>Loads the valid embedded CXAML shell and the programmatically configured collection controls.</summary>
    public static CxamlLoadResult Load(GalleryViewModel viewModel)
    {
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        var shell = LoadCxamlPage(viewModel);
        var page = (Page)shell.Root;
        var named = new Dictionary<string, IControl>(shell.NamedControls, StringComparer.Ordinal);

        Add(page, named, "ControlsPanel", new Panel
        {
            Dimension = new Rectangle(1, 4, 37, 10),
            BorderStyle = BorderStyle.Single,
            BorderColor = ConsoleColor.DarkGray
        });
        Add(page, named, "InputCaption", Label("Input controls", 2, 4, 20));
        Add(page, named, "SampleLabel", Label("Label: descriptive text", 2, 5, 20));
        Add(page, named, "ResetButton", new Button
        {
            Text = "Reset",
            Dimension = new Rectangle(24, 5, 10, 1),
            Command = viewModel.ResetCommand
        });
        Add(page, named, "SampleCheckBox", new CheckBox
        {
            Text = "CheckBox: enabled",
            IsChecked = true,
            Dimension = new Rectangle(2, 6, 22, 1)
        });

        var compact = new RadioButton { Text = "Compact", Dimension = new Rectangle(2, 7, 12, 1) };
        var spacious = new RadioButton { Text = "Spacious", Dimension = new Rectangle(16, 7, 13, 1) };
        Add(page, named, "CompactRadioButton", compact);
        Add(page, named, "SpaciousRadioButton", spacious);
        compact.Select();

        var combo = new ComboBox { Dimension = new Rectangle(2, 8, 18, 1) };
        combo.Items.Add("ComboBox: Blue");
        combo.Items.Add("ComboBox: Green");
        combo.SelectNext();
        Add(page, named, "ThemeComboBox", combo);

        Add(page, named, "SampleProgressBar", new ProgressBar
        {
            Dimension = new Rectangle(22, 8, 12, 1),
            Minimum = 0,
            Maximum = 100,
            Value = viewModel.Progress,
            ForeColor = ConsoleColor.Green
        });
        Add(page, named, "SampleTextBox", new TextBox
        {
            Text = "TextBox: editable input",
            MultiLine = false,
            Dimension = new Rectangle(2, 10, 32, 1)
        });

        Add(page, named, "CollectionsPanel", new Panel
        {
            Dimension = new Rectangle(40, 4, 39, 10),
            BorderStyle = BorderStyle.Single,
            BorderColor = ConsoleColor.DarkGray
        });
        Add(page, named, "CollectionCaption", Label("Collections and navigation", 41, 4, 28));

        var list = new ListBox { Dimension = new Rectangle(41, 5, 11, 6) };
        list.ItemsSource = new ArrayList { "ListBox", "Alpha", "Beta", "Gamma", "Delta", "Epsilon" };
        list.SelectedIndex = 0;
        Add(page, named, "SampleListBox", list);

        var tree = new TreeView { Dimension = new Rectangle(53, 5, 11, 6) };
        var root = new TreeNode("TreeView") { IsExpanded = true };
        root.Add(new TreeNode("Branch A"));
        root.Add(new TreeNode("Branch B"));
        tree.Nodes.Add(root);
        Add(page, named, "SampleTreeView", tree);

        var tiles = new TileView { Dimension = new Rectangle(65, 5, 12, 6), TileWidth = 6, TileHeight = 2 };
        tiles.SetItems(new[] { new TileItem("Tile A"), new TileItem("Tile B"), new TileItem("Tile C") });
        Add(page, named, "SampleTileView", tiles);

        var tabs = new TabControl { Dimension = new Rectangle(41, 12, 36, 1) };
        tabs.Items.Add(new TabItem("Inputs"));
        tabs.Items.Add(new TabItem("Collections"));
        tabs.Items.Add(new TabItem("Layouts"));
        tabs.SelectNext();
        Add(page, named, "SampleTabControl", tabs);

        Add(page, named, "SampleScrollBar", new ScrollBar
        {
            Vertical = true,
            Minimum = 0,
            Maximum = 10,
            Value = 4,
            LargeChange = 3,
            Dimension = new Rectangle(77, 5, 1, 6)
        });

        Add(page, named, "ScrollViewerCaption", Label("ScrollViewer", 2, 15, 14));
        var scrollContent = new Panel { Dimension = new Rectangle(0, 0, 33, 4) };
        scrollContent.Add(new Label { Text = "Scrollable content is wider than", Dimension = new Rectangle(0, 0, 33, 1) });
        scrollContent.Add(new Label { Text = "this viewport.", Dimension = new Rectangle(0, 1, 20, 1) });
        var scrollViewer = new ScrollViewer { Dimension = new Rectangle(2, 16, 25, 3) };
        scrollViewer.SetContent(scrollContent);
        Add(page, named, "SampleScrollViewer", scrollViewer);

        AddLayoutSamples(page, named);
        AddMenu(page, named);

        var status = new StatusBar
        {
            Status = viewModel.Status,
            StatusColor = ConsoleColor.Cyan,
            Dimension = new Rectangle(1, 26, 78, 1)
        };
        Add(page, named, "SampleStatusBar", status);
        BindViewModel(viewModel, named["SampleProgressBar"], status);

        return new CxamlLoadResult(page, named);
    }

    /// <summary>Loads only the embedded, parser-compatible CXAML page shell.</summary>
    public static CxamlLoadResult LoadCxamlPage(GalleryViewModel viewModel)
    {
        if (viewModel is null)
            throw new ArgumentNullException(nameof(viewModel));

        var assembly = typeof(GalleryPage).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith(".Gallery.cxaml", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The gallery CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().LoadPage(reader, new CxamlLoadContext(viewModel));
    }

    private static void AddLayoutSamples(Page page, IDictionary<string, IControl> named)
    {
        Add(page, named, "LayoutsPanel", new Panel
        {
            Dimension = new Rectangle(28, 15, 51, 10),
            BorderStyle = BorderStyle.Single,
            BorderColor = ConsoleColor.DarkGray
        });
        Add(page, named, "LayoutsCaption", Label("Layout containers", 29, 15, 20));

        var stack = new StackPanel { Text = "StackPanel: vertical", Dimension = new Rectangle(29, 17, 22, 1) };
        stack.Add(new Label { Text = "First stack item", Visible = false, Dimension = new Rectangle(0, 0, 16, 1) });
        stack.Add(new Label { Text = "Second stack item", Visible = false, Dimension = new Rectangle(0, 0, 16, 1) });
        Add(page, named, "SampleStackPanel", stack);

        var grid = new Grid { Text = "Grid: two columns", Dimension = new Rectangle(29, 19, 22, 1) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        var gridCell = new Label { Text = "cell", Visible = false, Dimension = new Rectangle(0, 0, 4, 1) };
        Grid.SetColumn(gridCell, 1);
        grid.Add(gridCell);
        Add(page, named, "SampleGrid", grid);

        var dock = new DockPanel { Text = "DockPanel: fill", Dimension = new Rectangle(29, 21, 22, 1) };
        var docked = new Label { Text = "dock", Visible = false, Dimension = new Rectangle(0, 0, 4, 1) };
        DockPanel.SetDock(docked, Dock.Left);
        dock.Add(docked);
        Add(page, named, "SampleDockPanel", dock);

        Add(page, named, "LayoutNote", Label("Panel frames each gallery group.", 53, 17, 24));
        Add(page, named, "LayoutHint", Label("Tab, tile and tree keyboard navigation.", 53, 19, 24));
        Add(page, named, "LayoutProgress", Label("ProgressBar and StatusBar reflect actions.", 53, 21, 24));
    }

    private static void AddMenu(Page page, IDictionary<string, IControl> named)
    {
        var menu = new MenuBar { Dimension = new Rectangle(1, 1, 78, 1) };
        Add(page, named, "SampleMenuBar", menu);

        var viewPopup = new MenuPopup();
        viewPopup.AddItem(new MenuItem { Text = "Refresh" });
        viewPopup.AddItem(new MenuItem { Text = "Reset" });
        menu.AddRootItem(new MenuItem { Text = "&View" }, viewPopup);
        named.Add("SampleMenuPopup", viewPopup);

        menu.AddRootItem(new MenuItem { Text = "&Help" });
    }

    private static void BindViewModel(GalleryViewModel viewModel, IControl progressControl, StatusBar status)
    {
        viewModel.PropertyChanged += (_, eventArgs) =>
        {
            if (eventArgs.PropertyName == nameof(GalleryViewModel.Progress) && progressControl is ProgressBar progress)
                progress.Value = viewModel.Progress;
            if (eventArgs.PropertyName == nameof(GalleryViewModel.Status))
                status.Status = viewModel.Status;
        };
    }

    private static Label Label(string text, int x, int y, int width) =>
        new() { Text = text, Dimension = new Rectangle(x, y, width, 1) };

    private static void Add(Page page, IDictionary<string, IControl> named, string name, IControl control)
    {
        page.Add(control);
        named.Add(name, control);
    }
}
