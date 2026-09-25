using System;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.ViewModels;

public interface IPageNavigationRequestSource
{
    event EventHandler<PageNavigationRequestedEventArgs>? NavigationRequested;
}
