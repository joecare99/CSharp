// ***********************************************************************
// Assembly         : MVVM_25_RichTextEdit
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_25_RichTextEdit.Models;
using System;
using System.ComponentModel;

namespace MVVM_25_RichTextEdit.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class RichTextEditViewModel : BaseViewModelCT
{
    #region Properties
    private readonly IRichTextEditModel _model;

    public DateTime Now => _model.Now;

    [ObservableProperty]
    private string _document = @"<FlowDocument PagePadding=""5,0,5,0"" AllowDrop=""True"" NumberSubstitution.CultureSource=""User"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
<Paragraph><Run FontWeight=""Bold""><Run.TextDecorations><TextDecoration Location=""Underline"" /></Run.TextDecorations>Model–view–viewmodel</Run>
<Run FontWeight=""Bold"" xml:lang=""de-de"" xml:space=""preserve""><Run.TextDecorations><TextDecoration Location=""Underline"" /></Run.TextDecorations> </Run>
<Run FontStyle=""Italic"" xml:lang=""de-de"">(from Wikipedia)</Run></Paragraph><Paragraph>(Redirected from MVVM)<LineBreak />
<Run xml:lang=""de-de"">[...]</Run></Paragraph>
<Paragraph><Run FontWeight=""Bold""><Run.TextDecorations><TextDecoration Location=""Underline"" /></Run.TextDecorations>Components of MVVM pattern</Run></Paragraph>
<Paragraph><Run FontWeight=""Bold"">Model</Run></Paragraph><Paragraph xml:space=""preserve"">    Model refers either to a domain model, which represents real state content 
(an object-oriented approach), or to the data access layer, which represents content (a data-centric approach).[citation needed]</Paragraph>
<Paragraph><Run FontWeight=""Bold"">View</Run></Paragraph>
<Paragraph xml:space=""preserve"">    As in the model–view–controller (MVC) and model–view–presenter (MVP) patterns, the view is the structure, layout, and appearance of what 
a user sees on the screen.[7] It displays a representation of the model and receives the user's interaction with the view (mouse clicks, keyboard input, screen tap gestures, 
etc.), and it forwards the handling of these to the view model via the data binding (properties, event callbacks, etc.) that is defined to link the view and view model.</Paragraph>
<Paragraph><Run FontWeight=""Bold"">View model</Run></Paragraph>
<Paragraph TextAlignment=""Justify"" xml:space=""preserve"">    The view model is an abstraction of the view exposing public properties and commands. Instead of the controller of the 
MVC pattern, or the presenter of the MVP pattern, MVVM has a binder, which automates communication between the view and its bound properties in the view model. The view model has been 
described as a state of the data in the model.[8]</Paragraph>
<Paragraph TextAlignment=""Justify"" xml:space=""preserve"">    The main difference between the view model and the Presenter in the MVP pattern is that the presenter has a reference 
to a view, whereas the view model does not. Instead, a view directly binds to properties on the view model to send and receive updates. To function efficiently, this requires a 
binding technology or generating boilerplate code to do the binding.[7]</Paragraph>
<Paragraph TextAlignment=""Justify"" xml:space=""preserve"">    Under object-oriented programming, the view model can sometimes be referred to as a data transfer object.[9]</Paragraph>
<Paragraph><Run FontWeight=""Bold"">Binder</Run></Paragraph>
<Paragraph xml:space=""preserve"">    Declarative data and command-binding are implicit in the MVVM pattern. In the Microsoft solution stack, the binder is a markup language called 
XAML.[10] The binder frees the developer from being obliged to write boiler-plate logic to synchronize the view model and view. When implemented outside of the Microsoft stack, the 
presence of a declarative data binding technology is what makes this pattern possible,[5][11] and without a binder, one would typically use MVP or MVC instead and have to write more 
boilerplate (or generate it with some other tool).</Paragraph></FlowDocument>";
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public RichTextEditViewModel():this(IoC.GetRequiredService<IRichTextEditModel>())
    {
    }

    public RichTextEditViewModel(IRichTextEditModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
    }

    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName); 
    }

    #endregion
}
