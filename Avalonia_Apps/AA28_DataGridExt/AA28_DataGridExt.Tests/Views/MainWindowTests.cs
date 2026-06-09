using AA28_DataGridExt.Model;
using AA28_DataGridExt.ViewModels;
using AA28_DataGridExt.Views;
using Avalonia.Headless.MSTest;
using System.Collections.Generic;

namespace AA28_DataGridExt.Tests.Views;

[TestClass]
public class MainWindowTests
{
    [AvaloniaTestMethod]
    public void MainWindowCanBeCreatedTest()
    {
        var window = new MainWindow
        {
            DataContext = new MainWindowViewModel(new DataGridViewModel(new TestPersonService())),
        };

        window.Show();

        Assert.IsNotNull(window);
        Assert.IsNotNull(window.Content);
    }

    private sealed class TestPersonService : IPersonService
    {
        public Department[] GetDepartments() => [];

        public int GetNext(int minimum, int maximum) => minimum;

        public IEnumerable<Person> GetPersons() => [];
    }
}
