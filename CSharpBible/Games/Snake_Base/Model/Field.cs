using BaseLib.Helper;
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
    /// Class Field.
    /// Implements the <see cref="Snake_Base.Model.IPlacedObject" />
    /// Implements the <see cref="IParentedObject" />
    /// Implements the <see cref="IHasChildren{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Snake_Base.Model.IPlacedObject" />
    /// <seealso cref="IParentedObject" />
    /// <seealso cref="IHasChildren{T}" />
    public class Field<T> : IPlacedObject, IParentedObject, IHasChildren<T>
    {
        #region Properties
        private List<T> _items = new List<T>();
        private Point _place;
        private object? _parent;
        private Point _oldPlace;

        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>The place.</value>
        public Point Place { get => GetPlace(); set => SetPlace(value); }
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public object? Parent { get => GetParent(); set => SetParent(value); }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public List<T> Items { get => _items; set => Property.SetProperty(ref _items, value, NtfyListChange); }
        /// <summary>
        /// Occurs when [data change event].
        /// </summary>
        public event EventHandler<(string sender, object? oldVal, object? newVal)> DataChangeEvent;
        /// <summary>
        /// Occurs when [on place change].
        /// </summary>
        public event EventHandler<(Point oP, Point nP)> OnPlaceChange;
        #endregion

        #region Methods
        #region Interface Methods
        /// <summary>
        /// Gets the place.
        /// </summary>
        /// <returns>Point.</returns>
        public Point GetPlace() => _place;
        /// <summary>
        /// Sets the place.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="Name">The name.</param>
        public void SetPlace(Point value, [CallerMemberName] string Name = "") => Property.SetProperty(ref _place, value, NtfyPlaceChange, Name);
        /// <summary>
        /// Gets the old place.
        /// </summary>
        /// <returns>Point.</returns>
        public Point GetOldPlace() => _oldPlace;

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AddItem(T value)
        {
            if ((_items ?? (Items = new List<T>())).Contains(value)) return false;
            if (value is IPlacedObject plo)
                plo.Place = GetPlace();
            if (value is IParentedObject po)
                po.Parent = this;
            _items!.Add(value);
            DataChangeEvent?.Invoke(this, ("Items.Add", null, value));

            return true;
        }
        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool RemoveItem(T value)
        {
            if (_items == null || !Items.Contains(value)) return false;
            _items.Remove(value);
            DataChangeEvent?.Invoke(this, ("Items.Remove", value, null));
            return true;
        }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetItems() => _items;

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="CallerMember">The caller member.</param>
        public void SetParent(object? value, [CallerMemberName] string CallerMember = "") => Property.SetProperty(ref _parent, value, NtfyParentChange, CallerMember);
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
        public object? GetParent() => _parent;
        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="Field{T}"/> class.
        /// </summary>
        public Field():this(Point.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field{T}"/> class.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="parent">The parent.</param>
        public Field(Point place, object? parent=null)
        {
            _oldPlace =
            _place = place;
            _parent = parent;
            if (_parent is IPlacedObject po)
                _oldPlace =
                _place = po.Place;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"{nameof(Field<T>)}({GetPlace()},I:{Items?.Count})";

        #region Private Methods
        private void NtfyPlaceChange(string arg1, Point arg2, Point arg3)
        {
            _oldPlace = arg2;
            if(_items!=null)
            foreach (T item in _items)
                if (item is IPlacedObject po)
                    po.Place = arg3;
            OnPlaceChange?.Invoke(this, (arg2, arg3));
            DataChangeEvent?.Invoke(this,(arg1, arg2, arg3));
        }

        private void NtfyListChange(string arg1, List<T> arg2, List<T> arg3) => DataChangeEvent?.Invoke(this, (arg1, arg2, arg3));

        private void NtfyParentChange(string arg1, object? arg2, object? arg3)
        {
            if (arg3 is IPlacedObject po)
                Place = po.Place;
            DataChangeEvent?.Invoke(this, (arg1, arg2, arg3));
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
            {
                if (childObject is IParentedObject co)
                {
                    if (co.Parent != this && co.Parent is IHasChildren<T> ophc)
                        ophc.RemoveItem(childObject);
                }
                AddItem(childObject);
            }
        }
        #endregion
        #endregion
    }
}
