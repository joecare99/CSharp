using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.ModernHost.Views;

namespace Osb.ModernHost.Tests.Views;

[TestClass]
public sealed class DesignerViewConstructionTests
{
    [TestMethod]
    public void DesignerViewsCanBeConstructedWithoutViewModels()
    {
        using var mainForm = new MainForm();
        using var menuView = new OFBMenuPageView();
        using var placeView = new PlaceSelectionPageView();
        using var settingsView = new PublicationSettingsPageView();
        using var familyView = new FamilyPublicationPageView();
        using var statisticsView = new StatisticsPageView();
        using var fallbackView = new HostPageView();

        Assert.IsFalse(mainForm.IsDisposed);
        Assert.IsFalse(menuView.IsDisposed);
        Assert.IsFalse(placeView.IsDisposed);
        Assert.IsFalse(settingsView.IsDisposed);
        Assert.IsFalse(familyView.IsDisposed);
        Assert.IsFalse(statisticsView.IsDisposed);
        Assert.IsFalse(fallbackView.IsDisposed);
    }
}
