using System;
using AA09_DialogBoxes.Messages;
using AA09_DialogBoxes.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;

namespace AA09_DialogBoxes.Views;

public partial class BrowserMainView : UserControl
{
    private readonly OverlayDialogControl _overlayDialogControl;
    private readonly BrowserEditDialogControl _editDialogControl;

    public BrowserMainView()
    {
        InitializeComponent();
        DataContext ??= new MainWindowViewModel();
        _overlayDialogControl = this.FindControl<OverlayDialogControl>("OverlayDialogControl")!;
        _editDialogControl = this.FindControl<BrowserEditDialogControl>("EditDialogControl")!;
        _overlayDialogControl.SetValue(Panel.ZIndexProperty, 10);
        _editDialogControl.SetValue(Panel.ZIndexProperty, 11);
        Loaded += BrowserMainView_Loaded;
        Unloaded += BrowserMainView_Unloaded;
    }

    private void BrowserMainView_Loaded(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        WeakReferenceMessenger.Default.Register<MessageBoxRequestMessage>(this, (r, m) =>
        {
            m.Reply(_overlayDialogControl.ShowAsync(m.Title, m.Content));
        });

        WeakReferenceMessenger.Default.Register<OverlayMessageRequestMessage>(this, (r, m) =>
        {
            m.Reply(_overlayDialogControl.ShowAsync(m.Title, m.Content));
        });

        WeakReferenceMessenger.Default.Register<EditDialogRequestMessage>(this, (r, m) =>
        {
            m.Reply(_editDialogControl.ShowAsync(m.Name, m.Email));
        });
    }

    private void BrowserMainView_Unloaded(object? sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
