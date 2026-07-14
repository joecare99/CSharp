using Avalonia.Controls;
using Avalonia.Threading;
using AvlnAhnenNew.Interfaces;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using WinAhnenNew.Messages;

namespace AvlnAhnenNew.Views;

public partial class MainWindow : Window
{
    private readonly IMessenger _messenger;
    private int _iLastSelectedTabIndex;

    public MainWindow()
    {
        InitializeComponent();

        var appServices = ((App)Avalonia.Application.Current!).Services;
        _messenger = appServices.GetRequiredService<IMessenger>();

        SelectionHost.Content = appServices.GetRequiredService<Controls.SelectionPageView>();
        EditHost.Content = appServices.GetRequiredService<Controls.EditPageView>();
        DetailHost.Content = appServices.GetRequiredService<Controls.DetailPageView>();
        RelationshipsHost.Content = appServices.GetRequiredService<Controls.RelationshipsPageView>();
        SiblingsHost.Content = appServices.GetRequiredService<Controls.SiblingsPageView>();
        TextHost.Content = appServices.GetRequiredService<Controls.TextPageView>();
        AdditionalHost.Content = appServices.GetRequiredService<Controls.AdditionalPageView>();
        AddressHost.Content = appServices.GetRequiredService<Controls.AddressPageView>();
        ImagesHost.Content = appServices.GetRequiredService<Controls.ImagesPageView>();

        _messenger.Register<NavigateToEditTabMessage>(this, static (objRecipient, _) =>
        {
            if (objRecipient is MainWindow wndMain)
            {
                wndMain.PageControl1.SelectedIndex = 1;
            }
        });

        Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _messenger.UnregisterAll(this);
    }

    private void PageControl1_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TabControl pageControl)
        {
            return;
        }

        var commitPendingEdits = _iLastSelectedTabIndex == 1
            ? EditHost.Content as ICommitPendingEdits
            : null;

        _iLastSelectedTabIndex = pageControl.SelectedIndex;

        if (commitPendingEdits is not null)
        {
            Dispatcher.UIThread.Post(commitPendingEdits.CommitPendingEdits);
        }
    }
}
