using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ImageDocumentEditorTests
{
    [TestMethod]
    public void WorkbenchRegistersImageFactoryForSupportedExtensionsOnly()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_ImageFactory_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            ServiceCollection services = new();
            App.ConfigureServices(services, () => null, root, Path.Combine(root, "system"),
                Path.Combine(root, "group"), "test");
            using ServiceProvider provider = services.BuildServiceProvider();
            IDocumentEditorFactory<EditorTabViewModel> factory = provider
                .GetServices<IDocumentEditorFactory<EditorTabViewModel>>()
                .Single(item => item.Id == "image");

            foreach (string path in new[] { "a.png", "a.PNG", "a.jpg", "a.jpeg", "a.bmp" })
            {
                Assert.IsTrue(factory.CanOpen(path), path);
            }

            foreach (string path in new[] { "a.svg", "a.png.cs", "a.gif", "a" })
            {
                Assert.IsFalse(factory.CanOpen(path), path);
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
