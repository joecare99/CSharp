// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="PlayObject.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Game_Base.Model.Interfaces;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class PlayObject.
    /// </summary>
    public abstract class PlayObject : IPlacedObject
    {
        private Point _place = Point.Empty;

        public event EventHandler<(Point oP, Point nP)>? OnPlaceChange;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position of the player on the playfield</value>
        public Point Place { get=>GetPlace(); set=>SetPlace(value); }

        public void SetPlace(Point value, [CallerMemberName] string Name = "")
        {
            if (_place == value) return;
            var _o = _place;
            _place = value;
            OnPlaceChange?.Invoke(this,(_o, _place));
        }

        public Point GetPlace() => _place;

        /// <summary>
        /// Gets or sets the old position.
        /// </summary>
        /// <value>The old position.</value>
        public Point OldPosition { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PlayObject" /> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field as reference.</value>
        public Field? field { get; set; }

        /// <summary>
        /// Tests if the object can move in the given direction.
        /// </summary>
        /// <param name="dir">The directon to test</param>
        /// <returns>true: if the object can move in the direction</returns>
        public abstract bool TestMove(Direction? dir=null);

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The directon to move</param>
        /// <returns>true: if the object has moveed in the direction</returns>
        public abstract bool TryMove(Direction? dir=null);

        public Point GetOldPlace()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayObject" /> class.
        /// </summary>
        /// <param name="aField">a field.</param>
        public PlayObject(Field? aField) { field = aField; }
    }
}
