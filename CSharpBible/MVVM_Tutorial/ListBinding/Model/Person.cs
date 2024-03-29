// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 06-20-2022
// ***********************************************************************
// <copyright file="Person.cs" company="JC-Soft">
//     Copyright � JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using System.Runtime.Serialization;

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
    public class Person : NotificationObject//, ISafeSerializationData
    {
        #region Properties
        #region private properties
        /// <summary>
        /// The identifier
        /// </summary>
        private int _id;
        /// <summary>
        /// The first name
        /// </summary>
        private string _firstName="";
        /// <summary>
        /// The last name
        /// </summary>
        private string _lastName="";
        /// <summary>
        /// The title
        /// </summary>
        private string _title = "";
        #endregion
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id
        {
            get => _id; set => SetProperty(ref _id, value); 
        }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName
        {
            get => _firstName; set => SetProperty(ref _firstName, value,new string[] { nameof(FullName) });
        }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName
        {
            get => _lastName; set => SetProperty(ref _lastName, value, new string[] { nameof(FullName) });
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get => _title; set => SetProperty(ref _title, value, new string[] { nameof(FullName) });
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName => $"{_lastName}{(IsEmpty?"":", ")}{_firstName}{(string.IsNullOrEmpty(Title) ? "" : ", ")}{Title}";

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty =>  string.IsNullOrWhiteSpace(_firstName) && string.IsNullOrEmpty(_lastName);
        #endregion

        #region Methods
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
        public Person(string fullName) 
            : this(fullName.Split(',')[0].Trim(), 
                  fullName.Split(',').Length > 1 ? fullName.Split(',')[1].Trim():"", 
                  fullName.Split(',').Length > 2 ? fullName.Split(',')[2].Trim():"")
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Person" /> class.
        /// </summary>
        public Person() { }

        /// <summary>
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Id}, {FullName}";
        }
        #endregion
    }
}
