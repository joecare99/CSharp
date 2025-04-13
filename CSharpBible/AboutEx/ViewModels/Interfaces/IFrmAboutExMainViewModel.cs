// ***********************************************************************
// Assembly         : AboutEx
// Author           : Joe care
// Created          : 02-19-2025
//
// Last Modified By : Mir
// Last Modified On : 04-13-2025
// ***********************************************************************
// <copyright file="IFrmAboutExMainViewModel.cs" company="JC-Soft">
//     Copyright (c) 2025 by JC-Soft All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace CSharpBible.AboutEx.ViewModels.Interfaces;

/// <summary>
/// Interface IFrmAboutExMainViewModel
/// Extends the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public interface IFrmAboutExMainViewModel: INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    string Title { get; set; }
    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    /// <value>The version.</value>
    string Version { get; set; }
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    string Description { get; set; }
    /// <summary>
    /// Gets or sets the author.
    /// </summary>
    /// <value>The author.</value>
    string Author { get; set; }
    /// <summary>
    /// Gets or sets the company.
    /// </summary>
    /// <value>The company.</value>
    string Company { get; set; }
    /// <summary>
    /// Gets or sets the copyright.
    /// </summary>
    /// <value>The copyright.</value>
    string Copyright { get; set; }
    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <value>The product.</value>
    string Product { get; set; }

    /// <summary>
    /// Gets or sets the show about FRM1.
    /// </summary>
    /// <value>The show about FRM1.</value>
    Action<string[]>? ShowAboutFrm1 { get; set; }
    /// <summary>
    /// Gets or sets the show about FRM2.
    /// </summary>
    /// <value>The show about FRM2.</value>
    Action<string[]>? ShowAboutFrm2 { get; set; }

    /// <summary>
    /// Gets the show about1 command.
    /// </summary>
    /// <value>The show about1 command.</value>
    IRelayCommand ShowAbout1Command { get; }
    /// <summary>
    /// Gets the show about2 command.
    /// </summary>
    /// <value>The show about2 command.</value>
    IRelayCommand ShowAbout2Command { get; }
}
