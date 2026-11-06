using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Views;

public partial class AIProviderDraftView : UserControl
{
    public AIProviderDraftView() => InitializeComponent();

    public Func<Task>? OpenCredentialsAsync { get; set; }

    private async void OnManageCredentialsClick(object? sender, RoutedEventArgs args)
    {
        if (OpenCredentialsAsync is not null)
        {
            await OpenCredentialsAsync();
        }
    }
}
