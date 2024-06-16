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
using MVVM_19_FilterLists.Model;
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace MVVM_19_FilterLists.ViewModel
{
    /// <summary>
    /// Class PersonViewViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class PersonViewViewModel : BaseViewModel
    {
        #region Properties
        /// <summary>
        /// The new person
        /// </summary>
        private Person newPerson = new Person();

        private IPersons _persons;
        private string _filter;

        /// <summary>
        /// Occurs when [missing data].
        /// </summary>
        public event EventHandler? MissingData;

        /// <summary>
        /// Creates new person.
        /// </summary>
        /// <value>The new person.</value>
        public Person NewPerson
        {
            get => newPerson;
            set
            {
                if (value == newPerson) return;
                newPerson = value;
                RaisePropertyChanged();
                btnAddPerson.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the persons.
        /// </summary>
        /// <value>The persons.</value>
        public ObservableCollection<Person> Persons { get => _persons.Persons; }
        public ObservableCollection<Person> FilteredPersons { get; set; } = new();

        /// <summary>
        /// Gets or sets the BTN add person.
        /// </summary>
        /// <value>The BTN add person.</value>
        public DelegateCommand btnAddPerson { get; set; }


        public string Filter { get => _filter; set => SetProperty(ref _filter , value,(s,n)=>DoFiltering()); }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewViewModel" /> class.
        /// </summary>
        public PersonViewViewModel() : this(new Persons()) { }
        public PersonViewViewModel(IPersons persons)
        {
            _persons = persons;
            _persons.Persons.CollectionChanged += (s,e)=>DoFiltering();
            DoFiltering();
            this.btnAddPerson = new DelegateCommand(
                (o) =>
             {
                 if (newPerson == null || newPerson.FullName == "")
                     MissingData?.Invoke(this, new EventArgs());
                 else
                 {
                     _persons.Persons.Add(NewPerson);
                     NewPerson.Id = _persons.Persons.IndexOf(newPerson) + 1;
                     NewPerson = new Person();
                 }
             },
                (o) => newPerson?.Id == 0
             );

        }

        private void DoFiltering()
        {
            FilteredPersons.Clear();
            string value = _filter?.ToLower() ?? "";
            foreach (Person person in _persons.Persons)
            {
                if (string.IsNullOrEmpty(value)
                    || person.FullName.ToLower().Contains(value)
                    || person.Title.ToLower().Contains(value)
                    //       || person.Job.ToLower().Contains(value)
                    )
                    FilteredPersons.Add(person);
            } 
        }
        #endregion
    }
}
