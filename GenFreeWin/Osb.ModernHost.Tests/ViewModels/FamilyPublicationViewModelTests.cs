using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Models;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.ViewModels.Tests;

[TestClass]
public sealed class FamilyPublicationViewModelTests
{
    [TestMethod]
    public void CreatePublication_ReportsUnavailableDataSource()
    {
        var viewModel = new FamilyPublicationViewModel();
        viewModel.CreatePublicationCommand.Execute(null);
        StringAssert.Contains(viewModel.StatusText, "nicht erstellt");
    }

    [TestMethod]
    public void ReturnToMenu_RequestsMenu()
    {
        var viewModel = new FamilyPublicationViewModel();
        ModernHostPage? page = null;
        viewModel.NavigationRequested += (_, args) => page = args.Page;
        viewModel.ReturnToMenuCommand.Execute(null);
        Assert.AreEqual(ModernHostPage.Menu, page);
    }
}
