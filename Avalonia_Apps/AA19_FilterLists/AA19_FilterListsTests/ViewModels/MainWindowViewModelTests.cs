using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA19_FilterLists.ViewModels.Tests;

[TestClass]
public class MainWindowViewModelTests
{
    [TestMethod]
    public void MainWindowViewModelProvidesSourceTabsContent()
    {
        var model = new MainWindowViewModel();

        Assert.AreEqual(model.WindowTitle, model.SampleTabHeader);
        Assert.IsFalse(string.IsNullOrWhiteSpace(model.WindowDescription));
        Assert.IsTrue(model.PersonViewSource.Contains("<UserControl", System.StringComparison.Ordinal));
        Assert.IsTrue(model.PersonViewViewModelSource.Contains("class PersonViewViewModel", System.StringComparison.Ordinal));
    }
}
