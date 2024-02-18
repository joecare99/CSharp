// ***********************************************************************
// Assembly         : MVVM_38_CTDependencyInjection
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="TemplateModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace MVVM_38_CTDependencyInjection.Models.Interfaces
{
    public interface IUserRepository
    {
        Guid GetUser(string name, string password);
        IEnumerable<string> GetUsers();
    }
}