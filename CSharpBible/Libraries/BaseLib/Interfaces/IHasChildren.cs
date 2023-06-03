// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 08-24-2022
//
// Last Modified By : Mir
// Last Modified On : 08-26-2022
// ***********************************************************************
// <copyright file="IHasChildren.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BaseLib.Interfaces
{
    /// <summary>
    /// Interface IHasChildren
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasChildren<T> where T : class  
    {
#if NET5_0_OR_GREATER
        IEnumerable<T> Items { get => GetItems(); }
#endif
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool AddItem(T value);
        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool RemoveItem(T value);

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        IEnumerable<T> GetItems();
        /// <summary>
        /// Notifies the child change.
        /// </summary>
        /// <param name="childObject">The child object.</param>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        /// <param name="prop">The property.</param>
        void NotifyChildChange(T childObject, object oldVal, object newVal,[CallerMemberName] string prop = "" );
    }

    /// <summary>
    /// Interface IHasChildren
    /// </summary>
    public interface IHasChildren : IHasChildren<object> { }

}