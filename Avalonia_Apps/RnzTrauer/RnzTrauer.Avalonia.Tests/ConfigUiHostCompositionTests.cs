using Config.Service;
using Config.UI;
using Config.UI.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace RnzTrauer.Avalonia.Tests;

/// <summary>
/// Verifies that RnzTrauer owns its provider registration while consuming Config.UI through DI.
/// </summary>
[TestClass]
public sealed class ConfigUiHostCompositionTests
{
    [TestMethod]
    public void AddConfigurationUi_RegistersOrderedRnzSectionsAndSharedUi()
    {
        ServiceCollection services = new();
        App.AddConfigurationUi(services);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ConfigService configService = serviceProvider.GetRequiredService<ConfigService>();
        IConfigUiSectionRegistry registry = serviceProvider.GetRequiredService<IConfigUiSectionRegistry>();

        Assert.AreEqual("JC-Soft", configService.VendorName);
        Assert.AreEqual("RnzTrauer", configService.ApplicationName);
        CollectionAssert.AreEqual(
            new[] { "Database", "Places", "Acquisition", "Announcements" },
            registry.Sections.Select(static section => section.Section.Name).ToArray());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ConfigUiViewModel>());
    }
}
