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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_19_FilterLists.ViewModel
{
    /// <summary>
    /// Class PersonViewViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class PersonViewViewModel : BaseViewModel
    {
        /// <summary>
        /// The new person
        /// </summary>
        private Person newPerson = new Person();

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

        private IPersons _persons;

        /// <summary>
        /// Gets or sets the BTN add person.
        /// </summary>
        /// <value>The BTN add person.</value>
        public DelegateCommand btnAddPerson { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewViewModel" /> class.
        /// </summary>
        public PersonViewViewModel() : this(new Persons()) { }
        public PersonViewViewModel(IPersons persons)
        {
            _persons = persons;
            this.btnAddPerson = new DelegateCommand(
                (o) =>
             {
                 if (newPerson == null || newPerson.FullName == "")
                     MissingData?.Invoke(this, new EventArgs());
                 else
                 {
                     _persons.Persons.Add(NewPerson);
                     NewPerson.Id = _persons.Persons.IndexOf(newPerson)+1;
                     NewPerson = new Person();
                 }
             },
                (o)=>newPerson?.Id==0 
             );

        }
    }
}
