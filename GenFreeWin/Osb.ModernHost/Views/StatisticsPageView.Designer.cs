#nullable enable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class StatisticsPageView
{
    [global::Views.TextBinding(nameof(StatisticsViewModel.Title))]
    private Label _titleLabel = null!;
    [global::Views.TextBinding(nameof(StatisticsViewModel.Description))]
    private Label _descriptionLabel = null!;
    [global::Views.TextBinding(nameof(StatisticsViewModel.SampleNotice))]
    private Label _sampleNoticeLabel = null!;
    [global::Views.TextBinding(nameof(StatisticsViewModel.StatusText))]
    private Label _statusLabel = null!;
    [global::Views.ListBinding(nameof(StatisticsViewModel.Buckets), "SelectedBucket")]
    private ListBox _bucketList = null!;
    [global::Views.CommandBinding(nameof(StatisticsViewModel.RefreshSampleCommand))]
    private Button _refreshButton = null!;
    [global::Views.CommandBinding(nameof(StatisticsViewModel.ReturnToMenuCommand))]
    private Button _backButton = null!;
    private IContainer? components;

    private void InitializeComponent()
    {
        components = new Container();
        _titleLabel = new Label();
        _descriptionLabel = new Label();
        _sampleNoticeLabel = new Label();
        _statusLabel = new Label();
        _bucketList = new ListBox();
        _refreshButton = new Button();
        _backButton = new Button();
        _titleLabel.AutoSize = false;
        _titleLabel.Dock = DockStyle.Top;
        _titleLabel.Font = new Font(Font, FontStyle.Bold);
        _titleLabel.Height = 34;
        _descriptionLabel.AutoSize = false;
        _descriptionLabel.Dock = DockStyle.Top;
        _descriptionLabel.Height = 28;
        _sampleNoticeLabel.AutoSize = false;
        _sampleNoticeLabel.Dock = DockStyle.Bottom;
        _sampleNoticeLabel.ForeColor = Color.DarkGoldenrod;
        _sampleNoticeLabel.Height = 42;
        _sampleNoticeLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.AutoSize = false;
        _statusLabel.Dock = DockStyle.Bottom;
        _statusLabel.ForeColor = Color.DarkRed;
        _statusLabel.Height = 42;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _bucketList.Dock = DockStyle.Fill;
        _bucketList.Font = new Font("Courier New", 11.25f);
        _refreshButton.AutoSize = true;
        _refreshButton.Text = "Beispiel neu laden";
        _backButton.AutoSize = true;
        _backButton.Text = "Zurück zum Menü";
        var commandPanel = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Bottom, FlowDirection = FlowDirection.LeftToRight };
        commandPanel.Controls.Add(_refreshButton);
        commandPanel.Controls.Add(_backButton);
        Controls.Add(_bucketList);
        Controls.Add(commandPanel);
        Controls.Add(_statusLabel);
        Controls.Add(_sampleNoticeLabel);
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
