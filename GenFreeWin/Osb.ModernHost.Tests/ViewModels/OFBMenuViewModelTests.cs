using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Models;
using Osb.ModernHost.Services;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.ViewModels.Tests;

[TestClass]
public sealed class OFBMenuViewModelTests
{
    [TestMethod]
    public void MenuCommandsRequestTheirCorrespondingPages()
    {
        var viewModel = new OFBMenuViewModel();
        ModernHostPage? requestedPage = null;
        viewModel.NavigationRequested += (_, eventArgs) => requestedPage = eventArgs.Page;

        viewModel.OpenPlaceSelectionCommand.Execute(null);
        Assert.AreEqual(ModernHostPage.PlaceSelection, requestedPage);

        viewModel.OpenPublicationSettingsCommand.Execute(null);
        Assert.AreEqual(ModernHostPage.PublicationSettings, requestedPage);

        viewModel.OpenFamilyPublicationCommand.Execute(null);
        Assert.AreEqual(ModernHostPage.FamilyPublication, requestedPage);

        viewModel.OpenStatisticsCommand.Execute(null);
        Assert.AreEqual(ModernHostPage.Statistics, requestedPage);
    }

    [TestMethod]
    public void NavigationServiceConsumesMenuNavigationRequests()
    {
        var menuViewModel = new OFBMenuViewModel();
        var navigationService = new ModernHostNavigationService(
            new IHostPageViewModel[]
            {
                menuViewModel,
                new HostPageViewModel(ModernHostPage.Statistics, "Statistik", "Statistikbeschreibung")
            });

        menuViewModel.OpenStatisticsCommand.Execute(null);

        Assert.AreEqual(ModernHostPage.Statistics, navigationService.ActivePageViewModel.Page);
    }

    [TestMethod]
    public void MenuProvidesExpectedPagePresentation()
    {
        var viewModel = new OFBMenuViewModel();

        Assert.AreEqual(ModernHostPage.Menu, viewModel.Page);
        Assert.AreEqual("Ortsfamilienbuch", viewModel.Title);
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.Description));
    }
}
