using System;
using Avalonia;
using Avalonia.Controls;
using AA09_DialogBoxes.ViewModels;
using System.Threading.Tasks;

namespace AA09_DialogBoxes.Views;

public partial class DialogWindow : Window, IDialogWindow
{
    private DialogWindowViewModel vm;

    public DialogWindow()
    {
        InitializeComponent();
        DataContext ??= new DialogWindowViewModel();
        this.Opened += Window_Opened;
    }

    private void Window_Opened(object? sender, EventArgs e)
    {
        vm = (DialogWindowViewModel)DataContext!;
        vm.DoCancel += DoCancel;
        vm.DoOK += DoOK;
    }

    private void DoOK(object? sender, EventArgs e) => Close((true,(vm.Name,vm.Email)));
    private void DoCancel(object? sender, EventArgs e) => Close((false,("","")));

    Task<(bool, (string, string))> IDialogWindow.ShowDialog(Window o) => this.ShowDialog<(bool, (string, string))>(o);
}
