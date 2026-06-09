using AA28_DataGridExt.Model;
using AA28_DataGridExt.ViewModels;
using System.Collections.Generic;

namespace AA28_DataGridExt.Tests.ViewModels;

[TestClass]
public class MainWindowViewModelTests
{
    [TestMethod]
    public void ConstructorUsesProvidedDataGridViewModelTest()
    {
        var dataGrid = new DataGridViewModel(new TestPersonService());
        var viewModel = new MainWindowViewModel(dataGrid);

        Assert.AreSame(dataGrid, viewModel.DataGrid);
    }

    private sealed class TestPersonService : IPersonService
    {
        public Department[] GetDepartments() => [];

        public int GetNext(int minimum, int maximum) => minimum;

        public IEnumerable<Person> GetPersons() => [];
    }
}
