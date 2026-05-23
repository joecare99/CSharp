using Avalonia.Media;
using Avln_CommonDialogs.Base.Interfaces;
using Avln_CommonDialogs.Base.Models;
using Avln_CommonDialogs.Avalonia.ViewModels;
using Avln_CommonDialogs.Avalonia.Views;
using Avalonia.Controls;
using Avalonia.Layout;

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

    // expects/returns Avln_CommonDialogs.Base.Models.FontDialogSelection (kept as object for base agnostic interface)
    public object? Selection { get; set; }

    public string? PreviewText { get; set; }

    public FontDialogPresentationMode PresentationMode { get; set; }

    public ValueTask<bool?> ShowAsync(object owner)
        => ShowInternalAsync(owner, TopLevelOwner.From(owner));

    public ValueTask<bool?> ShowAsync()
        => ShowInternalAsync(null, _topLevelProvider());

    private async ValueTask<bool?> ShowInternalAsync(object? ownerReference, TopLevel? owner)
    {
        var ownerWindow = owner as Window;

        var families = FontManager.Current.SystemFonts
            .OrderBy(f => f.Name)
            .ToArray();

        var initialSelection = CreateInitialSelection();
        var viewModel = new FontPickerViewModel(families, initialSelection, PreviewText);

        if (PresentationMode == FontDialogPresentationMode.Overlay && ownerReference is not null)
        {
            var overlayResult = await ShowOverlayAsync(ownerReference, viewModel);
            if (overlayResult.HasValue)
                return overlayResult;
        }

        var dlg = new FontPickerWindow(viewModel)
        {
            WindowStartupLocation = ownerWindow is null
                ? WindowStartupLocation.CenterScreen
                : WindowStartupLocation.CenterOwner
        };

        var ok = dlg.EmbeddedPickerView.OkButton;
        var cancel = dlg.EmbeddedPickerView.CancelButton;

        var tcs = new TaskCompletionSource<bool?>(TaskCreationOptions.RunContinuationsAsynchronously);

        ok.Click += (_, _) =>
        {
            var selection = viewModel.CreateSelection();
            Font = new FontFamily(selection.FamilyName ?? string.Empty);
            Selection = selection;
            PreviewText = viewModel.PreviewText;
            tcs.TrySetResult(true);
            dlg.Close();
        };
        cancel.Click += (_, _) => { tcs.TrySetResult(false); dlg.Close(); };
        dlg.Closed += (_, _) => tcs.TrySetResult(null);

        if (ownerWindow is not null)
            await dlg.ShowDialog(ownerWindow);
        else
            dlg.Show();

        return await tcs.Task;
    }

    private async Task<bool?> ShowOverlayAsync(object ownerReference, FontPickerViewModel viewModel)
    {
        var hostPanel = FontPickerOverlay.FindOverlayHost(ownerReference);
        if (hostPanel is null)
            return null;

        var overlay = new FontPickerOverlay
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        overlay.SetValue(Panel.ZIndexProperty, 1000);
        hostPanel.Children.Add(overlay);

        try
        {
            var accepted = await overlay.ShowAsync(viewModel);
            if (accepted == true && overlay.AcceptedSelection is { } selection)
            {
                Font = new FontFamily(selection.FamilyName ?? string.Empty);
                Selection = selection;
                PreviewText = viewModel.PreviewText;
            }

            return accepted;
        }
        finally
        {
            hostPanel.Children.Remove(overlay);
        }
    }

    private FontDialogSelection CreateInitialSelection()
    {
        if (Selection is FontDialogSelection selection)
            return selection.Clone();

        if (Font is FontFamily current)
        {
            return new FontDialogSelection
            {
                FamilyName = current.Name
            };
        }

        return new FontDialogSelection();
    }
}
