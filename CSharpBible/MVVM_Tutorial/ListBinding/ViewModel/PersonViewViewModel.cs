// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 12-24-2021
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="PersonViewViewModel.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using ListBinding.Model;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBinding.ViewModel
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
        /// The persons
        /// </summary>
        private ObservableCollection<Person> persons = new ObservableCollection<Person>();

#if NET5_0_OR_GREATER
        public event EventHandler? MissingData;
#else
        /// <summary>
        /// Occurs when [missing data].
        /// </summary>
        public event EventHandler MissingData;
#endif
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
        public ObservableCollection<Person> Persons { get => persons; set {
                if (value == persons) return;
                persons = value;
                RaisePropertyChanged();
            } }

        /// <summary>
        /// Gets or sets the BTN add person.
        /// </summary>
        /// <value>The BTN add person.</value>
        public DelegateCommand btnAddPerson { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewViewModel" /> class.
        /// </summary>
        public PersonViewViewModel()
        {
            this.btnAddPerson = new DelegateCommand(
                (o) =>
             {
                 if (newPerson == null || newPerson.FullName == "")
                     MissingData?.Invoke(this, new EventArgs());
                 else
                 {
                     persons.Add(NewPerson);
                     NewPerson.Id = persons.IndexOf(newPerson)+1;
                     NewPerson = new Person();
                 }
             },
                (o)=>newPerson?.Id==0 
             );

        }
    }
}
