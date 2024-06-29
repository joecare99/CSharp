using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM.View.Extension;
using MVVM_28_1_CTDataGridExt.Models;
using MVVM_28_1_CTDataGridExt.Services;
using System.Collections.ObjectModel;

namespace MVVM_28_1_CTDataGridExt.ViewModels
{
    public partial class DataGridViewModel : BaseViewModelCT
    {
        public ObservableCollection<Person> Persons { get; } = new();
        public ObservableCollection<Department> Departments { get; } = new();

        public ObservableCollection<string> Cols { get; set; } = new() { "FirstName","LastName","Email","Department","Birthday" };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsItemSelected))]
        private Person? _selectedPerson;

        public bool IsItemSelected => SelectedPerson != null;

        public DataGridViewModel() : this(IoC.GetRequiredService<IPersonService>()){}
 
        public DataGridViewModel(IPersonService svc) {
            foreach(var person in svc.GetPersons())
                Persons.Add(person);
            foreach (var deprtment in svc.GetDepartments())
                Departments.Add(deprtment);
        }

        [RelayCommand]
        private void Remove(object? o)
        {
            if (o is Person p)
            {
                Persons.Remove(p);
            }
        }
    }
}
