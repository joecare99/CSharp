// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 08-19-2022
//
// Last Modified By : Mir
// Last Modified On : 08-20-2022
// ***********************************************************************
// <copyright file="NotificationObject.cs" company="MVVM_BaseLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Avalonia.ViewModels;

/// <summary>
/// Class NotificationObject.
/// Implements the <see cref="ObservableObject" />
/// </summary>
/// <seealso cref="ObservableObject" />
public class NotificationObjectCT : ObservableObject
{
		
    /// <summary>
    /// Helper for setting properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">The data.</param>
    /// <param name="value">The value.</param>
    /// <param name="action">The action.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool ExecPropSetter<T>(Action<T> setter, T data, T value, Action<T, T>? action, [CallerMemberName] string propertyName = "")
        => ExecPropSetter(setter,data, value, null, null, action, propertyName);

    /// <summary>
    /// Helper for setting properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">The data.</param>
    /// <param name="value">The value.</param>
    /// <param name="propertyNames">The property names.</param>
    /// <param name="action">The action.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		protected bool ExecPropSetter<T>(Action<T> setter,T data, T value, string[]? propertyNames = null, Predicate<T>? validate = null, Action<T, T>? action = null, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(data, value)) return false;
        if (validate?.Invoke(value) == false) return false;
        T old = data;
        OnPropertyChanging(propertyName);
        foreach (var pn in propertyNames ?? Array.Empty<string>())
            OnPropertyChanging(pn);
        setter(value);
        OnPropertyChanged(propertyName);
        foreach (var pn in propertyNames ?? Array.Empty<string>())
            OnPropertyChanged(pn);
        try
        { action?.Invoke(old, value); }
        catch { }
        return true;
    }
}
