using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AA19_FilterLists.Model;

public interface IPersonDataService
{
    ReadOnlyObservableCollection<Person> Persons { get; }

    event NotifyCollectionChangedEventHandler? PersonsChanged;

    void AddPerson(Person person);
}
