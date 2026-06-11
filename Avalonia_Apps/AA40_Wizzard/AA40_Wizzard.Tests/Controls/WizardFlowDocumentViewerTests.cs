using AA40_Wizzard.Controls;
using Avalonia.Controls;

namespace AA40_Wizzard.Tests.Controls;

[TestClass]
public class WizardFlowDocumentViewerTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
        => TestAppBuilder.EnsureInitialized();

    [TestMethod]
    public void SettingDocumentXamlCreatesPreviewContentTest()
    {
        var viewer = new WizardFlowDocumentViewer
        {
            DocumentXaml = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run>Viewer text</Run></Paragraph></FlowDocument>",
        };

        Assert.IsNotNull(viewer.Content);
        Assert.IsInstanceOfType<StackPanel>(viewer.Content);
    }
}
