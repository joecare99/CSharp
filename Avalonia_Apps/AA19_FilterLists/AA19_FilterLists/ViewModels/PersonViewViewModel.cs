// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 12-24-2021
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="PersonViewViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AA19_FilterLists.Model;

namespace AA19_FilterLists.ViewModels;

public class PersonViewViewModel : ObservableObject
{
    private Person newPerson = new Person();
    private readonly IPersons _persons;
    private string _filter;

    public event EventHandler? MissingData;

    public Person NewPerson
    {
        get => newPerson;
        set
        {
            if (ReferenceEquals(value, newPerson)) return;
            SetProperty(ref newPerson, value);
            btnAddPerson?.NotifyCanExecuteChanged();
        }
    }

    public ObservableCollection<Person> Persons => _persons.Persons;
    public ObservableCollection<Person> FilteredPersons { get; } = new();

    public IRelayCommand btnAddPerson { get; }
    public IRelayCommand ClearFilterCommand { get; }

    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
                DoFiltering();
        }
    }

    public PersonViewViewModel() : this(new Persons()) { }
    public PersonViewViewModel(IPersons persons)
    {
        _persons = persons;
        _persons.Persons.CollectionChanged += (_, __) => DoFiltering();
        DoFiltering();
        btnAddPerson = new RelayCommand(AddPerson, CanAddPerson);
        ClearFilterCommand = new RelayCommand(() => Filter = string.Empty);
    }

    private void AddPerson()
    {
        if (newPerson == null || string.IsNullOrWhiteSpace(newPerson.FullName))
        {
            MissingData?.Invoke(this, EventArgs.Empty);
            return;
        }
        _persons.Persons.Add(NewPerson);
        NewPerson.Id = _persons.Persons.IndexOf(newPerson) + 1;
        NewPerson = new Person();
    }
    private bool CanAddPerson() => newPerson?.Id == 0;

    private void DoFiltering()
    {
        FilteredPersons.Clear();
        string value = _filter?.ToLower() ?? "";
        foreach (var person in _persons.Persons)
        {
            if (string.IsNullOrEmpty(value)
                || person.FullName.ToLower().Contains(value)
                || person.Title.ToLower().Contains(value))
                FilteredPersons.Add(person);
        }
    }
}
