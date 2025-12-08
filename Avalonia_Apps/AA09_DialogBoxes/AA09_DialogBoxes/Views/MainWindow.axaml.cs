using System;
using Avalonia.Controls;
using AA09_DialogBoxes.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using AA09_DialogBoxes.Messages;
using MessageBox.Avalonia.Enums;

namespace AA09_DialogBoxes.Views;

public partial class MainWindow : Window
{
    public Func<object,IDialogWindow> NewDialogWindow = (o) => new DialogWindow() { DataContext = o};

    public MainWindow()
    {
        InitializeComponent();
        DataContext ??= new MainWindowViewModel();
        this.Opened += Window_Opened;
    }

    private void Window_Opened(object? sender, EventArgs e)
    {
        // Register UI handlers for messages
        WeakReferenceMessenger.Default.Register<MessageBoxRequestMessage>(this, async (r, m) =>
        {
            var box = MessageBoxManager.GetMessageBoxStandard(m.Title, m.Content, ButtonEnum.YesNo);
            m.Reply(box.ShowAsync());
        });

        WeakReferenceMessenger.Default.Register<EditDialogRequestMessage>(this, (r, m) =>
        {
            var dialog = NewDialogWindow(new DialogWindowViewModel());
            var vm = (DialogWindowViewModel)dialog.DataContext;
            vm.Name = m.Name;
            vm.Email = m.Email;
            m.Reply(dialog.ShowDialog(this));
        });
    }
}
