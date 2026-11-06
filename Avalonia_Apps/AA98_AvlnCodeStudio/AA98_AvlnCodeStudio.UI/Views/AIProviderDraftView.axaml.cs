using Avalonia.Controls;
using Avalonia.Interactivity;
using AA98_AvlnCodeStudio.UI.Configuration;
using System;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Views;

public partial class AIProviderDraftView : UserControl
{
    public AIProviderDraftView() => InitializeComponent();

    public Func<Task>? OpenCredentialsAsync { get; set; }

    private async void OnManageCredentialsClick(object? sender, RoutedEventArgs args)
    {
        if (DataContext is AIProviderDraftViewModel { CanManageActiveCredentials: true }
            && OpenCredentialsAsync is not null)
        {
            await OpenCredentialsAsync();
        }
    }
}
