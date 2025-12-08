// ***********************************************************************
// Assembly         : Avalonia_App02
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
using Avalonia_App02.ViewModels;

/// <summary>
/// The Avalonia_App02 namespace.
/// </summary>
namespace Avalonia_App02;

/// <summary>
/// Class ViewLocator.
/// Implements the <see cref="IDataTemplate" />
/// </summary>
/// <seealso cref="IDataTemplate" />
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
    /// Checks to see if this data template matches the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>True if the data template can build a control for the data, otherwise false.</returns>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
