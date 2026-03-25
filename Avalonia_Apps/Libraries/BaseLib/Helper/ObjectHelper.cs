// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="ObjectHelper.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BaseLib.Helper;

/// <summary>
/// Provides extension methods for safe type conversion of <see cref="object"/> instances to various primitive and common types.
/// </summary>
/// <remarks>
/// <para>
/// This static class contains a collection of extension methods that facilitate the conversion of objects 
/// to specific types such as <see cref="int"/>, <see cref="long"/>, <see cref="Enum"/>, <see cref="DateTime"/>, 
/// <see cref="double"/>, <see cref="bool"/>, and <see cref="Guid"/>.
/// </para>
/// <para>
/// Each conversion method handles various input types including:
/// <list type="bullet">
///   <item><description>Direct type matches (returns the value as-is)</description></item>
///   <item><description>String parsing with appropriate format providers</description></item>
///   <item><description><see cref="IHasValue"/> interface implementations for recursive value extraction</description></item>
///   <item><description><see langword="null"/> and <see cref="DBNull"/> values (returns default)</description></item>
///   <item><description><see cref="IConvertible"/> implementations for standard .NET type conversions</description></item>
/// </list>
/// </para>
/// </remarks>
public static class ObjectHelper
{
    /// <summary>
    /// Converts an object to a 32-bit signed integer.
    /// </summary>
    /// <param name="obj">The object to convert. Can be <see langword="null"/>.</param>
    /// <param name="def">The default value to return when conversion fails. Defaults to <c>0</c>.</param>
    /// <returns>
    /// The converted <see cref="int"/> value, or <paramref name="def"/> if conversion is not possible.
    /// </returns>
    /// <remarks>
    /// <para>The conversion is performed using the following priority:</para>
    /// <list type="number">
    ///   <item><description>If <paramref name="obj"/> is already an <see cref="int"/>, returns it directly.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="uint"/>, performs an unchecked cast to <see cref="int"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/>, attempts to parse it as an integer.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IHasValue"/>, recursively converts its <see cref="IHasValue.Value"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is <see langword="null"/> or <see cref="DBNull"/>, returns <paramref name="def"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IConvertible"/>, uses <see cref="IConvertible.ToInt32(IFormatProvider)"/> with <see cref="CultureInfo.InvariantCulture"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// int result1 = "42".AsInt();           // Returns 42
    /// int result2 = ((object)null).AsInt(); // Returns 0
    /// int result3 = "invalid".AsInt(-1);    // Returns -1
    /// </code>
    /// </example>
    public static int AsInt(this object? obj, int def = default) => obj switch
    {
        int i => i,
        uint ui => unchecked((int)ui),
        string s => int.TryParse(s, out int i) ? i : def,
        IHasValue f => f.Value.AsInt(),
        null => def,
        DBNull => def,
        IConvertible c => c.ToInt32(CultureInfo.InvariantCulture),
        _ => def
    };

    /// <summary>
    /// Converts an object to a 64-bit signed integer.
    /// </summary>
    /// <param name="obj">The object to convert. Can be <see langword="null"/>.</param>
    /// <param name="def">The default value to return when conversion fails. Defaults to <c>0</c>.</param>
    /// <returns>
    /// The converted <see cref="long"/> value, or <paramref name="def"/> if conversion is not possible.
    /// </returns>
    /// <remarks>
    /// <para>The conversion is performed using the following priority:</para>
    /// <list type="number">
    ///   <item><description>If <paramref name="obj"/> is already a <see cref="long"/>, returns it directly.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="ulong"/>, performs an unchecked cast to <see cref="long"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/>, attempts to parse it as a long integer.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IHasValue"/>, recursively converts its <see cref="IHasValue.Value"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is <see langword="null"/> or <see cref="DBNull"/>, returns <paramref name="def"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IConvertible"/>, uses <see cref="IConvertible.ToInt64(IFormatProvider)"/> with <see cref="CultureInfo.InvariantCulture"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// long result1 = "9223372036854775807".AsLong(); // Returns long.MaxValue
    /// long result2 = ((object)null).AsLong();        // Returns 0
    /// </code>
    /// </example>
    public static long AsLong(this object? obj, int def = default) => obj switch
    {
        long i => i,
        ulong ul => unchecked((long)ul),
        string s => long.TryParse(s, out long i) ? i : def,
        IHasValue f => f.Value.AsLong(),
        null => def,
        DBNull => def,
        IConvertible c => c.ToInt64(CultureInfo.InvariantCulture),
        _ => def
    };

    /// <summary>
    /// Converts an object to an enumeration value of the specified type.
    /// </summary>
    /// <typeparam name="T">The enumeration type to convert to. Must be a value type and an <see cref="Enum"/>.</typeparam>
    /// <param name="obj">The object to convert. Can be <see langword="null"/>.</param>
    /// <returns>
    /// The converted enumeration value of type <typeparamref name="T"/>, or <see langword="default"/> if conversion is not possible.
    /// </returns>
    /// <remarks>
    /// <para>The conversion is performed using the following priority:</para>
    /// <list type="number">
    ///   <item><description>If <paramref name="obj"/> is already of type <typeparamref name="T"/>, returns it directly.</description></item>
    ///   <item><description>If <paramref name="obj"/> is an <see cref="int"/> within the valid enum range, converts it to the enum value.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/>, attempts to parse it as an enum name.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IHasValue"/>, recursively converts its <see cref="IHasValue.Value"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is <see langword="null"/> or <see cref="DBNull"/>, returns <see langword="default"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="uint"/>, performs an unchecked cast and converts to enum.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IConvertible"/>, converts to <see cref="int"/> first, then to enum.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// enum Color { Red, Green, Blue }
    /// Color result1 = "Green".AsEnum&lt;Color&gt;();  // Returns Color.Green
    /// Color result2 = 1.AsEnum&lt;Color&gt;();        // Returns Color.Green
    /// Color result3 = "Invalid".AsEnum&lt;Color&gt;(); // Returns Color.Red (default)
    /// </code>
    /// </example>
    public static T AsEnum<T>(this object? obj) where T : struct, Enum => obj switch
    {
        T t => t,
        int i when i < Enum.GetValues(typeof(T)).Length => (T)Enum.ToObject(typeof(T), i),
        string s when Enum.TryParse<T>(s, out var t) => t,
//        string s when int.TryParse(s, out var i) => (T)Enum.ToObject(typeof(T), i),
        string s=> default,
        IHasValue f => f.Value.AsEnum<T>(),
        null => default,
        DBNull => default,
        uint ui => (T)Enum.ToObject(typeof(T), unchecked((int)ui)),
        IConvertible c => (T)Enum.ToObject(typeof(T), c.ToInt32(CultureInfo.InvariantCulture)),
        _ => default
    };

    /// <summary>
    /// Converts an object to a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="obj">The object to convert. Can be <see langword="null"/>.</param>
    /// <returns>
    /// The converted <see cref="DateTime"/> value, or <see cref="DateTime.MinValue"/> if conversion is not possible.
    /// </returns>
    /// <remarks>
    /// <para>The conversion is performed using the following priority:</para>
    /// <list type="number">
    ///   <item><description>If <paramref name="obj"/> is already a <see cref="DateTime"/>, returns it directly.</description></item>
    ///   <item><description>If <paramref name="obj"/> is an <see cref="int"/> in the format YYYYMMDD (e.g., 20231225), parses it as a date.</description></item>
    ///   <item><description>If <paramref name="obj"/> is an <see cref="int"/> that is 0 or greater than 10000000, returns <see langword="default"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IHasValue"/>, recursively converts its <see cref="IHasValue.Value"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/> without dots, attempts to parse using <see cref="CultureInfo.InvariantCulture"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/> with dots, attempts to parse using <see cref="CultureInfo.CurrentUICulture"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/> representing a numeric date (YYYYMMDD), parses accordingly.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="long"/>, treats it as ticks for <see cref="DateTime"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="uint"/>, converts from OLE Automation date format.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="char"/>, creates a date with year = char value + 1900.</description></item>
    ///   <item><description>If <paramref name="obj"/> is <see cref="DBNull"/>, returns <see langword="default"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IConvertible"/>, converts from OLE Automation date format.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// DateTime result1 = "2023-12-25".AsDate();    // Returns December 25, 2023
    /// DateTime result2 = 20231225.AsDate();        // Returns December 25, 2023
    /// DateTime result3 = ((object)null).AsDate();  // Returns DateTime.MinValue
    /// </code>
    /// </example>
    public static DateTime AsDate(this object? obj) => obj switch
    {
        DateTime dt => dt,
        int i when (i%100 is >0 and <32) && (i/100%100 is >0 and <13) => new(i / 10000, i % 10000 / 100, i % 100),
        int i when i == 0 || i>10000000 => default,
        IHasValue f => f.Value.AsDate(),
        string s when !s.Contains('.') && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) => dt,
        string s when DateTime.TryParse(s, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var dt) => dt,
        string s when int.TryParse(s, out var i) && (i % 100 is > 0 and < 32) && (i / 100 % 100 is > 0 and < 13) => new(i / 10000, i % 10000 / 100, i % 100),
        string s when !int.TryParse(s, out var i) || i==0 => default,
        long l => new(l),
        uint ui => DateTime.FromOADate(unchecked((int)ui)),
        char c => new(c+1900,1,1),
        DBNull => default,
        IConvertible c => DateTime.FromOADate(c.ToDouble(CultureInfo.InvariantCulture)),
        _ => default,
    };

    /// <summary>
    /// Converts an object to a double-precision floating-point number.
    /// </summary>
    /// <param name="obj">The object to convert. Can be <see langword="null"/>.</param>
    /// <param name="culture">
    /// The culture-specific formatting information to use for parsing strings. 
    /// If <see langword="null"/>, <see cref="CultureInfo.InvariantCulture"/> is used.
    /// </param>
    /// <returns>
    /// The converted <see cref="double"/> value, or <c>0.0</c> if conversion is not possible.
    /// </returns>
    /// <remarks>
    /// <para>The conversion is performed using the following priority:</para>
    /// <list type="number">
    ///   <item><description>If <paramref name="obj"/> is already a <see cref="double"/>, returns it directly.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/>, attempts to parse it using <see cref="NumberStyles.Float"/> and the specified culture.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IHasValue"/>, recursively converts its <see cref="IHasValue.Value"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is <see langword="null"/> or <see cref="DBNull"/>, returns <see langword="default"/> (0.0).</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="char"/>, returns its numeric value.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IConvertible"/>, uses <see cref="IConvertible.ToDouble(IFormatProvider)"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// double result1 = "3.14".AsDouble();                              // Returns 3.14
    /// double result2 = "3,14".AsDouble(CultureInfo.GetCultureInfo("de-DE")); // Returns 3.14
    /// double result3 = ((object)null).AsDouble();                      // Returns 0.0
    /// </code>
    /// </example>
    public static double AsDouble(this object? obj, CultureInfo? culture = null) => obj switch
    {
        double d => d,
        string s when double.TryParse(s, NumberStyles.Float, culture ?? CultureInfo.InvariantCulture, out var d) => d,
        string s => default,
        IHasValue f => f.Value.AsDouble(culture),
        null => default,
        DBNull => default,
        char c => c,
        IConvertible c => c.ToDouble(culture ?? CultureInfo.InvariantCulture),
        _ => default
    };

    /// <summary>
    /// Converts an object to a Boolean value.
    /// </summary>
    /// <param name="obj">The object to convert. Can be <see langword="null"/>.</param>
    /// <returns>
    /// The converted <see cref="bool"/> value, or <see langword="false"/> if conversion is not possible.
    /// </returns>
    /// <remarks>
    /// <para>The conversion is performed using the following priority:</para>
    /// <list type="number">
    ///   <item><description>If <paramref name="obj"/> is already a <see cref="bool"/>, returns it directly.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IHasValue"/>, recursively converts its <see cref="IHasValue.Value"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is <see langword="null"/> or <see cref="DBNull"/>, returns <see langword="false"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/>, attempts to parse it using <see cref="bool.TryParse(string, out bool)"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/> equal to "1", returns <see langword="true"/>; otherwise <see langword="false"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="char"/>, returns <see langword="true"/> for '1', 'T', or 't'.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IConvertible"/>, returns <see langword="true"/> if not equal to 0.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// bool result1 = "true".AsBool();           // Returns true
    /// bool result2 = "1".AsBool();              // Returns true
    /// bool result3 = 'T'.AsBool();              // Returns true
    /// bool result4 = ((object)null).AsBool();  // Returns false
    /// </code>
    /// </example>
    public static bool AsBool(this object? obj) => obj switch
    {
        bool x => x,
        IHasValue f => f.Value.AsBool(),
        null => default,
        DBNull => default,
        string s when bool.TryParse(s, out var b) => b,
        string s when s!="1" => false,
        char c => c is '1' or 'T' or 't',
        IConvertible c => !((IConvertible)0).Equals(c),
        _ => default
    };

    /// <summary>
    /// Converts an object to a <see cref="Guid"/> value.
    /// </summary>
    /// <param name="obj">The object to convert. Can be <see langword="null"/>.</param>
    /// <returns>
    /// The converted <see cref="Guid"/> value, or <see cref="Guid.Empty"/> if conversion is not possible.
    /// </returns>
    /// <remarks>
    /// <para>The conversion is performed using the following priority:</para>
    /// <list type="number">
    ///   <item><description>If <paramref name="obj"/> is already a <see cref="Guid"/>, returns it directly.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IHasValue"/>, recursively converts its <see cref="IHasValue.Value"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is <see langword="null"/> or <see cref="DBNull"/>, returns <see cref="Guid.Empty"/>.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/>, attempts to parse it as a GUID.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="string"/> representing an integer, creates a GUID with that integer as the first component.</description></item>
    ///   <item><description>If <paramref name="obj"/> is a <see cref="uint"/>, creates a GUID with that value as the first component.</description></item>
    ///   <item><description>If <paramref name="obj"/> implements <see cref="IConvertible"/>, converts to <see cref="int"/> and creates a GUID with that as the first component.</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// Guid result1 = "550e8400-e29b-41d4-a716-446655440000".AsGUID(); // Returns the parsed GUID
    /// Guid result2 = 42.AsGUID();                                      // Returns GUID with first component = 42
    /// Guid result3 = ((object)null).AsGUID();                          // Returns Guid.Empty
    /// </code>
    /// </example>
    public static Guid AsGUID(this object? obj) => obj switch
    {
        Guid g => g,
        IHasValue f => f.Value.AsGUID(),
        null => default,
        DBNull => default,
        string s when Guid.TryParse(s, out var g) => g,
        string s when int.TryParse(s, out var i) => new Guid(i, 0, 0, new byte[8]),
        string s => default,
        uint ui => new Guid(unchecked((int)ui), 0, 0, new byte[8]),
        IConvertible c => new Guid(c.ToInt32(CultureInfo.InvariantCulture),0,0,new byte[8]),
        _ => default
    };

    /// <summary>
    /// Executes an action on an object and returns a specified value.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    /// <typeparam name="T2">The type of the object on which the action is performed.</typeparam>
    /// <param name="obj">The object on which to perform the action.</param>
    /// <param name="action">The action to perform on <paramref name="obj"/>.</param>
    /// <param name="v">The value to return after the action is executed.</param>
    /// <returns>The value <paramref name="v"/> after executing the action.</returns>
    /// <remarks>
    /// This method is useful for performing side effects on an object while returning 
    /// a value in a fluent or expression-based context.
    /// </remarks>
    /// <example>
    /// <code>
    /// var list = new List&lt;int&gt;();
    /// int count = list.SetRet(l => l.Add(42), 1); // Adds 42 to list, returns 1
    /// </code>
    /// </example>
    public static T SetRet<T, T2>(this T2 obj, Action<T2> action, T v)
    {
        action(obj);
        return v;
    }
}

/// <summary>
/// Provides extension methods for dictionary operations with 1-based indexing.
/// </summary>
/// <remarks>
/// This static class contains helper methods for <see cref="Dictionary{TKey, TValue}"/> 
/// that use 1-based indexing internally while accepting 0-based index parameters.
/// This is useful for compatibility with legacy VB6-style control arrays.
/// </remarks>
public static class ObjectHelper2
{
    /// <summary>
    /// Sets a value in the dictionary at a 1-based internal index corresponding to the given 0-based index.
    /// </summary>
    /// <typeparam name="T">The type of the values in the dictionary.</typeparam>
    /// <param name="dic">The dictionary to modify.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="index">The 0-based index. Internally stored at <paramref name="index"/> + 1.</param>
    /// <remarks>
    /// This method stores the value at key (<paramref name="index"/> + 1) to maintain 
    /// compatibility with 1-based indexing systems.
    /// </remarks>
    /// <example>
    /// <code>
    /// var dic = new Dictionary&lt;int, string&gt;();
    /// dic.SetIndex("Hello", 0); // Stores "Hello" at key 1
    /// </code>
    /// </example>
    public static void SetIndex<T>(this Dictionary<int, T> dic, T value, int index) => dic[index + 1] = value;

    /// <summary>
    /// Gets the 0-based index of a value in the dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the values in the dictionary.</typeparam>
    /// <param name="dic">The dictionary to search.</param>
    /// <param name="value">The value to find.</param>
    /// <returns>
    /// The 0-based index of the value (internal key - 1), or -1 if the value is not found.
    /// </returns>
    /// <remarks>
    /// This method searches for the value using <see cref="object.Equals(object)"/> 
    /// and returns the corresponding 0-based index.
    /// </remarks>
    /// <example>
    /// <code>
    /// var dic = new Dictionary&lt;int, string&gt; { { 1, "Hello" }, { 2, "World" } };
    /// int index = dic.GetIndex("World"); // Returns 1 (key 2 - 1)
    /// </code>
    /// </example>
    public static int GetIndex<T>(this Dictionary<int, T> dic, T value) => dic.Where((itm) => itm.Value?.Equals(value) ?? false)
        .FirstOrDefault().Key - 1;
}

/// <summary>
/// Represents a generic control array that uses 0-based external indexing with 1-based internal storage.
/// </summary>
/// <typeparam name="T">The type of elements in the control array.</typeparam>
/// <remarks>
/// <para>
/// This class extends <see cref="Dictionary{TKey, TValue}"/> to provide functionality 
/// similar to Visual Basic 6 control arrays. The external indexer uses 0-based indexing, 
/// but values are stored internally with 1-based keys.
/// </para>
/// <para>
/// The class implements <see cref="ISupportInitialize"/> to support designer initialization 
/// scenarios, though the implementation is empty.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var controls = new ControlArray&lt;Button&gt;();
/// controls[1] = new Button(); // Stored at key 2
/// var button = controls[0];   // Retrieves from key 1
/// </code>
/// </example>
public class ControlArray<T> : Dictionary<int, T>, ISupportInitialize
{
    /// <summary>
    /// Signals the object that initialization is starting.
    /// </summary>
    /// <remarks>
    /// This method is part of the <see cref="ISupportInitialize"/> interface 
    /// and is intentionally left empty as no special initialization logic is required.
    /// </remarks>
    public void BeginInit() { }

    /// <summary>
    /// Signals the object that initialization is complete.
    /// </summary>
    /// <remarks>
    /// This method is part of the <see cref="ISupportInitialize"/> interface 
    /// and is intentionally left empty as no special finalization logic is required.
    /// </remarks>
    public void EndInit() { }

    /// <summary>
    /// Gets the element at the specified 0-based index.
    /// </summary>
    /// <param name="i">The 0-based index of the element to retrieve.</param>
    /// <returns>The element stored at the internal key (<paramref name="i"/> + 1).</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when no element exists at the specified index.
    /// </exception>
    /// <remarks>
    /// This indexer provides 0-based access while internally using 1-based keys, 
    /// maintaining compatibility with VB6-style control arrays.
    /// </remarks>
    public new T this[int i] => base[i+1];
}
