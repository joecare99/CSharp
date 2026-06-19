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

public partial class PersonViewViewModel : ObservableObject
{
    private readonly IPersonDataService _personDataService;
    private string _filter = string.Empty;

    public event EventHandler? MissingData;

    [ObservableProperty]
    public partial Person ActPerson { get; set; } = new Person();

    public ReadOnlyObservableCollection<Person> Persons => _personDataService.Persons;
    public ObservableCollection<Person> FilteredPersons { get; } = new();

    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
                DoFiltering();
        }
    }

//    public PersonViewViewModel() : this(new PersonDataService()) { }
    public PersonViewViewModel(IPersonDataService personDataService)
    {
        _personDataService = personDataService;
        _personDataService.PersonsChanged += (_, __) => DoFiltering();
        DoFiltering();
    }

    [RelayCommand]
    private void NewPerson()
    {
        ActPerson = new Person();
    }

    [RelayCommand]
    private void ClearFilter()
    {
        Filter = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanAddPerson))]
    private void AddPerson()
    {
        if (ActPerson == null || string.IsNullOrWhiteSpace(ActPerson.FullName))
        {
            MissingData?.Invoke(this, EventArgs.Empty);
            return;
        }
        _personDataService.AddPerson(ActPerson);
        ActPerson = new Person();
    }

    private bool CanAddPerson() => ActPerson?.Id == 0;

    private void DoFiltering()
    {
        FilteredPersons.Clear();
        string value = _filter?.ToLower() ?? "";
        foreach (var person in _personDataService.Persons)
        {
            if (string.IsNullOrEmpty(value)
                || person.FullName.ToLower().Contains(value)
                || person.Title.ToLower().Contains(value))
                FilteredPersons.Add(person);
        }
    }
}
