using Avalonia.Controls;
using Avalonia.VisualTree;
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
        this.AttachedToVisualTree += RichTextEditView_AttachedToVisualTree;
    }

    private void RichTextEditView_AttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
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
        var files = await par.ShowAsync();
        bool result = files.Count > 0;
        if (result) onAccept?.Invoke(files[0], par);
        return result;
    }

    private async Task<bool?> DoSaveFileDialog(string filename, ISaveFileDialog par, Action<string, ISaveFileDialog>? onAccept)
    {
        par.InitialFileName = filename;
        var file = await par.ShowAsync();
        bool result = file != null;
        if (result) onAccept?.Invoke(file!, par);
        return result;
    }

    private async Task<bool?> DoPrintDialog(IPrintDialog par, Action<IPrintDialog, object?>? onPrint)
    {
        var session = await par.ShowAsync();
        bool result = session != null;
        if (result) onPrint?.Invoke(par, session);
        return result;
    }

    private void DoClose() => (TopLevel.GetTopLevel(this) as Window)?.Close();
}
