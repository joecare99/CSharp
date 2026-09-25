using System;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Models;

public sealed class PageChangedEventArgs : EventArgs
{
    public PageChangedEventArgs(IHostPageViewModel activePageViewModel)
    {
        ActivePageViewModel = activePageViewModel ?? throw new ArgumentNullException(nameof(activePageViewModel));
    }

    public IHostPageViewModel ActivePageViewModel { get; }
}
