using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Models;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Tests.ViewModels;

[TestClass]
public sealed class PlaceSelectionViewModelTests
{
    [TestMethod]
    public void ApplySelectionText_LoadsEntriesAndUsesCanonicalPersistenceText()
    {
        var viewModel = new PlaceSelectionViewModel
        {
            SelectionText = "Musterort\nNebenort"
        };

        viewModel.ApplySelectionTextCommand.Execute(null);

        CollectionAssert.AreEqual(new[] { "Musterort", "Nebenort" }, viewModel.SelectedPlaces);
        Assert.AreEqual("Musterort\r\nNebenort", viewModel.SerializedSelectionText);
        StringAssert.Contains(viewModel.StatusText, "2 Ort(e)");
    }

    [TestMethod]
    public void ApplySelectionText_ReportsInvalidSelectionWithoutChangingCurrentEntries()
    {
        var viewModel = new PlaceSelectionViewModel();
        viewModel.SelectedPlaces.Add("Musterort");
        viewModel.SelectionText = "Ort A\r\n\r\nOrt B";

        viewModel.ApplySelectionTextCommand.Execute(null);

        CollectionAssert.AreEqual(new[] { "Musterort" }, viewModel.SelectedPlaces);
        StringAssert.Contains(viewModel.StatusText, "empty");
    }

    [TestMethod]
    public void ReturnToMenuCommandRequestsMenuPage()
    {
        var viewModel = new PlaceSelectionViewModel();
        ModernHostPage? requestedPage = null;
        viewModel.NavigationRequested += (_, eventArgs) => requestedPage = eventArgs.Page;

        viewModel.ReturnToMenuCommand.Execute(null);

        Assert.AreEqual(ModernHostPage.Menu, requestedPage);
    }

    [TestMethod]
    public void PageDescribesUnavailableSearchExplicitly()
    {
        var viewModel = new PlaceSelectionViewModel();

        Assert.IsFalse(viewModel.IsSearchAvailable);
        Assert.AreEqual(ModernHostPage.PlaceSelection, viewModel.Page);
        StringAssert.Contains(viewModel.StatusText, "noch nicht angebunden");
    }
}
