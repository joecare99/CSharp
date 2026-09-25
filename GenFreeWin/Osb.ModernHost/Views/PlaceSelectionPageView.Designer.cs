#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class PlaceSelectionPageView
{
    [global::Views.TextBinding(nameof(PlaceSelectionViewModel.SearchText))]
    [global::Views.EnabledBinding(nameof(PlaceSelectionViewModel.IsSearchAvailable))]
    private TextBox _searchTextBox = null!;
    [global::Views.ListBinding(nameof(PlaceSelectionViewModel.SearchResults), "SelectedSearchResult")]
    private ListBox _searchResultsListBox = null!;
    [global::Views.ListBinding(nameof(PlaceSelectionViewModel.SelectedPlaces), "SelectedPlace")]
    private ListBox _selectedPlacesListBox = null!;
    [global::Views.TextBinding(nameof(PlaceSelectionViewModel.SelectionText))]
    private TextBox _selectionTextBox = null!;
    [global::Views.TextBinding(nameof(PlaceSelectionViewModel.StatusText))]
    private Label _statusLabel = null!;
    [global::Views.CommandBinding(nameof(PlaceSelectionViewModel.ApplySelectionTextCommand))]
    private Button _applySelectionButton = null!;
    [global::Views.CommandBinding(nameof(PlaceSelectionViewModel.ReturnToMenuCommand))]
    private Button _backButton = null!;
    private IContainer? components;

    private void InitializeComponent()
    {
        components = new Container();
        _searchTextBox = new TextBox();
        _searchResultsListBox = new ListBox();
        _selectedPlacesListBox = new ListBox();
        _selectionTextBox = new TextBox();
        _statusLabel = new Label();
        _applySelectionButton = new Button();
        _backButton = new Button();
        var searchLabel = new Label { AutoSize = true, Text = "Ort suchen (Datenbankanbindung folgt):" };
        var searchPanel = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.TopDown, Height = 88, WrapContents = false };
        searchPanel.Controls.Add(searchLabel);
        searchPanel.Controls.Add(_searchTextBox);
        _searchTextBox.Width = 300;
        _searchResultsListBox.Dock = DockStyle.Left;
        _searchResultsListBox.Font = new Font("Courier New", 11.25f);
        _searchResultsListBox.Width = 360;
        _selectedPlacesListBox.Dock = DockStyle.Fill;
        _selectedPlacesListBox.Font = new Font("Courier New", 11.25f);
        var listContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 8) };
        listContainer.Controls.Add(_selectedPlacesListBox);
        listContainer.Controls.Add(_searchResultsListBox);
        _selectionTextBox.AcceptsReturn = true;
        _selectionTextBox.Dock = DockStyle.Bottom;
        _selectionTextBox.Height = 84;
        _selectionTextBox.Multiline = true;
        _selectionTextBox.ScrollBars = ScrollBars.Vertical;
        _applySelectionButton.AutoSize = true;
        _applySelectionButton.Text = "Auswahltext übernehmen";
        _backButton.AutoSize = true;
        _backButton.Text = "Zurück zum Menü";
        var commandPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.LeftToRight, Height = 44 };
        commandPanel.Controls.Add(_applySelectionButton);
        commandPanel.Controls.Add(_backButton);
        _statusLabel.AutoSize = false;
        _statusLabel.Dock = DockStyle.Bottom;
        _statusLabel.ForeColor = Color.DarkRed;
        _statusLabel.Height = 34;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        Controls.Add(listContainer);
        Controls.Add(_selectionTextBox);
        Controls.Add(commandPanel);
        Controls.Add(_statusLabel);
        Controls.Add(searchPanel);
        Dock = DockStyle.Fill;
        Padding = new Padding(24);
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
