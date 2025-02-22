// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Playfield2D.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using BaseLib.Interfaces;
using Game_Base.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

/// <summary>
/// The Model namespace.
/// </summary>
/// <autogeneratedoc />
namespace Game_Base.Model
{
    /// <summary>
    /// Class Playfield2D.
    /// Implements the <see cref="IHasChildren{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IHasChildren{T}" />
    public class Playfield2D<T>: IPlayfield2D<T>, IHasChildren<T> where T : class
    {
        #region Properties
        #region private Properties
        /// <summary>
        /// The pf data
        /// </summary>
        /// <autogeneratedoc />
        private readonly Dictionary<Point, T> _pfData = new Dictionary<Point, T>();
        /// <summary>
        /// The pf rect
        /// </summary>
        /// <autogeneratedoc />
        private Rectangle _pfRect;
        /// <summary>
        /// The pf size
        /// </summary>
        /// <autogeneratedoc />
        private Size _pfSize;
        #endregion

        /// <summary>
        /// Gets or sets the size of the pf.
        /// </summary>
        /// <value>The size of the pf.</value>
        public Size PfSize { get => _pfRect.Size; set => value.SetProperty(ref _pfSize, PfResize); }
        /// <summary>
        /// Gets the rect.
        /// </summary>
        /// <value>The rect.</value>
        public Rectangle Rect { get => _pfRect; }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<T> Items => GetItems();
        /// <summary>
        /// Occurs when [on data changed].
        /// </summary>
        public event EventHandler<(string prop,object? oldVal,object? newVal)>? OnDataChanged;

        /// <summary>
        /// Gets or sets the default size.
        /// </summary>
        /// <value>The default size.</value>
        public static Size DefaultSize { get; set; } = new Size(20, 20);

        #region this
        /// <summary>
        /// Gets or sets the <see cref="System.Nullable{T}" /> with the specified p.
        /// </summary>
        /// <param name="P">The p.</param>
        /// <returns>System.Nullable&lt;T&gt;.</returns>
        public T? this[Point P]
        {
            get => _pfData.TryGetValue(P,out var t) ? t : default; set
            {
                if (IsInside(P))
                {
                    if (EqualityComparer<T?>.Default.Equals(this[P], value)) return;
                    if (value is IPlacedObject plo)
                    {
#if NET6_0_OR_GREATER
                        plo.Place = P;
#else
                        plo.SetPlace(P);
#endif
                        plo.OnPlaceChange += ChildPlaceChanged;
                    }
                    if (value is IParentedObject && EqualityComparer<T?>.Default.Equals(this[P], value)) return;
                    if (value != null)
                    {
                        _pfData[P] = value;
#if NET6_0_OR_GREATER
                        if (value is IParentedObject po && po.Parent != this)
                            po.Parent = (IHasChildren<object>)this;
#else
                        if (value is IParentedObject po && po.GetParent() != this)
                            po.SetParent((IHasChildren<object>)this);
#endif
                        OnDataChanged?.Invoke(this, ("Items", null, value));
                    }
                    else if (_pfData.ContainsKey(P))
                    {
                        var oldData = _pfData[P];
                        _pfData.Remove(P);
                        if (oldData is IPlacedObject opl)
                            opl.OnPlaceChange -= ChildPlaceChanged;
                        if (oldData is IParentedObject opr)
#if NET6_0_OR_GREATER
                            opr.Parent = null;
#else
                            opr.SetParent(null);
#endif
                        OnDataChanged?.Invoke(this, ("Items", oldData, null));
                    }
                }
            }
        }

#endregion
#endregion

#region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Playfield2D{T}" /> class.
        /// </summary>
        public Playfield2D():this(DefaultSize){
            }

        /// <summary>
        /// Initializes a new instance of the <see cref="Playfield2D{T}" /> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public Playfield2D(Size size)
        {
            PfSize = size;
        }

        /// <summary>Determines whether the specified point p is inside the Playfield.</summary>
        /// <param name="P">The point.</param>
        /// <returns>
        ///   <c>true</c> if the specified point p is inside; otherwise, <c>false</c>.</returns>
        /// <autogeneratedoc />
        public bool IsInside(Point P)
        {
            return P.X < _pfSize.Width && P.Y < _pfSize.Height && P.X >= 0 && P.Y >= 0;
        }
        /// <summary>
        /// Pfs the resize.
        /// </summary>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        /// <autogeneratedoc />
        private void PfResize(string arg1, Size arg2, Size arg3)
        {
            _pfRect = new Rectangle(Point.Empty, arg3);
            OnDataChanged?.Invoke(this, (arg1, arg2, arg3));
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AddItem(T value)
        {
            bool result = false;
            if (value is IPlacedObject po
#if NET6_0_OR_GREATER
                && !EqualityComparer<T?>.Default.Equals(this[po.Place], value))
#else
                && !EqualityComparer<T?>.Default.Equals(this[po.GetPlace()], value))
#endif
            {
#if NET6_0_OR_GREATER
                this[po.Place] = value;
#else
                this[po.GetPlace()] = value;
#endif
                //po.OnPlaceChange += ChildPlaceChanged;
                //OnDataChanged?.Invoke(this, ("Items", null, value));
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool RemoveItem(T value)
        {
            if (value is IPlacedObject plo)
            {
                plo.OnPlaceChange -= ChildPlaceChanged;
#if NET6_0_OR_GREATER
                bool result =_pfData.Remove(plo.Place);
#else
                bool result = _pfData.Remove(plo.GetPlace());
#endif
               if (value is IParentedObject pro)
#if NET6_0_OR_GREATER
                    pro.Parent = null;
#else
                    pro.SetParent(null);
#endif
                OnDataChanged?.Invoke(this, ("Items", value, null));
                return result;
            }
            else
               return false;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetItems()
        {
            return _pfData.Values;
        }

        /// <summary>
        /// Notifies the child change.
        /// </summary>
        /// <param name="childObject">The child object.</param>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        /// <param name="prop">The property.</param>
        public void NotifyChildChange(T childObject, object oldVal, object newVal, [CallerMemberName] string prop = "")
        {
            if (newVal is Point np && oldVal is Point op)
            { //s.u.
            }
            else
                OnDataChanged?.Invoke(childObject, (prop, oldVal, newVal));
        }

        /// <summary>
        /// Childs the place changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <autogeneratedoc />
        private void ChildPlaceChanged(object? sender, (Point oP, Point nP) e)
        {
            if (sender is T tObj)
            {
                if (EqualityComparer<T?>.Default.Equals(this[e.oP], tObj))
                    _pfData.Remove(e.oP);
                if (this[e.nP] == null)
                    _pfData.Add(e.nP, tObj);
                OnDataChanged?.Invoke(sender, ("Place", e.oP, e.nP));
            }
        }
#endregion
    }

    /// <summary>
    /// Class Playfield2D.
    /// Implements the <see cref="IHasChildren{T}" />
    /// </summary>
    /// <seealso cref="IHasChildren{T}" />
    public class Playfield2D : Playfield2D<object> { public Playfield2D(Size size) : base(size) { } }
}
