using BaseLib.Models;
using MVVM.ViewModel;
using MVVM_28_1_DataGridExt.Models;
using MVVM_28_1_DataGridExt.Services;
using System.Collections.ObjectModel;

namespace MVVM_28_1_DataGridExt.ViewModels;

public class DataGridViewModel : BaseViewModel
{
    public ObservableCollection<Person> Persons { get; } = new();
    public ObservableCollection<Department> Departments { get; } = new();

    public ObservableCollection<string> Cols { get; set; } = new() { "FirstName","LastName","Email","Department","Birthday" };
    public DelegateCommand RemoveCommand{get; set; }

    Person? selectedPerson;

    public Person? SelectedPerson { get => selectedPerson; set => SetProperty(ref selectedPerson, value); }

    public bool IsItemSelected => SelectedPerson != null;
    public DataGridViewModel() {
        var svc = new PersonService(
            new CRandom()); 
        foreach(var person in svc.GetPersons())
            Persons.Add(person);
        foreach (var deprtment in svc.GetDepartments())
            Departments.Add(deprtment);

        RemoveCommand = new DelegateCommand(Remove);

        AddPropertyDependency(nameof(IsItemSelected), nameof(SelectedPerson));
    }

    private void Remove(object? o)
    {
        if (o is Person p)
        {
            Persons.Remove(p);
            RaisePropertyChanged(nameof(Persons));
        }
    }
}
