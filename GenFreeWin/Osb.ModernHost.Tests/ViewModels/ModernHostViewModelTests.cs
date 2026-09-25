using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Models;
using Osb.ModernHost.Services;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.ViewModels.Tests;

[TestClass]
public sealed class ModernHostViewModelTests
{
    [TestMethod]
    public void ReadyStateIsPresentedAsTenantStatus()
    {
        var tenantPath = Path.Combine(Path.GetTempPath(), "OsbModernHostVmTests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tenantPath);

        try
        {
            var viewModel = new ModernHostViewModel(
                new ModernHostStartupService(new[] { "--tenant", tenantPath }),
                CreateNavigationService());

            Assert.IsTrue(viewModel.IsReady);
            StringAssert.Contains(viewModel.StatusText, "Tenant bereit");
            StringAssert.Contains(viewModel.StatusText, tenantPath);
        }
        finally
        {
            Directory.Delete(tenantPath);
        }
    }

    [TestMethod]
    public void InvalidStateIsPresentedAsActionableError()
    {
        var viewModel = new ModernHostViewModel(
            new ModernHostStartupService(Array.Empty<string>()),
            CreateNavigationService());

        Assert.IsFalse(viewModel.IsReady);
        StringAssert.Contains(viewModel.StatusText, "--tenant");
    }

    [TestMethod]
    public void NavigationUpdatesActivePageViewModel()
    {
        var navigationService = CreateNavigationService();
        var viewModel = new ModernHostViewModel(
            new ModernHostStartupService(Array.Empty<string>()),
            navigationService);

        navigationService.NavigateTo(ModernHostPage.Statistics);

        Assert.AreEqual(ModernHostPage.Statistics, viewModel.ActivePageViewModel!.Page);
        Assert.AreEqual("Statistik", viewModel.ActivePageTitle);
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
