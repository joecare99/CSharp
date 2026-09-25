using System;
using System.Drawing;
using System.Windows.Forms;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Views;

internal sealed partial class OFBMenuPageView : UserControl
{
    public OFBMenuPageView()
    {
        InitializeComponent();
    }

    public OFBMenuPageView(OFBMenuViewModel viewModel)
    {
        InitializeComponent();
        global::Views.ViewBinding.Commit(this, viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
    }

    private static Label CreateHeaderLabel(string text, int top)
    {
        var label = new Label();
        ConfigureHeader(label, Color.Red, Color.White, FontStyle.Bold, top);
        label.Text = text;
        return label;
    }

    private static void ConfigureActionButton(Button button, string text)
    {
        button.AutoSize = false;
        button.BackColor = Color.FromArgb(128, 255, 255);
        button.Margin = new Padding(0, 0, 0, 8);
        button.Size = new Size(260, 36);
        button.Text = text;
        button.UseVisualStyleBackColor = false;
    }

    private static void ConfigureHeader(Label label, Color backColor, Color foreColor, FontStyle style, int top)
    {
        label.AutoSize = false;
        label.BackColor = backColor;
        label.Dock = DockStyle.Top;
        label.Font = new Font("Arial", 11.25f, style);
        label.ForeColor = foreColor;
        label.Height = 28;
        label.Location = new Point(0, top);
        label.TextAlign = ContentAlignment.MiddleCenter;
    }
}
