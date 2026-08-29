using System;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>ANSI renderer for the standard ConsoleLib form controls.</summary>
public sealed class PosixFormRenderer : IFormWidgetRenderer
{
    private readonly IAnsiOutput _output;

    public PosixFormRenderer(IAnsiOutput output) =>
        _output = output ?? throw new ArgumentNullException(nameof(output));

    public void DrawCheckBox(CheckBox checkBox) =>
        Write(checkBox, checkBox.IsChecked ? "[x] " : "[ ] ", checkBox.Text);

    public void DrawComboBox(ComboBox comboBox) =>
        Write(comboBox, "[", (comboBox.SelectedItem ?? string.Empty) + "]");

    public void DrawProgressBar(ProgressBar progressBar)
    {
        var width = Math.Max(0, progressBar.size.Width);
        var filled = (int)Math.Round(width * progressBar.Fraction, MidpointRounding.ToZero);
        Write(progressBar, new string('#', filled) + new string('-', width - filled), string.Empty);
    }

    public void DrawStatusBar(StatusBar statusBar) =>
        Write(statusBar, string.Empty, statusBar.Status);

    public void DrawTabControl(TabControl tabControl)
    {
        var text = string.Empty;
        foreach (var item in tabControl.Items)
            text += ReferenceEquals(item, tabControl.SelectedItem) ? $"[{item.Header}]" : $" {item.Header} ";
        Write(tabControl, string.Empty, text);
    }

    private void Write(Control control, string prefix, string text)
    {
        var width = Math.Max(0, control.size.Width);
        var content = (prefix + text);
        if (content.Length > width)
            content = content[..width];
        else if (content.Length < width)
            content = content.PadRight(width);
        _output.MoveCursorAsync(control.RealDim.Left + 1, control.RealDim.Top + 1).GetAwaiter().GetResult();
        _output.WriteAsync(content).GetAwaiter().GetResult();
        _output.ResetAsync().GetAwaiter().GetResult();
    }
}
