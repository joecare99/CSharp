// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia_App02.ViewModels.Interfaces;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace Avalonia_App02.ViewModels;

/// <summary>Class MainWindowViewModel.
/// Implements the <see cref="IMainWindowViewModel" />
/// and the <see cref="ViewModelBase" /></summary>
/// <seealso cref="ViewModelBase" />
/// <seealso cref="IMainWindowViewModel" />
public partial class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    /// <summary>
    /// Gets the greeting.
    /// </summary>
    /// <value>The greeting.</value>
    public string Greeting { get; } = "Welcome to Avalonia!";
}
