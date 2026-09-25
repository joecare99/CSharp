#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class OFBMenuPageView
{
    [global::Views.TextBinding(nameof(OFBMenuViewModel.Title))]
    private Label _applicationNameLabel = null!;

    [global::Views.TextBinding(nameof(OFBMenuViewModel.Description))]
    private Label _descriptionLabel = null!;

    [global::Views.CommandBinding(nameof(OFBMenuViewModel.OpenFamilyPublicationCommand))]
    private Button _continueButton = null!;

    [global::Views.CommandBinding(nameof(OFBMenuViewModel.ContinueFamilyPublicationCommand))]
    private Button _createBookButton = null!;

    [global::Views.CommandBinding(nameof(OFBMenuViewModel.OpenPublicationSettingsCommand))]
    private Button _printMenuButton = null!;

    [global::Views.CommandBinding(nameof(OFBMenuViewModel.OpenPlaceSelectionCommand))]
    private Button _placeSelectionButton = null!;

    [global::Views.CommandBinding(nameof(OFBMenuViewModel.OpenStatisticsCommand))]
    private Button _statisticsButton = null!;

    private IContainer? components;

    private void InitializeComponent()
    {
        components = new Container();
        _applicationNameLabel = new Label();
        _descriptionLabel = new Label();
        _continueButton = new Button();
        _createBookButton = new Button();
        _printMenuButton = new Button();
        _placeSelectionButton = new Button();
        _statisticsButton = new Button();
        var headerPanel = new Panel { Dock = DockStyle.Top, Height = 84 };
        var productLabel = CreateHeaderLabel("GEN_FREEWIN das freie Genealogie-Programm", 28);
        var copyrightLabel = CreateHeaderLabel("(c) 1994-2026 Joe Care · Germany", 56);
        ConfigureHeader(_applicationNameLabel, Color.Red, Color.White, FontStyle.Bold, 0);
        headerPanel.Controls.Add(copyrightLabel);
        headerPanel.Controls.Add(productLabel);
        headerPanel.Controls.Add(_applicationNameLabel);
        var actionPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            Height = 286,
            Padding = new Padding(0, 18, 0, 0),
            WrapContents = false
        };
        ConfigureActionButton(_continueButton, "letztes Ortsfamilienbuch weiterbearbeiten.");
        ConfigureActionButton(_createBookButton, "&Ortsfamilienbuch erstellen");
        ConfigureActionButton(_printMenuButton, "&Druckmenue / Einstellungen");
        ConfigureActionButton(_placeSelectionButton, "Ortsauswahl");
        ConfigureActionButton(_statisticsButton, "Statistik");
        actionPanel.Controls.Add(_continueButton);
        actionPanel.Controls.Add(_createBookButton);
        actionPanel.Controls.Add(_printMenuButton);
        actionPanel.Controls.Add(_placeSelectionButton);
        actionPanel.Controls.Add(_statisticsButton);
        _descriptionLabel.AutoSize = false;
        _descriptionLabel.Dock = DockStyle.Top;
        _descriptionLabel.Height = 40;
        _descriptionLabel.Padding = new Padding(0, 8, 0, 0);
        _descriptionLabel.TextAlign = ContentAlignment.MiddleLeft;
        var updatePanel = new Panel
        {
            BackColor = Color.FromArgb(255, 192, 192),
            Dock = DockStyle.Bottom,
            Height = 96,
            Padding = new Padding(12)
        };
        updatePanel.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "Aktualitätskontrolle\r\nDie automatische Update-Prüfung ist im modernen Host noch nicht angebunden.",
            TextAlign = ContentAlignment.MiddleCenter
        });
        Controls.Add(updatePanel);
        Controls.Add(actionPanel);
        Controls.Add(_descriptionLabel);
        Controls.Add(headerPanel);
        Dock = DockStyle.Fill;
        Padding = new Padding(50, 12, 50, 24);
        ResumeLayout(false);
    }
}
