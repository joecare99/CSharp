using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Models;
using Osb.ModernHost.Services;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Tests;

[TestClass]
public sealed class ModernHostNavigationServiceTests
{
    [TestMethod]
    public void NavigateTo_ChangesActivePageAndRaisesEvent()
    {
        var navigationService = CreateNavigationService();
        PageChangedEventArgs? observedEvent = null;
        navigationService.ActivePageChanged += (_, eventArgs) => observedEvent = eventArgs;

        navigationService.NavigateTo(ModernHostPage.Statistics);

        Assert.AreEqual(ModernHostPage.Statistics, navigationService.ActivePageViewModel.Page);
        Assert.IsNotNull(observedEvent);
        Assert.AreEqual(ModernHostPage.Statistics, observedEvent.ActivePageViewModel.Page);
    }

    [TestMethod]
    public void NavigateTo_SamePageDoesNotRaiseEvent()
    {
        var navigationService = CreateNavigationService();
        var eventCount = 0;
        navigationService.ActivePageChanged += (_, _) => eventCount++;

        navigationService.NavigateTo(ModernHostPage.Menu);

        Assert.AreEqual(0, eventCount);
    }

    [TestMethod]
    public void Constructor_RequiresMenuPage()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => new ModernHostNavigationService(
                new IHostPageViewModel[]
                {
                    new HostPageViewModel(ModernHostPage.Statistics, "Statistik", "Statistikbeschreibung")
                }));
    }

    [TestMethod]
    public void NavigateTo_RejectsUnregisteredPage()
    {
        var navigationService = CreateNavigationService();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => navigationService.NavigateTo(ModernHostPage.PlaceSelection));
    }

    private static ModernHostNavigationService CreateNavigationService()
    {
        return new ModernHostNavigationService(
            new IHostPageViewModel[]
            {
                new HostPageViewModel(ModernHostPage.Menu, "Menü", "Menübeschreibung"),
                new HostPageViewModel(ModernHostPage.Statistics, "Statistik", "Statistikbeschreibung")
            });
    }
}
