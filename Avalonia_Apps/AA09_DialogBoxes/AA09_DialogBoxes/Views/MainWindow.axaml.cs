using System;
using Avalonia.Controls;
using AA09_DialogBoxes.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using AA09_DialogBoxes.Messages;

namespace AA09_DialogBoxes.Views;

public partial class MainWindow : Window
{
    private readonly OverlayDialogControl _overlayDialogControl;

    public Func<object,IDialogWindow> NewDialogWindow = (o) => new DialogWindow() { DataContext = o};

    public MainWindow()
    {
        InitializeComponent();
        DataContext ??= new MainWindowViewModel();

        var root = this.FindControl<Grid>("MainRootGrid")!;
        _overlayDialogControl = new OverlayDialogControl();
        _overlayDialogControl.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        _overlayDialogControl.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        _overlayDialogControl.SetValue(Panel.ZIndexProperty, 10);
        root.Children.Add(_overlayDialogControl);

        this.Opened += Window_Opened;
    }

    private void Window_Opened(object? sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Register<MessageBoxRequestMessage>(this, (r, m) =>
        {
            var box = new MessageBoxWindow(m.Title, m.Content);
            m.Reply(box.ShowDialog<MsgBoxResult>(this));
        });

        WeakReferenceMessenger.Default.Register<OverlayMessageRequestMessage>(this, (r, m) =>
        {
            m.Reply(_overlayDialogControl.ShowAsync(this, m.Title, m.Content));
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
