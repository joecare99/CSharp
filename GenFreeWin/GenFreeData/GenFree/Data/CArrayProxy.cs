// ***********************************************************************
// Assembly         : GenFreeData
// Author           : Mir
// Created          : 11-17-2023
//
// Last Modified By : Mir
// Last Modified On : 11-19-2023
// ***********************************************************************
// <copyright file="CArrayProxy.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenFree.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The Data namespace.
/// </summary>
namespace GenFree.Data
{
    /// <summary>
    /// Class CArrayProxy.
    /// Implements the <see cref="IArrayProxy{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IArrayProxy{T}" />
    public class CArrayProxy<T> : IArrayProxy<T>
    {
        /// <summary>
        /// Gets or sets the <see cref="T"/> with the specified index.
        /// </summary>
        /// <param name="Idx">The index.</param>
        /// <returns>T.</returns>
        public T this[object Idx] { get => getaction.Invoke(Idx) ?? default!; set => setaction?.Invoke(Idx, value); }

        /// <summary>
        /// Gets the getaction.
        /// </summary>
        /// <value>The getaction.</value>
        public Func<object, T?> getaction { get; private set; }

        /// <summary>
        /// Gets the setaction.
        /// </summary>
        /// <value>The setaction.</value>
        public Action<object, T>? setaction { get; private set; }
        /// <summary>
        /// Gets or sets the getenum.
        /// </summary>
        /// <value>The getenum.</value>
        public Func<IEnumerator<T>>? getenum { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CArrayProxy{T}"/> class.
        /// </summary>
        /// <param name="getaction">The getaction.</param>
        /// <param name="setaction">The setaction.</param>
        public CArrayProxy(Func<object, T?> getaction, Action<object, T>? setaction = null)
        {
            this.getaction = getaction;
            this.setaction = setaction;
            getenum = () => Enumerable.Empty<T>().GetEnumerator();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CArrayProxy{T}"/> to <see cref="T[]"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator T[](CArrayProxy<T> proxy)
             => proxy.ToArray();

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator&lt;T&gt;.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return getenum?.Invoke()!;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return getenum?.Invoke()!;
        }
    }
}