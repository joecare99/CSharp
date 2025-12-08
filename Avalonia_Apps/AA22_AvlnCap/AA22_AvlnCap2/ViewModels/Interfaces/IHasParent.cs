// ***********************************************************************
// Assembly         : AA22_AvlnCap2
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="ColData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace AA22_AvlnCap2.ViewModels.Interfaces;

public interface IHasParent<T>
{
    public T? Parent { get; set; }
}