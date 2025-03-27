// ***********************************************************************
// Assembly         : BaseGenClasses
// Author           : Mir
// Created          : 03-21-2025
//
// Last Modified By : Mir
// Last Modified On : 03-21-2025
// ***********************************************************************
// <copyright file="WeakLinkList.cs" company="BaseGenClasses">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Helper namespace.
/// </summary>
namespace BaseGenClasses.Helper
{
    /// <summary>
    /// Class WeakLinkList.
    /// Implements the <see cref="IList{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IList{T}" />
    public class WeakLinkList<T> : IList<T> where T : class
    {
        /// <summary>
        /// The list
        /// </summary>
        private List<WeakReference<T>?> _list = new List<WeakReference<T>?>();
        /// <summary>
        /// Gets or sets the <see cref="System.Nullable{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Nullable&lt;T&gt;.</returns>
        public T? this[int index] { 
            get => _list[index]?.TryGetTarget(out T? target) ??false ? target : default; 
            set => _list[index] = value ==null?null: new WeakReference<T>(value); }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The count.</value>
        public int Count => _list.Count;
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => false;
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public virtual void Add(T item) => _list.Add(new WeakReference<T>(item));
        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public virtual void Clear() => _list.Clear();
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(T item) => _list.Any(wr => (wr?.TryGetTarget(out T? target) ?? false ? target.Equals(item) : false ));
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i]?.TryGetTarget(out T? target) ?? false)
                {
                    array[arrayIndex++] = target;
                }
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i]?.TryGetTarget(out T? target) ?? false)
                {
                    yield return target;
                }
            }
        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if ((_list[i]?.TryGetTarget(out T? target) ?? false) && target.Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public virtual void Insert(int index, T item) => _list.Insert(index, new WeakReference<T>(item));
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public virtual bool Remove(T item)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if ((_list[i]?.TryGetTarget(out T? target) ?? false) && target.Equals(item))
                {
                    _list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public virtual void RemoveAt(int index) => _list.RemoveAt(index);
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
