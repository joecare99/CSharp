// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 06-20-2022
// ***********************************************************************
// <copyright file="Person.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

namespace AA19_FilterLists.Model;

public class Person : ObservableObject
{
    #region Properties
    private int _id;
    private string _firstName = "";
    private string _lastName = "";
    private string _title = "";

    public int Id
    {
        get => _id; set => SetProperty(ref _id, value);
    }
    public string FirstName
    {
        get => _firstName; set => SetProperty(ref _firstName, value, nameof(FullName));
    }
    public string LastName
    {
        get => _lastName; set => SetProperty(ref _lastName, value, nameof(FullName));
    }
    public string Title
    {
        get => _title; set => SetProperty(ref _title, value, nameof(FullName));
    }

    public string FullName => $"{_lastName}{(IsEmpty ? "" : ", ")}{_firstName}{(string.IsNullOrEmpty(Title) ? "" : ", ")}{Title}";

    public bool IsEmpty => string.IsNullOrWhiteSpace(_firstName) && string.IsNullOrEmpty(_lastName);
    #endregion

    #region Methods
    public Person(string lastName, string firstName, string title = "")
    {
        (FirstName, LastName, Title) = (firstName, lastName, title);
    }

    public Person(string fullName) : this(fullName.Split(',')[0].Trim(), fullName.Split(',').Length >1 ? fullName.Split(',')[1].Trim() : "", fullName.Split(',').Length >2 ? fullName.Split(',')[2].Trim() : "")
    {
    }

    public Person() { }

    public override string ToString()
    {
        return $"{Id}, {FullName}";
    }
    #endregion
}
