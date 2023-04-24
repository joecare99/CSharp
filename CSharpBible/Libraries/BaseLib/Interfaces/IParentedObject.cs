// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 08-24-2022
//
// Last Modified By : Mir
// Last Modified On : 08-25-2022
// ***********************************************************************
// <copyright file="IParentedObject.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
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
#if NET5_0_OR_GREATER
        T? Parent { get => GetParent(); set => SetParent(value); }
#endif
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="CallerMember">The caller member.</param>
        void SetParent(T? value, [CallerMemberName] string CallerMember ="" );
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns>T.</returns>
        T? GetParent();
    }

    /// <summary>
    /// Interface IParentedObject
    /// </summary>
    public interface IParentedObject : IParentedObject<object?>{ }

}