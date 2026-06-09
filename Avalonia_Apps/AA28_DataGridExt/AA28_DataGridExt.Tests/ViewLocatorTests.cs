using AA28_DataGridExt.Model;
using AA28_DataGridExt.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System.Collections.Generic;

namespace AA28_DataGridExt.Tests;

[TestClass]
public class ViewLocatorTests
{
    [TestMethod]
    public void SetupCreatesDataTemplateTest()
    {
        var locator = new ViewLocator();

        Assert.IsInstanceOfType(locator, typeof(IDataTemplate));
    }

    [TestMethod]
    public void BuildForUnknownObjectReturnsTextBlockTest()
    {
        var locator = new ViewLocator();

        var result = locator.Build(new object());

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(TextBlock));
    }

    [TestMethod]
    public void MatchRecognizesSupportedViewModelsTest()
    {
        var locator = new ViewLocator();

        Assert.IsTrue(locator.Match(new MainWindowViewModel(new DataGridViewModel(new TestPersonService()))));
        Assert.IsTrue(locator.Match(new DataGridViewModel(new TestPersonService())));
        Assert.IsFalse(locator.Match("test"));
    }

    private sealed class TestPersonService : IPersonService
    {
        public Department[] GetDepartments() => [];

        public int GetNext(int minimum, int maximum) => minimum;

        public IEnumerable<Person> GetPersons() => [];
    }
}
