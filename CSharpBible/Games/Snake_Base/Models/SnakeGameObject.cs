﻿// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-25-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SnakeGameObject.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using BaseLib.Interfaces;
using Game_Base.Model;
using Game_Base.Model.Interfaces;
using Snake_Base.Models.Interfaces;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace Snake_Base.Models
{
    /// <summary>
    /// Class SnakeGameObject.
    /// Implements the <see cref="IPlacedObject" />
    /// Implements the <see cref="IParentedObject{IPlayfield2D{ISnakeGameObject}}" />
    /// </summary>
    /// <seealso cref="IPlacedObject" />
    /// <seealso cref="IParentedObject{IPlayfield2D{ISnakeGameObject}}" />
    public abstract class SnakeGameObject :ISnakeGameObject, IPlacedObject, IParentedObject<IPlayfield2D<ISnakeGameObject>> 
    {
        #region Properties
        #region private Properties
        /// <summary>
        /// The place
        /// </summary>
        /// <autogeneratedoc />
        private Point _place=Point.Empty;
        /// <summary>
        /// The old place
        /// </summary>
        /// <autogeneratedoc />
        private Point _oldPlace;
        /// <summary>
        /// The pf parent
        /// </summary>
        /// <autogeneratedoc />
        private IPlayfield2D<ISnakeGameObject>? _pfParent;
        #endregion
        #region static Properties
        /// <summary>
        /// Gets or sets the default parent.
        /// </summary>
        /// <value>The default parent.</value>
        public static IPlayfield2D<ISnakeGameObject>? DefaultParent { get; set; } = null;
        #endregion

        /// <summary>
        /// Occurs when [data change event].
        /// </summary>
        public event EventHandler<(string sender, object? oldVal, object? newVal)>? DataChangeEvent;
        /// <summary>
        /// Occurs when [on place change].
        /// </summary>
        public event EventHandler<(Point oP, Point nP)>? OnPlaceChange;

        /// <summary>
        /// Gets the old place.
        /// </summary>
        /// <value>The old place.</value>
        public Point OldPlace { get => GetOldPlace(); }
        #region interface Properties
        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>The place.</value>
        public Point Place { get => GetPlace();set=> SetPlace(value); }
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public IPlayfield2D<ISnakeGameObject>? Parent { get => GetParent(); set => SetParent(value); }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeGameObject" /> class.
        /// </summary>
        public SnakeGameObject() : this(Point.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeGameObject" /> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="parent">The parent.</param>
        public SnakeGameObject(Point place, IPlayfield2D<ISnakeGameObject>? parent=null) {
            Place= place;
            if (parent!=null)
                Parent = parent;
            else if (DefaultParent!=null)
                Parent= DefaultParent;  
        }

        #region interface Methods
        /// <summary>
        /// Gets the old place.
        /// </summary>
        /// <returns>Point.</returns>
        public virtual Point GetOldPlace() => _oldPlace;
        /// <summary>
        /// Gets the place.
        /// </summary>
        /// <returns>Point.</returns>
        public virtual Point GetPlace() => _place;
        /// <summary>
        /// Sets the place.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="Name">The name.</param>
        public virtual void SetPlace(Point value, [CallerMemberName] string Name = "") => value.SetProperty(ref _place, NtfyPlaceChange, Name);

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns>System.Nullable&lt;Playfield2D&lt;SnakeGameObject&gt;&gt;.</returns>
        public virtual IPlayfield2D<ISnakeGameObject>? GetParent() => _pfParent;
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="CallerMember">The caller member.</param>
        public virtual void SetParent(IPlayfield2D<ISnakeGameObject>? value, [CallerMemberName] string CallerMember = "") =>
            value.SetProperty(ref _pfParent, NtfyParentChange, CallerMember);
        #endregion

        /// <summary>
        /// Ntfies the parent change.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <autogeneratedoc />
        protected virtual void NtfyParentChange(string arg1, IPlayfield2D<ISnakeGameObject>? arg2, IPlayfield2D<ISnakeGameObject>? arg3)
        {
            if (arg2 is IHasChildren hc)
                hc.RemoveItem(this);
            if (arg3 is IHasChildren hc2)
                hc2.AddItem(this);
            if (arg3 is IHasChildren<ISnakeGameObject> hc3)
                hc3.AddItem(this);
            DataChangeEvent?.Invoke(this, (arg1, arg2, arg3));
        }

        /// <summary>
        /// Ntfies the place change.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <autogeneratedoc />
        protected virtual void NtfyPlaceChange(string arg1, Point arg2, Point arg3)
        {
            _oldPlace = arg2;
            if (_pfParent is IHasChildren<SnakeGameObject> hc)
                hc.NotifyChildChange(this, arg2, arg3);
            OnPlaceChange?.Invoke(this, (arg2, arg3));
            DataChangeEvent?.Invoke(this, (arg1, arg2, arg3));
        }

        public void ResetOldPlace()
        {
            _oldPlace = _place;
        }
        #endregion
    }
}