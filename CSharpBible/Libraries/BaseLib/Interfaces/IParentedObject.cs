// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 08-24-2022
//
// Last Modified By : Mir
// Last Modified On : 08-25-2022
// ***********************************************************************
// <copyright file="IParentedObject.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Runtime.CompilerServices;

namespace BaseLib.Interfaces
{
    /// <summary>
    /// Interface IParentedObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParentedObject<T>
    {
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="CallerMember">The caller member.</param>
        #if NET5_0_OR_GREATER
        void SetParent(T? value, [CallerMemberName] string CallerMember ="" );
        T Parent { get => GetParent(); set => SetParent(value); }
        T? GetParent();
        #else
        void SetParent(T value, [CallerMemberName] string CallerMember ="" );
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns>T.</returns>
        T GetParent();
        #endif

    }

    /// <summary>
    /// Interface IParentedObject
    /// </summary>
    public interface IParentedObject : IParentedObject<
#if NET5_0_OR_GREATER
        object?
#else
        object
#endif
        >{ }

}