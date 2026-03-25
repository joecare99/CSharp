using Avalonia.Controls;
using System;
using AA25_RichTextEdit.ViewModels;
using Avln_CommonDialogs.Base.Interfaces;

namespace AA25_RichTextEdit.Views;

public partial class RichTextEditView : UserControl
{
    public RichTextEditView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += RichTextEditView_AttachedToVisualTree;
    }

    private void RichTextEditView_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is RichTextEditViewModel vm)
        {
            vm.FileOpenDialog = DoFileDialog;
            vm.FileSaveAsDialog = DoFileDialog;
            vm.dPrintDialog = DoPrintDialog;
            vm.CloseApp = DoClose;
        }
    }

    private bool? DoFileDialog(string filename, IFileDialog par, Action<string, IFileDialog>? onAccept)
    {
        par.FileName = filename;
        var window = this.GetVisualRoot() as Window;
        bool? result = par.ShowDialog(window);
        if (result ?? false) onAccept?.Invoke(par.FileName, par);
        return result;
    }

    private bool? DoPrintDialog(IPrintDialog par, Action<IPrintDialog, object?>? onPrint)
    {
        bool? result = par.ShowAsync().GetAwaiter().GetResult();
        
        if (result ?? false) onPrint?.Invoke(par, null); // Avalonia placeholder
        return result;
    }

    private void DoClose() => (this)?.Close();
}
