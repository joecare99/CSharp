using System;
using System.Collections.Generic;
using System.Linq;
using Osb.ModernHost.Models;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Services;

public sealed class ModernHostNavigationService : IHostNavigationService
{
    private readonly IReadOnlyDictionary<ModernHostPage, IHostPageViewModel> _pages;

    public ModernHostNavigationService(IEnumerable<IHostPageViewModel> pages)
    {
        if (pages == null)
        {
            throw new ArgumentNullException(nameof(pages));
        }

        var pageList = pages.ToList();
        if (pageList.Count == 0)
        {
            throw new ArgumentException("At least one host page must be registered.", nameof(pages));
        }

        _pages = pageList.ToDictionary(page => page.Page);
        if (!_pages.TryGetValue(ModernHostPage.Menu, out var menuPage))
        {
            throw new ArgumentException("The menu page must be registered.", nameof(pages));
        }

        ActivePageViewModel = menuPage;

        foreach (var requestSource in pageList.OfType<IPageNavigationRequestSource>())
        {
            requestSource.NavigationRequested += PageViewModel_NavigationRequested;
        }
    }

    public event EventHandler<PageChangedEventArgs>? ActivePageChanged;

    public IHostPageViewModel ActivePageViewModel { get; private set; }

    public void NavigateTo(ModernHostPage page)
    {
        if (!_pages.TryGetValue(page, out var targetPage))
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "The requested host page is not registered.");
        }

        if (ReferenceEquals(ActivePageViewModel, targetPage))
        {
            return;
        }

        ActivePageViewModel = targetPage;
        ActivePageChanged?.Invoke(this, new PageChangedEventArgs(targetPage));
    }

    private void PageViewModel_NavigationRequested(object? sender, PageNavigationRequestedEventArgs eventArgs)
    {
        NavigateTo(eventArgs.Page);
    }
}
