using Avalonia.Controls;
using Avalonia.Media;
using Avln_CommonDialogs.Base.Interfaces;

namespace Avln_CommonDialogs.Avalonia.Dialogs;

public sealed class AvaloniaColorDialog : IColorDialog
{
    private readonly Func<TopLevel?> _topLevelProvider;

    public AvaloniaColorDialog(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider;
    }

    public bool AllowAlpha { get; set; } = true;

    // expects/returns Avalonia.Media.Color (kept as object for base agnostic interface)
    public object? Color { get; set; }

    public ValueTask<bool?> ShowAsync(object owner)
        => ShowInternalAsync(TopLevelOwner.From(owner));

    public ValueTask<bool?> ShowAsync()
        => ShowInternalAsync(_topLevelProvider());

    private async ValueTask<bool?> ShowInternalAsync(TopLevel? owner)
    {
        var ownerWindow = owner as Window;

        var picker = new global::Avalonia.Controls.ColorPicker
        {
            IsAlphaEnabled = AllowAlpha,
            Color = Color is Color c ? c : Colors.Black
        };

        var ok = new Button { Content = "OK", IsDefault = true };
        var cancel = new Button { Content = "Cancel", IsCancel = true };

        var buttons = new StackPanel
        {
            Orientation = global::Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Right,
            Spacing = 8,
            Children = { ok, cancel }
        };

        var root = new DockPanel
        {
            LastChildFill = true,
            Children = { buttons, picker }
        };

        DockPanel.SetDock(buttons, Dock.Bottom);

        var dlg = new Window
        {
            Title = "Color",
            Width = 520,
            Height = 420,
            Content = root,
            WindowStartupLocation = ownerWindow is null
                ? WindowStartupLocation.CenterScreen
                : WindowStartupLocation.CenterOwner
        };

        var tcs = new TaskCompletionSource<bool?>(TaskCreationOptions.RunContinuationsAsynchronously);

        ok.Click += (_, _) => { Color = picker.Color; tcs.TrySetResult(true); dlg.Close(); };
        cancel.Click += (_, _) => { tcs.TrySetResult(false); dlg.Close(); };
        dlg.Closed += (_, _) => tcs.TrySetResult(null);

        if (ownerWindow is not null)
            await dlg.ShowDialog(ownerWindow).ConfigureAwait(false);
        else
            dlg.Show();

        return await tcs.Task.ConfigureAwait(false);
    }
}
