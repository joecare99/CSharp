using BaseLib.Helper;
using BaseLib.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AA28_DataGridExt.Model;

/// <summary>
/// Creates deterministic sample-like persons and departments for the UI.
/// </summary>
public class PersonService : IPersonService
{
    private static readonly string[] FirstNames =
    {
        "Andrew", "Bob", "Carla", "Dany", "Earl", "Frank", "Georgina", "Henry", "Inez", "John",
        "Karl", "Lenny", "Monique", "Norbert", "Oscar", "Paula", "Quentin", "Richard", "Steve", "Theodor",
        "Urban", "Victor", "Walter", "Xavier", "Yvonne", "Zaharias"
    };

    private static readonly string[] LastNames =
    {
        "Smith", "Jones", "Garcia", "Hernandez", "Miller", "Santiago", "Thomas", "Lee", "Taylor", "Widmark"
    };

    private static readonly DateTime LowEndDate = new(1953, 1, 1);
    private static readonly int DaysFromLowDate = (DateTime.Today - LowEndDate).Days - 7000;

    private readonly IRandom _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonService"/> class.
    /// </summary>
    public PersonService()
        : this(IoC.GetRequiredService<IRandom>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonService"/> class.
    /// </summary>
    /// <param name="random">The random source used for sample generation.</param>
    public PersonService(IRandom random)
    {
        _random = random;
    }

    /// <inheritdoc />
    public int GetNext(int minimum, int maximum)
        => _random.Next(minimum, maximum);

    /// <inheritdoc />
    public IEnumerable<Person> GetPersons()
    {
        var departments = GetDepartments();
        var nextId = 5;

        return
        [
            new Person { FirstName = "Max", LastName = "Muster", Email = "max@muster.com", Id = 1, Department = departments[0] },
            new Person { FirstName = "Susi", LastName = "Müller", Email = "susi@muster.com", Id = 2, Department = departments[1], Birthday = new DateTime(1980, 1, 1) },
            new Person { FirstName = "Dave", LastName = "Dev", Email = "dev.dave@muster.com", Id = 3, Department = departments[3], Birthday = new DateTime(1988, 5, 2) },
            new Person { FirstName = "Herbert", LastName = "Bossinger", Email = "ceo@muster.com", Id = 4, Department = departments[2], Birthday = new DateTime(1999, 7, 7) },
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
            CreateRandomPerson(ref nextId, departments),
        ];
    }

    /// <inheritdoc />
    public Department[] GetDepartments()
        =>
        [
            new Department { Id = 1, Name = "Management", Description = "Leads the organization and coordinates strategy." },
            new Department { Id = 2, Name = "Sales", Description = "Maintains customer relationships and revenue growth." },
            new Department { Id = 3, Name = "IT", Description = "Builds and supports the technical landscape." },
            new Department { Id = 4, Name = "Development", Description = "Implements software features and fixes." },
        ];

    private T GetRandomItem<T>(T[] items)
        => items[GetNext(0, items.Length)];

    private DateTime GetRandomDate()
        => LowEndDate.AddDays(GetNext(0, DaysFromLowDate));

    private Person CreateRandomPerson(ref int nextId, Department[] departments)
    {
        var firstName = GetRandomItem(FirstNames);
        var lastName = GetRandomItem(LastNames);

        return new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Email = $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}@muster.com",
            Id = nextId++,
            Department = GetRandomItem(departments),
            Birthday = GetRandomDate(),
        };
    }
}
