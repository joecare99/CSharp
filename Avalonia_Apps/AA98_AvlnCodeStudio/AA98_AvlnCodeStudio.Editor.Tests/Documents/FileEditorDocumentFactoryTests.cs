using AA98_AvlnCodeStudio.Model.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Editor.Tests.Documents;

/// <summary>
/// Verifies the default document factory for the editor component.
/// </summary>
[TestClass]
public class FileEditorDocumentFactoryTests
{
    /// <summary>
    /// Verifies that the factory creates a fresh document instance each time.
    /// </summary>
    [TestMethod]
    public void Create_ReturnsFreshDocumentInstance()
    {
        var factory = new FileEditorDocumentFactory();

        var first = factory.Create();
        var second = factory.Create();

        Assert.IsNotNull(first);
        Assert.IsNotNull(second);
        Assert.AreNotSame(first, second);
        Assert.AreEqual("Untitled.txt", first.DisplayName);
    }
}