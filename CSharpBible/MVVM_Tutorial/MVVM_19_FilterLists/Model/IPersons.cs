using System.Collections.ObjectModel;

namespace MVVM_19_FilterLists.Model;

public interface IPersons
{
    ObservableCollection<Person> Persons { get; }
}