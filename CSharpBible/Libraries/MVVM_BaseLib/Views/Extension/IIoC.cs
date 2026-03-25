// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 05-20-2023
//
// Last Modified By : Mir
// Last Modified On : 09-26-2023
// ***********************************************************************
// <copyright file="IoC.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MVVM.Views.Extension;

/// <summary>
/// Defines a contract for Inversion of Control (IoC) container integration with XAML markup extensions.
/// </summary>
public interface IIoC
{
    /// <summary>
    /// Gets or sets the type to be resolved from the IoC container.
    /// </summary>
    /// <value>The type to resolve.</value>
    Type Type { get; set; }

    /// <summary>
    /// Provides the resolved value from the IoC container.
    /// </summary>
    /// <param name="serviceProvider">The service provider that can provide services for the markup extension.</param>
    /// <returns>The resolved object instance, or <c>null</c> if the type cannot be resolved.</returns>
    object? ProvideValue(IServiceProvider serviceProvider);
}