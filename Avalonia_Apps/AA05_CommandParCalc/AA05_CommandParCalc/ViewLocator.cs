// ***********************************************************************
// Assembly         : AA05_CommandParCalc
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-11-2025
// ***********************************************************************
// <copyright file="ViewLocator.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AA05_CommandParCalc.ViewModels;

/// <summary>
/// The AA05_CommandParCalc namespace.
/// </summary>
namespace AA05_CommandParCalc;

/// <summary>
/// Class ViewLocator.
/// Implements the <see cref="IDataCommandParCalc" />
/// </summary>
/// <seealso cref="IDataCommandParCalc" />
public class ViewLocator : IDataTemplate
{

    /// <summary>
    /// Creates the control.
    /// </summary>
    /// <param name="param">The parameter.</param>
    /// <returns>The created control.</returns>
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    /// <summary>
    /// Checks to see if this data CommandParCalc matches the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>True if the data CommandParCalc can build a control for the data, otherwise false.</returns>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
