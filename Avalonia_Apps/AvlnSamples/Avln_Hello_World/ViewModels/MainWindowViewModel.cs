// ***********************************************************************
// Assembly  : Avln_Hello_World
// Author  : Mir
// Created   : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.DependencyInjection;
using Avalonia.ViewModels;
using Avln_Hello_World.Models.Interfaces;

namespace Avln_Hello_World.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public class MainWindowViewModel : BaseViewModelCT
{
    private IHelloWorldModel? model;
 
    #region Properties
 #endregion
    
    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
  public MainWindowViewModel() : this(Ioc.Default.GetService<IHelloWorldModel>())
    {
 }

  public MainWindowViewModel(IHelloWorldModel? model)
    {
      this.model = model;
    }

    internal void Closing()
    {
        model?.ClosingCommand.Execute(null);
    }
    #endregion
}
