using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AA19_FilterLists.Model;

namespace AA19_FilterLists.ViewModels.Tests;

public sealed class TestPersons : IPersonDataService
{
    private readonly ObservableCollection<Person> _persons;

    public TestPersons(params Person[] persons)
    {
        _persons = new ObservableCollection<Person>(persons);
        Persons = new ReadOnlyObservableCollection<Person>(_persons);
    }

    public ReadOnlyObservableCollection<Person> Persons { get; }

    public event NotifyCollectionChangedEventHandler? PersonsChanged;

    public void AddPerson(Person person)
    {
        person.Id = _persons.Count + 1;
        _persons.Add(person);
        PersonsChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, person));
    }
}
