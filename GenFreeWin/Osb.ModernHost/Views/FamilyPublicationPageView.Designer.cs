#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class FamilyPublicationPageView
{
    [global::Views.TextBinding(nameof(FamilyPublicationViewModel.StatusText))]
    private Label _statusLabel = null!;
    [global::Views.CommandBinding(nameof(FamilyPublicationViewModel.CreatePublicationCommand))]
    private Button _createButton = null!;
    [global::Views.CommandBinding(nameof(FamilyPublicationViewModel.ReturnToMenuCommand))]
    private Button _backButton = null!;
    private IContainer? components;

    private void InitializeComponent()
    {
        components = new Container();
        _statusLabel = new Label();
        _createButton = new Button();
        _backButton = new Button();
        var options = new GroupBox { Dock = DockStyle.Left, Text = "Auswahl und Ausgabeoptionen", Width = 300 };
        options.Controls.Add(new CheckedListBox { Dock = DockStyle.Fill, Items = { "Personen auswählen", "Familien auswählen", "Register erzeugen", "Quellen ausgeben", "Medien ausgeben" } });
        var preview = new GroupBox { Dock = DockStyle.Fill, Text = "Vorschau" };
        preview.Controls.Add(new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, Text = "Die Vorschau wird nach Anbindung der charakterisierten Dokumenterzeugung angezeigt." });
        _statusLabel.Dock = DockStyle.Bottom;
        _statusLabel.Height = 42;
        _createButton.Dock = DockStyle.Bottom;
        _createButton.Text = "Ortsfamilienbuch erstellen";
        _backButton.Dock = DockStyle.Bottom;
        _backButton.Text = "Zurück zum Menü";
        Controls.Add(preview);
        Controls.Add(options);
        Controls.Add(_statusLabel);
        Controls.Add(_createButton);
        Controls.Add(_backButton);
        Dock = DockStyle.Fill;
        Padding = new Padding(12);
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
