using MVVM.ViewModel;
using MVVM_28_DataGrid.Models;
using MVVM_28_DataGrid.Services;
using System.Collections.ObjectModel;

namespace MVVM_28_DataGrid.ViewModels;

public class DataGridViewModel : BaseViewModel
{
    public ObservableCollection<Person> Persons { get; } = new();
    public ObservableCollection<Department> Departments { get; } = new();

    public DelegateCommand RemoveCommand{get; set; }
    public DataGridViewModel() {
        var svc = new PersonService();
        foreach(var person in svc.GetPersons())
            Persons.Add(person);
        foreach (var deprtment in svc.GetDepartments())
            Departments.Add(deprtment);


        RemoveCommand = new DelegateCommand((o) =>
        {
            if (o is Person p)
            {
                Persons.Remove(p);
                RaisePropertyChanged(nameof(Persons));
            }
        });
    }
}
