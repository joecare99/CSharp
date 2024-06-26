// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="DelegateCommand.cs" company="MVVM_BaseLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace MVVM.ViewModel;

/// <summary>
/// Class DelegateCommand.
/// Implements the <see cref="IRelayCommand" />
/// Implements the <see cref="ICommand" />
/// </summary>
/// <seealso cref="IRelayCommand" />
/// <seealso cref="ICommand" />
public class DelegateCommand : DelegateCommand<object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    /// </summary>
    /// <param name="execute">The execute.</param>
    /// <param name="canExecute">The can execute.</param>
    public DelegateCommand(Action<object?> execute, Predicate<object?>? canExecute) : base(execute, canExecute)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    /// </summary>
    /// <param name="execute">The execute.</param>
    public DelegateCommand(Action<object?> execute) : base(execute) { }
}

/// <summary>
/// Class DelegateCommand.
/// Implements the <see cref="IRelayCommand" />
/// Implements the <see cref="ICommand" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="IRelayCommand" />
/// <seealso cref="ICommand" />
public class DelegateCommand<T> : IRelayCommand, ICommand
{
    /// <summary>
    /// The execute
    /// </summary>
    readonly Action<T?> execute;
    /// <summary>
    /// The can execute
    /// </summary>
    readonly Predicate<T?>? canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execute.</param>
    /// <param name="canExecute">The can execute.</param>
    public DelegateCommand(Action<T?> execute, Predicate<T?>? canExecute) =>
        (this.canExecute, this.execute) = (canExecute, execute);
    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execute.</param>
    public DelegateCommand(Action<T?> execute) : this(execute, null) { }

    /// <summary>
    /// Tritt ein, wenn Änderungen auftreten, die sich auf die Ausführung des Befehls auswirken.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Definiert die Methode, die bestimmt, ob der Befehl im aktuellen Zustand ausgeführt werden kann.
    /// </summary>
    /// <param name="parameter">Vom Befehl verwendete Daten.  Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf <see langword="null" /> festgelegt werden.</param>
    /// <returns><see langword="true" />, wenn der Befehl ausgeführt werden kann, andernfalls <see langword="false" />.</returns>
    public bool CanExecute(object? parameter)
        => this.canExecute?.Invoke((T?)parameter) ?? true;
    /// <summary>
    /// Definiert die Methode, die aufgerufen wird, wenn der Befehl aufgerufen wird.
    /// </summary>
    /// <param name="parameter">Vom Befehl verwendete Daten.  Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf <see langword="null" /> festgelegt werden.</param>
    public void Execute(object? parameter)
        => this.execute.Invoke((T?)parameter);

    /// <summary>
    /// Notifies that the <see cref="M:System.Windows.Input.ICommand.CanExecute(System.Object)" /> property has changed.
    /// </summary>
    public void NotifyCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
