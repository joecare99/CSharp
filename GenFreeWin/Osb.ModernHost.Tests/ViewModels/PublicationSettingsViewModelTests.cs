using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Models;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Tests.ViewModels;

[TestClass]
public sealed class PublicationSettingsViewModelTests
{
    [TestMethod]
    public void ValidateProfile_AcceptsTypedDefaultProfile()
    {
        var viewModel = new PublicationSettingsViewModel();

        viewModel.ValidateProfileCommand.Execute(null);

        StringAssert.Contains(viewModel.StatusText, "ist gültig");
    }

    [TestMethod]
    public void ValidateProfile_ReportsInvalidProfile()
    {
        var viewModel = new PublicationSettingsViewModel { ProfileName = " " };

        viewModel.ValidateProfileCommand.Execute(null);

        StringAssert.Contains(viewModel.StatusText, "non-empty");
    }

    [TestMethod]
    public void ReturnToMenu_RequestsMenuPage()
    {
        var viewModel = new PublicationSettingsViewModel();
        ModernHostPage? page = null;
        viewModel.NavigationRequested += (_, eventArgs) => page = eventArgs.Page;

        viewModel.ReturnToMenuCommand.Execute(null);

        Assert.AreEqual(ModernHostPage.Menu, page);
    }
}
