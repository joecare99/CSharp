using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Input.Platform;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using AA25_RichTextEdit.ViewModels;
using Avln_CommonDialogs.Base.Interfaces;

namespace AA25_RichTextEdit.Views;

public partial class RichTextEditView : UserControl
{
    public RichTextEditView()
    {
        InitializeComponent();
        if (!Design.IsDesignMode)
        {
            DataContext = App.Services.GetRequiredService<RichTextEditViewModel>();
        }

        this.AttachedToVisualTree += RichTextEditView_AttachedToVisualTree;
    }

    private void RichTextEditView_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        App.Services.GetRequiredService<TopLevelAccessor>().Current = TopLevel.GetTopLevel(this);

        if (DataContext is RichTextEditViewModel vm)
        {
            vm.FileOpenDialog = DoOpenFileDialog;
            vm.FileSaveAsDialog = DoSaveFileDialog;
            vm.dPrintDialog = DoPrintDialog;
            vm.CloseApp = DoClose;
        }
    }

    private async Task<bool?> DoOpenFileDialog(string filename, IOpenFileDialog par, Action<string, IOpenFileDialog>? onAccept)
    {
        var owner = TopLevel.GetTopLevel(this);
        var files = owner is null ? await par.ShowAsync() : await par.ShowAsync(owner);
        bool result = files.Count > 0;
        if (result) onAccept?.Invoke(files[0], par);
        return result;
    }

    private async Task<bool?> DoSaveFileDialog(string filename, ISaveFileDialog par, Action<string, ISaveFileDialog>? onAccept)
    {
        par.InitialFileName = filename;
        var owner = TopLevel.GetTopLevel(this);
        var file = owner is null ? await par.ShowAsync() : await par.ShowAsync(owner);
        bool result = file != null;
        if (result) onAccept?.Invoke(file!, par);
        return result;
    }

    private async Task<bool?> DoPrintDialog(IPrintDialog par, Action<IPrintDialog, object?>? onPrint)
    {
        var owner = TopLevel.GetTopLevel(this);
        var session = owner is null ? await par.ShowAsync() : await par.ShowAsync(owner);
        bool result = session != null;
        if (result) onPrint?.Invoke(par, session);
        return result;
    }

    private void DoClose() => (TopLevel.GetTopLevel(this) as Window)?.Close();

    private async void CutToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await CopySelectionToClipboardAsync();
        ReplaceSelectionText(string.Empty);
    }

    private async void CopyToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await CopySelectionToClipboardAsync();
    }

    private async void PasteToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        var text = await clipboard.TryGetTextAsync();
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        ReplaceSelectionText(text);
    }

    private void BoldToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ToggleSelectionFormatting(TextElement.FontWeightProperty, FontWeight.Bold, FontWeight.Normal);
    }

    private void ItalicToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ToggleSelectionFormatting(TextElement.FontStyleProperty, FontStyle.Italic, FontStyle.Normal);
    }

    private void UnderlineToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var currentDecorations = rtb.FlowDocument.Selection.GetFormatting(Inline.TextDecorationsProperty) as TextDecorationCollection;
        var nextDecorations = currentDecorations is { Count: > 0 }
            ? new TextDecorationCollection(Array.Empty<TextDecoration>())
            : new TextDecorationCollection(TextDecorations.Underline);
        rtb.FlowDocument.Selection.ApplyFormatting(Inline.TextDecorationsProperty, nextDecorations);
    }

    private void AlignLeftToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ApplyParagraphAlignment(TextAlignment.Left);
    }

    private void AlignCenterToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ApplyParagraphAlignment(TextAlignment.Center);
    }

    private void AlignRightToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ApplyParagraphAlignment(TextAlignment.Right);
    }

    private void AlignJustifyToolbarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ApplyParagraphAlignment(TextAlignment.Justify);
    }

    private async Task CopySelectionToClipboardAsync()
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        await clipboard.SetTextAsync(rtb.FlowDocument.Selection.Text);
    }

    private void ReplaceSelectionText(string text)
    {
        rtb.FlowDocument.Selection.Text = text;
    }

    private void ApplyParagraphAlignment(TextAlignment textAlignment)
    {
        rtb.FlowDocument.Selection.ApplyFormatting(TextBlock.TextAlignmentProperty, textAlignment);
    }

    private void ToggleSelectionFormatting<T>(Avalonia.AvaloniaProperty property, T activeValue, T inactiveValue)
        where T : struct
    {
        var currentValue = rtb.FlowDocument.Selection.GetFormatting(property);
        var nextValue = currentValue is T typedCurrentValue && typedCurrentValue.Equals(activeValue)
            ? inactiveValue
            : activeValue;

        rtb.FlowDocument.Selection.ApplyFormatting(property, nextValue);
    }
}
