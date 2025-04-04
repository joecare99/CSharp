﻿// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 11-03-2022
//
// Last Modified By : Mir
// Last Modified On : 11-03-2022
// ***********************************************************************
// <copyright file="INotifyPropertyChangedAdv.cs" company="JC-Soft">
//     Copyright (c) by JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Security.Permissions;

/// <summary>
/// The Interfaces namespace.
/// </summary>
/// <autogeneratedoc />
namespace BaseLib.Interfaces
{
    /// <summary>
    /// Class PropertyChangedAdvEventArgs.
    /// Implements the <see cref="EventArgs" />
    /// </summary>
    /// <seealso cref="EventArgs" />
    /// <autogeneratedoc />
#if !NET6_0_OR_GREATER
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
#endif
    public class PropertyChangedAdvEventArgs : EventArgs
    {
        /// <summary>
        /// The property name
        /// </summary>
        /// <autogeneratedoc />
        private readonly string _propertyName;
        /// <summary>
        /// The old value
        /// </summary>
        /// <autogeneratedoc />
        private readonly object? _oldVal;
        /// <summary>
        /// The new value
        /// </summary>
        /// <autogeneratedoc />
        private readonly object? _newVal;

        //
        // Zusammenfassung:
        //     Ruft den Namen der geänderten Eigenschaft ab.
        //
        // Rückgabewerte:
        //     Der Name der geänderten Eigenschaft.
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        /// <autogeneratedoc />
        public virtual string PropertyName => _propertyName;

        //
        // Zusammenfassung:
        //     Ruft den Namen der geänderten Eigenschaft ab.
        //
        // Rückgabewerte:
        //     Der Name der geänderten Eigenschaft.
        /// <summary>
        /// Gets the old value of the changed property.
        /// </summary>
        /// <value>The old value.</value>
        /// <autogeneratedoc />
        public virtual object? OldVal => _oldVal;

        //
        // Zusammenfassung:
        //     Ruft den Namen der geänderten Eigenschaft ab.
        //
        // Rückgabewerte:
        //     Der Name der geänderten Eigenschaft.
        /// <summary>
        /// Gets the new value of the property.
        /// </summary>
        /// <value>The new value.</value>
        public virtual object? NewVal => _newVal;

        //
        // Zusammenfassung:
        //     Initialisiert eine neue Instanz der System.ComponentModel.PropertyChangedAdvEventArgs-Klasse.
        //
        // Parameter:
        //   propertyName:
        //     Der Name der geänderten Eigenschaft.
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedAdvEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        /// <autogeneratedoc />
        public PropertyChangedAdvEventArgs(string propertyName,object? oldVal,object? newVal)
        {
            _propertyName = propertyName;
            _oldVal = oldVal;
            _newVal = newVal;
        }
    }

    /// <summary>
    /// Delegate PropertyChangedAdvEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedAdvEventArgs"/> instance containing the event data.</param>
    /// <autogeneratedoc />
#if !NET6_0_OR_GREATER
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
#endif
    public delegate void PropertyChangedAdvEventHandler(object sender, PropertyChangedAdvEventArgs e);

    //
    // Zusammenfassung:
    //     Benachrichtigt Clients, dass sich ein Eigenschaftswert geändert hat.
    /// <summary>
    /// Interface INotifyPropertyChangedAdv 
    /// the class emits an event to notify other classes that a property in this class has been changed.<br/>
    /// Like <see cref="System.ComponentModel.INotifyPropertyChanged"/> but optionally with the new and the old value of the property.
    /// </summary>
    public interface INotifyPropertyChangedAdv
    {
        //
        // Zusammenfassung:
        //     Tritt ein, wenn sich ein Eigenschaftswert ändert.
        /// <summary>
        /// Occurs when the property is changed.
        /// </summary>
        /// <autogeneratedoc />

        event PropertyChangedAdvEventHandler? PropertyChangedAdv;
    }
    
}
