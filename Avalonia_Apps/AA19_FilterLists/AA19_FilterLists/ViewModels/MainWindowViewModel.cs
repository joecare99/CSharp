// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 12-24-2021
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using AA19_FilterLists.Properties;

namespace AA19_FilterLists.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="ObservableObject" />
/// </summary>
/// <seealso cref="ObservableObject" />
public class MainWindowViewModel : ObservableObject
{
    public string WindowTitle => Resources.Title;

    public string WindowDescription => Resources.Description;

    public string SampleTabHeader => Resources.Title;

    public string XamlTabHeader => "Xaml";

    public string ViewModelTabHeader => "ViewModel";

    public string XamlDescription => "the Xaml-Code";

    public string ViewModelDescription => "the ViewModel-Code";

    public string PersonViewSource => Resources.PersonView;

    public string PersonViewViewModelSource => Resources.PersonViewViewModel;
}
