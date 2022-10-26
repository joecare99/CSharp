﻿// ***********************************************************************
// Assembly         : CSFreeVision
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="Group.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// The Base namespace.
/// </summary>
/// <autogeneratedoc />
namespace CSFreeVision.Base
{
    /// <summary>
    /// Class Group.
    /// Implements the <see cref="CSFreeVision.Base.View" />
    /// Implements the <see cref="IContainer" />
    /// </summary>
    /// <seealso cref="CSFreeVision.Base.View" />
    /// <seealso cref="IContainer" />
    /// <autogeneratedoc />
    public partial class Group : View , IContainer
    {
        /// <summary>
        /// The components
        /// </summary>
        /// <autogeneratedoc />
        private Dictionary<string,View> _Components = new Dictionary<string, View>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public Group()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <autogeneratedoc />
        public Group(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Ruft alle Komponenten der <see cref="T:System.ComponentModel.IContainer" />.
        /// </summary>
        /// <value>The components.</value>
        /// <autogeneratedoc />
        public Dictionary<string, View>.ValueCollection Components => _Components.Values;

        /// <summary>
        /// Ruft alle Komponenten der <see cref="T:System.ComponentModel.IContainer" />.
        /// </summary>
        /// <value>The components.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <autogeneratedoc />
        ComponentCollection IContainer.Components => throw new NotImplementedException();

        /// <summary>
        /// Fügt das angegebene <see cref="T:System.ComponentModel.IComponent" /> auf die <see cref="T:System.ComponentModel.IContainer" /> am Ende der Liste.
        /// </summary>
        /// <param name="component">Das hinzuzufügende <see cref="T:System.ComponentModel.IComponent" />.</param>
        /// <autogeneratedoc />
        public void Add(IComponent component) => _Components.Add(null,(View)component);

        /// <summary>
        /// Fügt das angegebene <see cref="T:System.ComponentModel.IComponent" /> auf die <see cref="T:System.ComponentModel.IContainer" /> am Ende der Liste aus, und weist einen Namen für die Komponente.
        /// </summary>
        /// <param name="component">Das hinzuzufügende <see cref="T:System.ComponentModel.IComponent" />.</param>
        /// <param name="name">Der eindeutige, Groß-/Kleinschreibung der Name der Komponente zuweisen.
        /// - oder -
        /// <see langword="null" /> die bewirkt, dass die Komponente nicht benannt ist.</param>
        /// <autogeneratedoc />
        public void Add(IComponent component, string name)
        {
            _Components.Add(name,(View)component);
        }

        /// <summary>
        /// Entfernt eine Komponente aus der <see cref="T:System.ComponentModel.IContainer" />.
        /// </summary>
        /// <param name="component">Das zu entfernende <see cref="T:System.ComponentModel.IComponent" />-Element.</param>
        /// <autogeneratedoc />
        public void Remove(IComponent component) => _Components.Remove(null);
    }
}
