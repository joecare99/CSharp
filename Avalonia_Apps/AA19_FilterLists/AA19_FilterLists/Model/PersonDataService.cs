using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace AA19_FilterLists.Model;

public class PersonDataService : IPersonDataService
{
    private readonly ObservableCollection<Person> _persons;

    public PersonDataService()
        : this(CreateDefaultPersons())
    {
    }

    private PersonDataService(IEnumerable<Person> persons)
    {
        _persons = new ObservableCollection<Person>(persons);
        _persons.CollectionChanged += (_, e) => PersonsChanged?.Invoke(this, e);
        Persons = new ReadOnlyObservableCollection<Person>(_persons);
    }

    public ReadOnlyObservableCollection<Person> Persons { get; }

    public event NotifyCollectionChangedEventHandler? PersonsChanged;

    public void AddPerson(Person person)
    {
        person.Id = _persons.Count == 0 ? 1 : _persons.Max(p => p.Id) + 1;
        _persons.Add(person);
    }

    private static IEnumerable<Person> CreateDefaultPersons()
    {
        yield return new Person("Mustermann", "Max", "Dr.") { Id = 1 };
        yield return new Person("Musterfrau", "Anna", "Prof.") { Id = 2 };
        yield return new Person("Meier", "Hans", "Dr.") { Id = 3 };
        yield return new Person("Schmidt", "Julia", "Prof.") { Id = 4 };
    }
}
