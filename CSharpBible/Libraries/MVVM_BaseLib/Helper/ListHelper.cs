// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 09-27-2023
//
// Last Modified By : Mir
// Last Modified On : 09-27-2023
// ***********************************************************************
// <copyright file="ListHelper.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

/// <summary>
/// The Helper namespace.
/// </summary>
namespace BaseLib.Helper
{
    /// <summary>
    /// Class ListHelper.
    /// </summary>
    public static class ListHelper
    {
        /// <summary>Moves the item from <param name="iSrc">source</param> to <param name="iDst">target</param>.</summary>
        /// <typeparam name="T">The generic type of the <b>list</b></typeparam>
        /// <param name="list">The list to change.</param>
        /// <param name="iSrc">The index of the source.</param>
        /// <param name="iDst">The index of the destination.</param>
        public static void MoveItem<T>(this IList<T> list, int iSrc, int iDst)
        {
            if (iSrc == iDst) return;
            T c = list[iSrc];
            list.RemoveAt(iSrc);
            try
            {
                list.Insert(iSrc <= iDst ? iDst - 1 : iDst, c);
            }
            catch
            {
                list.Insert(iSrc, c);
                throw;
            }
        }

        /// <summary>
        /// Exchanges the items.
        /// </summary>
        /// <typeparam name="T">The generic type of the <b>list</b></typeparam>
        /// <param name="list">The list to change.</param>
        /// <param name="iItm1">The index of the first item.</param>
        /// <param name="iItm2">The index of the second item.</param>
        public static void Swap<T>(this IList<T> list, int iItm1, int iItm2)
        {
            if (iItm1 == iItm2) return;
            (list[iItm1], list[iItm2]) = (list[iItm2],list[iItm1]);
        }

        public static T[] Range<T>(this Type _, T v1, T v2) where T : struct, IComparable<T>
        {
            if (v1.CompareTo(v2) > 0)
                return Array.Empty<T>();
            T[] result = new T[Convert.ToInt32(v2) - Convert.ToInt32(v1) + 1];
            for (int i = 0; i < result.Length; i++)
                result[i] = (T)Convert.ChangeType(Convert.ToInt32(v1) + i, typeof(T));
            return result;
        }
        public static T[] To<T>(this T v1, T v2) where T : struct, IComparable<T>
        {
            if (v1.CompareTo(v2) > 0)
                return Array.Empty<T>();
            T[] result = new T[Convert.ToInt32(v2) - Convert.ToInt32(v1) + 1];
            for (int i = 0; i < result.Length; i++)
                result[i] = (T)Convert.ChangeType(Convert.ToInt32(v1) + i, typeof(T));
            return result;
        }

    }
}
