using AA28_DataGridExt.Model;
using Avalonia.ViewModels;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AA28_DataGridExt.ViewModels;

/// <summary>
/// Provides the editable grid state and commands.
/// </summary>
public partial class DataGridViewModel : BaseViewModelCT
{
    /// <summary>
    /// Gets the visible persons.
    /// </summary>
    public ObservableCollection<Person> Persons { get; } = [];

    /// <summary>
    /// Gets the available departments.
    /// </summary>
    public ObservableCollection<Department> Departments { get; } = [];

    /// <summary>
    /// Gets the original column names kept for parity with the source sample.
    /// </summary>
    public ObservableCollection<string> Columns { get; } =
    [
        nameof(Person.FirstName),
        nameof(Person.LastName),
        nameof(Person.Email),
        nameof(Person.Department),
        nameof(Person.Birthday),
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsItemSelected))]
    private Person? _selectedPerson;

    /// <summary>
    /// Gets a value indicating whether a person is currently selected.
    /// </summary>
    public bool IsItemSelected => SelectedPerson is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewModel"/> class.
    /// </summary>
    public DataGridViewModel()
        : this(IoC.GetRequiredService<IPersonService>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewModel"/> class.
    /// </summary>
    /// <param name="personService">The person service.</param>
    public DataGridViewModel(IPersonService personService)
    {
        foreach (var department in personService.GetDepartments())
        {
            Departments.Add(department);
        }

        var departmentsById = new Dictionary<int, Department>();
        foreach (var department in Departments)
        {
            departmentsById[department.Id] = department;
        }

        foreach (var person in personService.GetPersons())
        {
            if (person.Department is not null && departmentsById.TryGetValue(person.Department.Id, out var department))
            {
                person.Department = department;
            }

            Persons.Add(person);
        }
    }

    [RelayCommand(CanExecute = nameof(CanRemove))]
    private void Remove(Person? person)
    {
        if (person is null)
        {
            return;
        }

        Persons.Remove(person);

        if (ReferenceEquals(SelectedPerson, person))
        {
            SelectedPerson = null;
        }
    }

    private bool CanRemove(Person? person)
        => person is not null;
}
