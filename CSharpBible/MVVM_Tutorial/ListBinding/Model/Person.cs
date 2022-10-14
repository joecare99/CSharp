// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 06-20-2022
// ***********************************************************************
// <copyright file="Person.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ListBinding.Model
{
    /// <summary>
    /// Class Person.
    /// Implements the <see cref="BaseViewModel" />
    /// Implements the <see cref="ISafeSerializationData" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    /// <seealso cref="ISafeSerializationData" />
    [Serializable]
    public class Person : BaseViewModel, ISafeSerializationData
    {
        /// <summary>
        /// The identifier
        /// </summary>
        private int id;
        /// <summary>
        /// The first name
        /// </summary>
        private string firstName;
        /// <summary>
        /// The last name
        /// </summary>
        private string lastName;
        /// <summary>
        /// The title
        /// </summary>
        private string title;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id
        {
            get => id; set
            {
                if (id == value) return;
                id = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName
        {
            get => firstName; set
            {
                if (firstName == value) return;
                firstName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName
        {
            get => lastName; set
            {
                if (lastName == value) return;
                lastName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }
        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName => $"{lastName}{(IsEmpty?"":", ")}{firstName}{(string.IsNullOrEmpty(Title) ? "" : ", ")}{Title}";
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get => title; set
            {
                if (title == value) return;
                title = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Id}, {FullName}";
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty =>  string.IsNullOrWhiteSpace(firstName) && string.IsNullOrEmpty(lastName);
        /// <summary>
        /// Initializes a new instance of the <see cref="Person" /> class.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="title">The title.</param>
        public Person(string lastName, string firstName, string title="")
        {
            (FirstName, LastName, Title) = (firstName, lastName, title);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Person" /> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        public Person(string fullName) : this(fullName.Split(',')[0].Trim(), fullName.Split(',').Length > 1 ? fullName.Split(',')[1].Trim():"", fullName.Split(',').Length>2? fullName.Split(',')[2].Trim():"")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Person" /> class.
        /// </summary>
        public Person() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Person" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public Person(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            FirstName = info.GetString(nameof(FirstName));
            LastName = info.GetString(nameof(LastName));
            Title = info.GetString(nameof(Title));
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id, typeof(int));
            info.AddValue(nameof(FirstName), FirstName, typeof(string));
            info.AddValue(nameof(LastName), LastName, typeof(string));
            info.AddValue(nameof(Title), Title, typeof(string));
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn die Instanz deserialisiert wird.
        /// </summary>
        /// <param name="deserialized">Ein Objekt, das den gespeicherten Zustand der Instanz enthält.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CompleteDeserialization(object deserialized)
        {
            throw new NotImplementedException();
        }
    }
}
