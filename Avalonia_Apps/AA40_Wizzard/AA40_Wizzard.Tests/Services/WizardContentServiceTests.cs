using AA40_Wizzard.Model;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using System.Linq;

namespace AA40_Wizzard.Tests.Services;

[TestClass]
public class WizardContentServiceTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
        => TestAppBuilder.EnsureInitialized();

    [TestMethod]
    public void GetDocumentPreviewAndImageSourceUseEmbeddedAssetsTest()
    {
        var service = new WizardContentService();

        var documentPreview = service.GetDocumentPreview(0);
        var image = service.GetImage(0);

        Assert.IsNotNull(documentPreview);
        Assert.IsInstanceOfType<StackPanel>(documentPreview);
        var firstTextBlock = ((StackPanel)documentPreview).Children.OfType<TextBlock>().FirstOrDefault();
        Assert.IsNotNull(firstTextBlock);
        Assert.IsTrue((firstTextBlock.Inlines?.Count ?? 0) > 0);
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void GetOptionsCreatesLocalizedEntriesTest()
    {
        var service = new FakeWizardContentService();

        var result = service.GetOptions([1, 2], "MainSelection");

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEqual(new[] { 1, 2 }, result.Select(entry => entry.ID).ToArray());
        Assert.AreEqual("MainSelection:1", result[0].Text);
    }
}
