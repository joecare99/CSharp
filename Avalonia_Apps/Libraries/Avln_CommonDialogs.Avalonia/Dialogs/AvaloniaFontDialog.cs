using Avalonia.Controls;
using Avalonia.Media;
using Avln_CommonDialogs.Base.Interfaces;

namespace Avln_CommonDialogs.Avalonia.Dialogs;

public sealed class AvaloniaFontDialog : IFontDialog
{
    private readonly Func<TopLevel?> _topLevelProvider;

    public AvaloniaFontDialog(Func<TopLevel?> topLevelProvider)
    {
        _topLevelProvider = topLevelProvider;
    }

    // expects/returns Avalonia.Media.FontFamily (kept as object for base agnostic interface)
    public object? Font { get; set; }

    public ValueTask<bool?> ShowAsync(object owner)
        => ShowInternalAsync(TopLevelOwner.From(owner));

    public ValueTask<bool?> ShowAsync()
        => ShowInternalAsync(_topLevelProvider());

    private async ValueTask<bool?> ShowInternalAsync(TopLevel? owner)
    {
        var ownerWindow = owner as Window;

        var families = FontManager.Current.SystemFonts
            .OrderBy(f => f.Name)
            .ToArray();

        var list = new ListBox
        {
            ItemsSource = families
        };

        if (Font is FontFamily current)
            list.SelectedItem = families.FirstOrDefault(f => f.Name == current.Name);

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
            Children = { buttons, list }
        };

        DockPanel.SetDock(buttons, Dock.Bottom);

        var dlg = new Window
        {
            Title = "Font",
            Width = 700,
            Height = 520,
            Content = root,
            WindowStartupLocation = ownerWindow is null
                ? WindowStartupLocation.CenterScreen
                : WindowStartupLocation.CenterOwner
        };

        var tcs = new TaskCompletionSource<bool?>(TaskCreationOptions.RunContinuationsAsynchronously);

        ok.Click += (_, _) => { Font = list.SelectedItem as FontFamily; tcs.TrySetResult(true); dlg.Close(); };
        cancel.Click += (_, _) => { tcs.TrySetResult(false); dlg.Close(); };
        dlg.Closed += (_, _) => tcs.TrySetResult(null);

        if (ownerWindow is not null)
            await dlg.ShowDialog(ownerWindow).ConfigureAwait(false);
        else
            dlg.Show();

        return await tcs.Task.ConfigureAwait(false);
    }
}
