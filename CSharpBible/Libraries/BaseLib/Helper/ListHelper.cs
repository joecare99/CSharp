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
// <summary>
// Provides extension methods for working with generic lists and arrays,
// including item manipulation, swapping, and range generation utilities.
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

/// <summary>
/// The Helper namespace contains utility classes and extension methods
/// that provide common functionality across the BaseLib library.
/// </summary>
namespace BaseLib.Helper;

/// <summary>
/// Provides static extension methods for manipulating generic lists and generating arrays.
/// This class contains helper methods for common list operations such as moving items,
/// swapping elements, and creating ranges of values.
/// </summary>
/// <remarks>
/// All methods in this class are implemented as extension methods, allowing them to be
/// called directly on <see cref="IList{T}"/> instances or value types.
/// </remarks>
/// <example>
/// <code>
/// // Moving an item in a list
/// var list = new List&lt;string&gt; { "A", "B", "C", "D" };
/// list.MoveItem(0, 3); // Result: { "B", "C", "A", "D" }
/// 
/// // Swapping items
/// list.Swap(0, 2); // Swaps items at index 0 and 2
/// 
/// // Creating a range
/// int[] range = 1.To(5); // Result: { 1, 2, 3, 4, 5 }
/// </code>
/// </example>
public static class ListHelper
{
    /// <summary>
    /// Moves an item from a source index to a destination index within the list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list in which to move the item.</param>
    /// <param name="iSrc">The zero-based index of the item to move.</param>
    /// <param name="iDst">The zero-based destination index where the item should be placed.</param>
    /// <remarks>
    /// <para>
    /// If the source and destination indices are equal, no operation is performed.
    /// </para>
    /// <para>
    /// The method handles the index shift that occurs when removing an item before
    /// the destination index. If an exception occurs during insertion, the item
    /// is restored to its original position before re-throwing the exception.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="iSrc"/> or <paramref name="iDst"/> is outside
    /// the valid range of indices for the list.
    /// </exception>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt; { 1, 2, 3, 4, 5 };
    /// list.MoveItem(0, 4); // Moves element at index 0 to index 4
    /// // Result: { 2, 3, 4, 1, 5 }
    /// </code>
    /// </example>
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
    /// Exchanges (swaps) the positions of two items in the list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list containing the items to swap.</param>
    /// <param name="iItm1">The zero-based index of the first item to swap.</param>
    /// <param name="iItm2">The zero-based index of the second item to swap.</param>
    /// <remarks>
    /// If both indices are equal, no operation is performed.
    /// Uses tuple deconstruction for efficient in-place swapping.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="iItm1"/> or <paramref name="iItm2"/> is outside
    /// the valid range of indices for the list.
    /// </exception>
    /// <example>
    /// <code>
    /// var list = new List&lt;string&gt; { "A", "B", "C" };
    /// list.Swap(0, 2); // Swaps "A" and "C"
    /// // Result: { "C", "B", "A" }
    /// </code>
    /// </example>
    public static void Swap<T>(this IList<T> list, int iItm1, int iItm2)
    {
        if (iItm1 == iItm2) return;
        (list[iItm1], list[iItm2]) = (list[iItm2], list[iItm1]);
    }

    /// <summary>
    /// Creates an array containing a range of values from <paramref name="v1"/> to <paramref name="v2"/> inclusive.
    /// </summary>
    /// <typeparam name="T">
    /// A value type that implements <see cref="IComparable{T}"/> and can be converted to and from <see cref="int"/>.
    /// </typeparam>
    /// <param name="_">
    /// The <see cref="Type"/> instance used as an extension method anchor. This parameter is not used.
    /// </param>
    /// <param name="v1">The starting value of the range (inclusive).</param>
    /// <param name="v2">The ending value of the range (inclusive).</param>
    /// <returns>
    /// An array of type <typeparamref name="T"/> containing all values from <paramref name="v1"/> 
    /// to <paramref name="v2"/> inclusive. Returns an empty array if <paramref name="v1"/> 
    /// is greater than <paramref name="v2"/>.
    /// </returns>
    /// <remarks>
    /// This method uses <see cref="Convert.ToInt32(object)"/> internally, so it works best
    /// with numeric types that can be represented as integers.
    /// </remarks>
    /// <example>
    /// <code>
    /// int[] range = typeof(int).Range(1, 5); // Result: { 1, 2, 3, 4, 5 }
    /// int[] empty = typeof(int).Range(5, 1); // Result: empty array
    /// </code>
    /// </example>
    public static T[] Range<T>(this Type _, T v1, T v2) where T : struct, IComparable<T>
    {
        if (v1.CompareTo(v2) > 0)
            return Array.Empty<T>();
        T[] result = new T[Convert.ToInt32(v2) - Convert.ToInt32(v1) + 1];
        for (int i = 0; i < result.Length; i++)
            result[i] = (T)Convert.ChangeType(Convert.ToInt32(v1) + i, typeof(T));
        return result;
    }

    /// <summary>
    /// Creates an array containing a range of values from the current value to the specified end value inclusive.
    /// </summary>
    /// <typeparam name="T">
    /// A value type that implements <see cref="IComparable{T}"/> and can be converted to and from <see cref="int"/>.
    /// </typeparam>
    /// <param name="v1">The starting value of the range (inclusive).</param>
    /// <param name="v2">The ending value of the range (inclusive).</param>
    /// <returns>
    /// An array of type <typeparamref name="T"/> containing all values from <paramref name="v1"/> 
    /// to <paramref name="v2"/> inclusive. Returns an empty array if <paramref name="v1"/> 
    /// is greater than <paramref name="v2"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This extension method provides a fluent syntax for creating ranges directly from a value.
    /// </para>
    /// <para>
    /// This method uses <see cref="Convert.ToInt32(object)"/> internally, so it works best
    /// with numeric types that can be represented as integers (e.g., <see cref="int"/>, 
    /// <see cref="byte"/>, <see cref="short"/>).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// int[] range = 1.To(5);    // Result: { 1, 2, 3, 4, 5 }
    /// byte[] bytes = ((byte)0).To((byte)3); // Result: { 0, 1, 2, 3 }
    /// int[] empty = 10.To(5);   // Result: empty array
    /// </code>
    /// </example>
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
