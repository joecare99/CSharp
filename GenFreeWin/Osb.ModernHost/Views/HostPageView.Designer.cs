#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class HostPageView
{
    [global::Views.TextBinding(nameof(IHostPageViewModel.Title))]
    private Label _titleLabel = null!;
    [global::Views.TextBinding(nameof(IHostPageViewModel.Description))]
    private Label _descriptionLabel = null!;
    private IContainer? components;

    private void InitializeComponent()
    {
        components = new Container();
        _titleLabel = new Label();
        _descriptionLabel = new Label();
        _titleLabel.AutoSize = false;
        _titleLabel.Dock = DockStyle.Top;
        _titleLabel.Font = new Font(Font, FontStyle.Bold);
        _titleLabel.Height = 36;
        _descriptionLabel.AutoSize = false;
        _descriptionLabel.Dock = DockStyle.Fill;
        _descriptionLabel.Padding = new Padding(0, 16, 0, 0);
        Controls.Add(_descriptionLabel);
        Controls.Add(_titleLabel);
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
