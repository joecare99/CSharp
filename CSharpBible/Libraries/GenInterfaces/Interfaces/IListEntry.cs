// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 09-22-2024
//
// Last Modified By : Mir
// Last Modified On : 09-22-2024
// ***********************************************************************
// <copyright file="IGenTransaction.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace GenFree2Base.Interfaces;

public interface IListEntry<T> where T : IListEntry<T>
{
    IIndexedList<object> Owner { get; }
    T Next { get; }
    T Prev { get; }
    T First { get; }
    T Last { get; }
    bool IsFirst { get; }
    bool IsLast { get; }
}


