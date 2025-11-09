using System.Collections.ObjectModel;

namespace AA19_FilterLists.Model;

public interface IPersons
{
    ObservableCollection<Person> Persons { get; }
}