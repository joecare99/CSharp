// ***********************************************************************
// Assembly  : Avln_Hello_World
// Author  : Mir
// Created  : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="HelloWorldViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.ViewModels;
using System.ComponentModel;
using Avln_Hello_World.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Avln_Hello_World.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace Avln_Hello_World.ViewModels;

/// <summary>
/// Class HelloWorldViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class HelloWorldViewModel : BaseViewModelCT
{
    #region Properties
    private IHelloWorldModel model;

    [ObservableProperty]
    private string _greeting = string.Empty;
    #endregion

    #region Methods
    public HelloWorldViewModel() : this(Ioc.Default.GetRequiredService<IHelloWorldModel>()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HelloWorldViewModel"/> class.
    /// </summary>
    public HelloWorldViewModel(IHelloWorldModel model)
    {
        this.model = model;
        model.PropertyChanged += Model_PropertyChanged;
    }

    private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Greeting))
            Greeting = Resources.ResourceManager.GetString($"{model.Greeting}Text") ?? string.Empty;
    }

    #endregion
}
