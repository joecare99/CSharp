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
        _titleLabel = new Label();
        _descriptionLabel = new Label();
        SuspendLayout();
        // 
        // _titleLabel
        // 
        _titleLabel.Dock = DockStyle.Top;
        _titleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _titleLabel.Location = new Point(24, 24);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.Size = new Size(2379, 36);
        _titleLabel.TabIndex = 1;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Dock = DockStyle.Fill;
        _descriptionLabel.Location = new Point(24, 60);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Padding = new Padding(0, 16, 0, 0);
        _descriptionLabel.Size = new Size(2379, 1230);
        _descriptionLabel.TabIndex = 0;
        // 
        // HostPageView
        // 
        Controls.Add(_descriptionLabel);
        Controls.Add(_titleLabel);
        Name = "HostPageView";
        Padding = new Padding(24);
        Size = new Size(2427, 1314);
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
