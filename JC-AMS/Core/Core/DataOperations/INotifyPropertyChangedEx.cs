﻿// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 10-09-2022
//
// Last Modified By : Mir
// Last Modified On : 10-09-2022
// ***********************************************************************
// <copyright file="INotifyPropertyChangedEx.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary>Advanced interface for notifocation of property changes</summary>
// ***********************************************************************

using System.ComponentModel;

/// <summary>
/// The DataOperations namespace.
/// </summary>
/// <autogeneratedoc />
namespace JCAMS.Core.DataOperations
{
    /// <summary>Interface INotifyPropertyChangedEx
    /// <br /> "Extended on property changed notification"</summary>
    /// <remarks>The extended notification contains the old, the new value and the name of the property</remarks>
    /// <autogeneratedoc />
    public interface INotifyPropertyChangedEx
    {
        /// <summary>
        /// Occurs when [the property is changed] extended.
        /// </summary>
        /// <autogeneratedoc />
        event PropertyChangedExEventHandler OnPropertyChangedEx;
    }

    /// <summary>
    /// Delegate PropertyChangedExEventHandler
    /// </summary>
    /// <param name="Sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedExEventArgs"/> instance containing the event data.</param>
    /// <autogeneratedoc />
    public delegate void PropertyChangedExEventHandler(object Sender, PropertyChangedExEventArgs e);

    /// <summary>
    /// Class PropertyChangedExEventArgs.
    /// Implements the <see cref="PropertyChangedEventArgs" />
    /// </summary>
    /// <seealso cref="PropertyChangedEventArgs" />
    /// <autogeneratedoc />
    public class PropertyChangedExEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// The old value
        /// </summary>
        /// <autogeneratedoc />
        private object oldVal;
        /// <summary>
        /// The new value
        /// </summary>
        /// <autogeneratedoc />
        private object newVal;

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value.</value>
        /// <autogeneratedoc />
        public object OldVal => oldVal;
        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        /// <autogeneratedoc />
        public object NewVal => newVal;
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedExEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        /// <autogeneratedoc />
        public PropertyChangedExEventArgs(string propertyName,object oldVal,object newVal) : base(propertyName)
        {
            this.oldVal = oldVal;
            this.newVal = newVal;    
        }
    }
}