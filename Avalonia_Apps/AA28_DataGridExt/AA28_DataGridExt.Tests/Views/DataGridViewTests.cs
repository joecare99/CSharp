using AA28_DataGridExt.Model;
using AA28_DataGridExt.ViewModels;
using AA28_DataGridExt.Views;
using Avalonia.Headless.MSTest;
using System.Collections.Generic;

namespace AA28_DataGridExt.Tests.Views;

[TestClass]
public class DataGridViewTests
{
    [AvaloniaTestMethod]
    public void DataGridViewCanBeCreatedTest()
    {
        var view = new DataGridView
        {
            DataContext = new DataGridViewModel(new TestPersonService()),
        };

        Assert.IsNotNull(view);
        Assert.IsNotNull(view.DataContext);
    }

    [AvaloniaTestMethod]
    public void MainViewCanBeCreatedTest()
    {
        var view = new MainView
        {
            DataContext = new MainWindowViewModel(new DataGridViewModel(new TestPersonService())),
        };

        Assert.IsNotNull(view);
        Assert.IsNotNull(view.DataContext);
    }

    private sealed class TestPersonService : IPersonService
    {
        public Department[] GetDepartments() => [];

        public int GetNext(int minimum, int maximum) => minimum;

        public IEnumerable<Person> GetPersons() => [];
    }
}
