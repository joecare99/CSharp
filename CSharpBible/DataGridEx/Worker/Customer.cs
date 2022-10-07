// ***********************************************************************
// Assembly         : DataGridEx
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 04-10-2020
// ***********************************************************************
// <copyright file="Customer.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace DataGridExWPF.Worker
{
    //Defines the customer object
    /// <summary>
    /// Class Customer.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public Uri Email { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is member.
        /// </summary>
        /// <value><c>true</c> if this instance is member; otherwise, <c>false</c>.</value>
        public bool IsMember { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public OrderStatus Status { get; set; }
    }
}
