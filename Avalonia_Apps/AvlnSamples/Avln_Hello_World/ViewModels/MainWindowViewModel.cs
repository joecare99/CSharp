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
using System;
using System.IO;
using System.Reflection;
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
 
    /// <summary>
    /// Gets the HelloWorldView AXAML code for display
    /// </summary>
    public string HelloWorldViewCode => LoadEmbeddedResource("Avln_Hello_World.Views.HelloWorldView.axaml");
    
    /// <summary>
 /// Gets the HelloWorldViewModel C# code for display
    /// </summary>
    public string HelloWorldViewModelCode => LoadEmbeddedResource("Avln_Hello_World.ViewModels.HelloWorldViewModel.cs");
    
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
    
    /// <summary>
    /// Loads an embedded resource as string
    /// </summary>
    /// <param name="resourceName">Full resource name</param>
    /// <returns>Resource content as string</returns>
    private static string LoadEmbeddedResource(string resourceName)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
    using var stream = assembly.GetManifestResourceStream(resourceName);
          if (stream == null)
        return $"Resource not found: {resourceName}";
       
       using var reader = new StreamReader(stream);
  return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            return $"Error loading resource: {ex.Message}";
        }
    }
    
    #endregion
}
