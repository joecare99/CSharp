using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Models;
using Osb.ModernHost.Services;
using Osb.ModernHost.ViewModels;

namespace Osb.ModernHost.Tests.ViewModels;

[TestClass]
public sealed class StatisticsViewModelTests
{
    [TestMethod]
    public void Constructor_ExposesDeterministicSampleBuckets()
    {
        var viewModel = new StatisticsViewModel(new StatisticsDisplayService());

        Assert.AreEqual(ModernHostPage.Statistics, viewModel.Page);
        Assert.AreEqual(11, viewModel.Buckets.Count);
        Assert.AreEqual("unbekannt", viewModel.Buckets[0].Label);
        Assert.AreEqual(1, viewModel.Buckets[0].Count);
        Assert.AreEqual(2, viewModel.Buckets[2].Count);
        StringAssert.Contains(viewModel.SampleNotice, "keine Tenant");
    }

    [TestMethod]
    public void RefreshSample_ReportsDisplayOnlyRefresh()
    {
        var viewModel = new StatisticsViewModel(new StatisticsDisplayService());

        viewModel.RefreshSampleCommand.Execute(null);

        StringAssert.Contains(viewModel.StatusText, "Beispieldarstellung");
    }

    [TestMethod]
    public void ReturnToMenu_RequestsMenu()
    {
        var viewModel = new StatisticsViewModel(new StatisticsDisplayService());
        ModernHostPage? requestedPage = null;
        viewModel.NavigationRequested += (_, args) => requestedPage = args.Page;

        viewModel.ReturnToMenuCommand.Execute(null);

        Assert.AreEqual(ModernHostPage.Menu, requestedPage);
    }
}
