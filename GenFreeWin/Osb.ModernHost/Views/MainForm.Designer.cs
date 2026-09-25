#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class MainForm
{
    [global::Views.TextBinding(nameof(ModernHostViewModel.ActivePageTitle))]
    private Label _pageTitleLabel = null!;

    [global::Views.TextBinding(nameof(ModernHostViewModel.StatusText))]
    private Label _tenantStatusLabel = null!;

    private Panel _pageHost = null!;
    private IContainer? components;

    private void InitializeComponent()
    {
        components = new Container();
        _pageTitleLabel = new Label();
        _tenantStatusLabel = new Label();
        _pageHost = new Panel();
        SuspendLayout();
        _pageTitleLabel.BackColor = Color.Red;
        _pageTitleLabel.Dock = DockStyle.Top;
        _pageTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _pageTitleLabel.ForeColor = Color.White;
        _pageTitleLabel.Padding = new Padding(12, 0, 12, 0);
        _pageTitleLabel.Size = new Size(1018, 32);
        _pageTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
        _tenantStatusLabel.Dock = DockStyle.Bottom;
        _tenantStatusLabel.Padding = new Padding(12, 4, 12, 4);
        _tenantStatusLabel.Size = new Size(1018, 48);
        _tenantStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _pageHost.BackColor = SystemColors.Control;
        _pageHost.Dock = DockStyle.Fill;
        Controls.Add(_pageHost);
        Controls.Add(_tenantStatusLabel);
        Controls.Add(_pageTitleLabel);
        ClientSize = new Size(1018, 725);
        MinimumSize = new Size(800, 560);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Ortsfamilienbuch";
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
