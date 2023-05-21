﻿// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork
// Author           : Mir
// Created          : 05-20-2023
//
// Last Modified By : Mir
// Last Modified On : 05-20-2023
// ***********************************************************************
// <copyright file="UserRepository.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_36_ComToolKtSavesWork.Models
{
    /// <summary>
    /// Class UserRepository.
    /// Implements the <see cref="MVVM_36_ComToolKtSavesWork.Models.IUserRepository" />
    /// </summary>
    /// <seealso cref="MVVM_36_ComToolKtSavesWork.Models.IUserRepository" />
    /// <autogeneratedoc />
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Logins the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>System.Nullable&lt;User&gt;.</returns>
        /// <autogeneratedoc />
        public User? Login(string username, string password) => new() { Username = "DevDave", Id = 1 };
    }
}
