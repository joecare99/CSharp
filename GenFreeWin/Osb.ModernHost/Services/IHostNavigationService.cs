using System;
using Osb.ModernHost.Models;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Services;

public interface IHostNavigationService
{
    event EventHandler<PageChangedEventArgs>? ActivePageChanged;

    IHostPageViewModel ActivePageViewModel { get; }

    void NavigateTo(ModernHostPage page);
}
