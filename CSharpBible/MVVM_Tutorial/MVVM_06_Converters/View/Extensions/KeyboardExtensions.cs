// ***********************************************************************
// Assembly         : MVVM_06_Converters
// Author           : Mir
// Created          : 07-19-2022
//
// Last Modified By : Mir
// Last Modified On : 07-19-2022
// ***********************************************************************
// <copyright file="KeyboardExtensions.cs" company="MVVM_06_Converters">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using System.Windows.Input;

namespace MVVM_06_Converters.View.Extensions;

/// <summary>
/// Class KeyboardExtensions.
/// </summary>
public static class EnterKeyExtensions
{
    /// <summary>
    /// The enter key command property
    /// </summary>
    public static readonly DependencyProperty EnterKeyCommandProperty = DependencyProperty.RegisterAttached(
        nameof(SetEnterKeyCommand).Replace("Set",""),
        typeof(ICommand),
        typeof(EnterKeyExtensions),
        new PropertyMetadata(
            (d, e) =>
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (e.NewValue != null)
                    ((IInputElement)d).KeyDown += OnKeyDown;
                else
                    ((IInputElement)d).KeyDown -= OnKeyDown;
            }));

    /// <summary>
    /// Handles the <see cref="E:KeyDown" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    private static void OnKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.Enter)
        {
            var command = GetEnterKeyCommand((UIElement)sender);

            if (command?.CanExecute(null) == true)
                command.Execute(null);

            args.Handled = true;
        }
    }

    /// <summary>
    /// Gets the enter key command.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>ICommand.</returns>
    public static ICommand GetEnterKeyCommand(UIElement element) => (ICommand)element.GetValue(EnterKeyCommandProperty);
    /// <summary>
    /// Sets the enter key command.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    public static void SetEnterKeyCommand(UIElement element, ICommand value) => element.SetValue(EnterKeyCommandProperty, value);
}
