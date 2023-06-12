// ***********************************************************************
// Assembly         : DemoLibrary
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-14-2022
// ***********************************************************************
// <copyright file="PersonModel.cs" company="DemoLibrary">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace DemoLibrary.Models
{
    /// <summary>
    /// Class PersonModel.
    /// </summary>
    public class PersonModel {
		/// <summary>
		/// Gets or sets the person identifier.
		/// </summary>
		/// <value>The person identifier.</value>
		public int PersonId { get; set; }
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }
		/// <summary>
		/// Gets or sets the first names.
		/// </summary>
		/// <value>The first names.</value>
		public string FirstNames { get; set; }
		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>The last name.</value>
		public string LastName { get; set; }
		/// <summary>
		/// Gets or sets the age.
		/// </summary>
		/// <value>The age.</value>
		public int Age { get; set; }
		/// <summary>
		/// Gets or sets the date of birth.
		/// </summary>
		/// <value>The date of birth.</value>
		public DateTime DateOfBirth { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is alive.
		/// </summary>
		/// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
		public bool IsAlive { get; set; }
		/// <summary>
		/// Gets or sets the account balance.
		/// </summary>
		/// <value>The account balance.</value>
		public decimal AccountBalance { get; set; }
		/// <summary>
		/// The addresses
		/// </summary>
		public List<AddressModel> Addresses = new();
		/// <summary>
		/// Gets or sets the primary address.
		/// </summary>
		/// <value>The primary address.</value>
		public AddressModel? PrimaryAddress { get; set; }
		/// <summary>
		/// Gets the full name.
		/// </summary>
		/// <value>The full name.</value>
		public string FullName => $"{Title} {FirstNames} {LastName}".TrimStart(' ');

	}
}
