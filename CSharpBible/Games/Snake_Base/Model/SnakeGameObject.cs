﻿using BaseLib.Helper;
using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Base.Model
{
    /// <summary>
    /// Class SnakeGameObject.
    /// Implements the <see cref="Snake_Base.Model.IPlacedObject" />
    /// Implements the <see cref="BaseLib.Interfaces.IParentedObject{Snake_Base.Model.Playfield2D{Snake_Base.Model.SnakeGameObject}}" />
    /// </summary>
    /// <seealso cref="Snake_Base.Model.IPlacedObject" />
    /// <seealso cref="BaseLib.Interfaces.IParentedObject{Snake_Base.Model.Playfield2D{Snake_Base.Model.SnakeGameObject}}" />
    public abstract class SnakeGameObject : IPlacedObject, IParentedObject<Playfield2D<SnakeGameObject>> 
    {
        #region Properties
        #region private Properties
        private Point _place=Point.Empty;
        private Point _oldPlace;
        private Playfield2D<SnakeGameObject>? _pfParent;
        #endregion
        #region static Properties
        /// <summary>
        /// Gets or sets the default parent.
        /// </summary>
        /// <value>The default parent.</value>
        public static Playfield2D<SnakeGameObject>? DefaultParent { get; set; } = null;
        #endregion

        /// <summary>
        /// Occurs when [data change event].
        /// </summary>
        public event EventHandler<(string sender, object? oldVal, object? newVal)>? DataChangeEvent;
        /// <summary>
        /// Occurs when [on place change].
        /// </summary>
        public event EventHandler<(Point oP, Point nP)> OnPlaceChange;

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
        public Playfield2D<SnakeGameObject>? Parent { get => GetParent(); set => SetParent(value); }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeGameObject"/> class.
        /// </summary>
        public SnakeGameObject() : this(Point.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeGameObject"/> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="parent">The parent.</param>
        public SnakeGameObject(Point place, Playfield2D<SnakeGameObject>? parent=null) {
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
        public virtual void SetPlace(Point value, [CallerMemberName] string Name = "") => Property.SetProperty(ref _place, value,NtfyPlaceChange,Name);

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns>System.Nullable&lt;Playfield2D&lt;SnakeGameObject&gt;&gt;.</returns>
        public virtual Playfield2D<SnakeGameObject>? GetParent() => _pfParent;
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="CallerMember">The caller member.</param>
        public virtual void SetParent(Playfield2D<SnakeGameObject>? value, [CallerMemberName] string CallerMember = "") =>
            Property.SetProperty(ref _pfParent, value, NtfyParentChange, CallerMember);
        #endregion

        protected virtual void NtfyParentChange(string arg1, Playfield2D<SnakeGameObject>? arg2, Playfield2D<SnakeGameObject>? arg3)
        {
            if (arg2 is IHasChildren hc)
                hc.RemoveItem(this);
            if (arg3 is IHasChildren hc2)
                hc2.AddItem(this);
            if (arg3 is IHasChildren<SnakeGameObject> hc3)
                hc3.AddItem(this);
            DataChangeEvent?.Invoke(this, (arg1, arg2, arg3));
        }

        protected virtual void NtfyPlaceChange(string arg1, Point arg2, Point arg3)
        {
            _oldPlace = arg2;
            if (_pfParent is IHasChildren<SnakeGameObject> hc)
                hc.NotifyChildChange(this, arg2, arg3);
            OnPlaceChange?.Invoke(this, (arg2, arg3));
            DataChangeEvent?.Invoke(this, (arg1, arg2, arg3));
        }
        #endregion
    }
}
