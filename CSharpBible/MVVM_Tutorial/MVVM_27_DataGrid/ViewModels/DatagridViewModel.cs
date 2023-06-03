using MVVM.ViewModel;
using MVVM_27_DataGrid.Models;
using MVVM_27_DataGrid.Services;
using System.Collections.ObjectModel;

namespace MVVM_27_DataGrid.ViewModels
{
    public class DataGridViewModel : BaseViewModel
    {
        public ObservableCollection<Person> Persons { get; set; } = new();
        public ObservableCollection<Department> Departments { get; } = new();
        public DataGridViewModel() {
            var svc = new PersonService();
            foreach(var person in svc.GetPersons())
                Persons.Add(person);
            foreach (var deprtment in svc.GetDepartments())
                Departments.Add(deprtment);
        }
    }
}
