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
/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace Avalonia_App02.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="Avalonia_App02.ViewModels.ViewModelBase" />
/// </summary>
/// <seealso cref="Avalonia_App02.ViewModels.ViewModelBase" />
public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the greeting.
    /// </summary>
    /// <value>The greeting.</value>
    public string Greeting { get; } = "Welcome to Avalonia!";
}
