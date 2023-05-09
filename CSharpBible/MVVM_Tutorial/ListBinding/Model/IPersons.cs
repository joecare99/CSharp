using System.Collections.ObjectModel;

namespace ListBinding.Model
{
    public interface IPersons
    {
        ObservableCollection<Person> Persons { get; }
    }
}