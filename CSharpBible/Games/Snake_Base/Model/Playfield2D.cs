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
    /// Class Playfield2D.
    /// Implements the <see cref="IHasChildren{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IHasChildren{T}" />
    public class Playfield2D<T>: IHasChildren<T>
    {
        #region Properties
        #region private Properties
        private Dictionary<Point, T> _pfData = new Dictionary<Point, T>();
        private Rectangle _pfRect;
        private Size _pfSize;
        #endregion

        /// <summary>
        /// Gets or sets the size of the pf.
        /// </summary>
        /// <value>The size of the pf.</value>
        public Size PfSize { get => _pfRect.Size; set => Property.SetProperty(ref _pfSize, value, PfResize); }
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
        public event EventHandler<(string prop,object? oldVal,object? newVal)> OnDataChanged;

        /// <summary>
        /// Gets or sets the default size.
        /// </summary>
        /// <value>The default size.</value>
        public static Size DefaultSize { get; set; } = new Size(20, 20);
        #endregion

        #region Methods
        /// <summary>
        /// Gets or sets the <see cref="System.Nullable{T}"/> with the specified p.
        /// </summary>
        /// <param name="P">The p.</param>
        /// <returns>System.Nullable&lt;T&gt;.</returns>
        public T? this[Point P]
        {
            get => _pfData.ContainsKey(P) ? _pfData[P] : default; set
            {
                if (P.X < _pfSize.Width && P.Y < _pfSize.Height)
                {
                    if (EqualityComparer<T>.Default.Equals(this[P], value)) return;
                    if (value is IPlacedObject plo)
                    {
                        plo.Place = P;
                        plo.OnPlaceChange += ChildPlaceChanged;
                    }
                    if (value is IParentedObject && EqualityComparer<T>.Default.Equals(this[P], value)) return;
                    if (value != null)
                    { 
                        _pfData[P] = value;
                        if (value is IParentedObject po && po.Parent != this)
                            po.Parent = (IHasChildren<object>)this;
                        OnDataChanged?.Invoke(this, ("Items", null, value));
                    }
                    else if (_pfData.ContainsKey(P))
                    {
                        var oldData = _pfData[P];
                        _pfData.Remove(P);
                        if (oldData is IPlacedObject opl)
                            opl.OnPlaceChange -= ChildPlaceChanged;
                        if (oldData is IParentedObject opr)
                            opr.Parent = null;
                        OnDataChanged?.Invoke(this, ("Items", oldData, null));
                    }
                }
            } }
        #endregion

        #region
        /// <summary>
        /// Initializes a new instance of the <see cref="Playfield2D{T}"/> class.
        /// </summary>
        public Playfield2D():this(DefaultSize){
            }

        /// <summary>
        /// Initializes a new instance of the <see cref="Playfield2D{T}"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public Playfield2D(Size size)
        {
            PfSize = size;
        }

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
            if (value is IPlacedObject po && !EqualityComparer<T>.Default.Equals(this[po.Place], value))
            {
                this[po.Place] = value;
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
               _pfData.Remove(plo.Place);
               if (value is IParentedObject pro)
                  pro.Parent = null;
                OnDataChanged?.Invoke(this, ("Items", value, null));
                return true;
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

        private void ChildPlaceChanged(object? sender, (Point oP, Point nP) e)
        {
            if (sender is T tObj)
            {
                if (EqualityComparer<T>.Default.Equals(this[e.oP], tObj))
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
