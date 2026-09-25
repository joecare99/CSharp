#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class PublicationSettingsPageView
{
    [global::Views.TextBinding(nameof(PublicationSettingsViewModel.ProfileName))]
    private TextBox _profileNameTextBox = null!;
    [global::Views.TextBinding(nameof(PublicationSettingsViewModel.TitleTemplate))]
    private TextBox _titleTemplateTextBox = null!;
    [global::Views.TextBinding(nameof(PublicationSettingsViewModel.FooterTemplate))]
    private TextBox _footerTemplateTextBox = null!;
    [global::Views.CheckedBinding(nameof(PublicationSettingsViewModel.IncludeSelectedPlaces))]
    private CheckBox _includePlacesCheckBox = null!;
    [global::Views.TextBinding(nameof(PublicationSettingsViewModel.StatusText))]
    private Label _statusLabel = null!;
    [global::Views.CommandBinding(nameof(PublicationSettingsViewModel.ValidateProfileCommand))]
    private Button _validateButton = null!;
    [global::Views.CommandBinding(nameof(PublicationSettingsViewModel.ReturnToMenuCommand))]
    private Button _backButton = null!;
    private IContainer? components;

    private void InitializeComponent()
    {
        components = new Container();
        _profileNameTextBox = new TextBox();
        _titleTemplateTextBox = new TextBox();
        _footerTemplateTextBox = new TextBox();
        _includePlacesCheckBox = new CheckBox();
        _statusLabel = new Label();
        _validateButton = new Button();
        _backButton = new Button();
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var profilePage = new TabPage("Allgemein");
        AddField(profilePage, "Profilname", _profileNameTextBox, 16);
        AddField(profilePage, "Titelvorlage", _titleTemplateTextBox, 72);
        AddField(profilePage, "Fußzeile", _footerTemplateTextBox, 128);
        _includePlacesCheckBox.Location = new Point(16, 184);
        _includePlacesCheckBox.AutoSize = true;
        _includePlacesCheckBox.Text = "Ausgewählte Orte einschließen";
        profilePage.Controls.Add(_includePlacesCheckBox);
        _validateButton.Location = new Point(16, 220);
        _validateButton.Text = "Profil prüfen";
        profilePage.Controls.Add(_validateButton);
        foreach (var title in new[] { "Personen", "Familien", "Ausgabe", "Register", "Dateien", "Bilder" })
        {
            var page = new TabPage(title);
            page.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "Die visuelle Optionsgruppe wird vorbereitet.", TextAlign = ContentAlignment.MiddleCenter });
            tabs.TabPages.Add(page);
        }
        tabs.TabPages.Insert(0, profilePage);
        _statusLabel.Dock = DockStyle.Bottom;
        _statusLabel.Height = 38;
        _backButton.Dock = DockStyle.Bottom;
        _backButton.Text = "Zurück zum Menü";
        Controls.Add(tabs);
        Controls.Add(_statusLabel);
        Controls.Add(_backButton);
        Dock = DockStyle.Fill;
        Padding = new Padding(12);
        ResumeLayout(false);
    }

    private static void AddField(Control parent, string labelText, TextBox textBox, int top)
    {
        parent.Controls.Add(new Label { AutoSize = true, Location = new Point(16, top), Text = labelText });
        textBox.Location = new Point(160, top - 4);
        textBox.Width = 300;
        parent.Controls.Add(textBox);
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
