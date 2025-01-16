﻿// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="ClassHelper.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The Helper namespace.
/// </summary>
/// <autogeneratedoc />
namespace BaseLib.Helper;

/// <summary>
/// Class ClassHelper.
/// </summary>
/// <autogeneratedoc />
public static class ClassHelper
{
    /// <summary>
    /// is it a property ?.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cls">The CLS.</param>
    /// <param name="propName">Name of the possible property.</param>
    /// <returns>bool</returns>
    public static bool IsProperty<T>(this T cls, string? propName) where T : class
        => cls.GetType().GetProperty(propName ??"") != null;

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cls">The CLS.</param>
    /// <param name="propName">Name of the property.</param>
    /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
    /// <autogeneratedoc />
    public static object? GetProp<T>(this T cls, string propName) where T : class 
        => cls.GetType().GetProperty(propName)?.GetValue(cls);
    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <param name="cls">The CLS.</param>
    /// <param name="propName">Name of the property.</param>
    /// <param name="def">The definition.</param>
    /// <returns>System.Nullable&lt;T2&gt;.</returns>
    /// <autogeneratedoc />
    public static T2? GetProp<T,T2>(this T cls, string propName, T2 def) where T : class
        => (T2?)(cls.GetType().GetProperty(propName, def?.GetType()) ??
                 cls.GetType().GetProperty(propName, typeof(T2)))?.GetValue(cls) ?? def;
    /// <summary>
    /// Sets the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <param name="cls">The CLS.</param>
    /// <param name="propName">Name of the property.</param>
    /// <param name="newVal">The new value.</param>
    /// <autogeneratedoc />
    public static void SetProp<T, T2>(this T cls, string propName, T2 newVal) where T : class
        => ((newVal == null ? null: cls.GetType().GetProperty(propName, newVal.GetType())) ??
            cls.GetType().GetProperty(propName, typeof(T2)))?.SetValue(cls, newVal);
    /// <summary>
    /// Gets the field.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cls">The CLS.</param>
    /// <param name="fieldName">Name of the property.</param>
    /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
    /// <autogeneratedoc />
    /// 
    public static object? GetField<T>(this T cls, string fieldName) where T : class
        => cls.GetType().GetField(fieldName)?.GetValue(cls);

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ar">The ar.</param>
    /// <param name="search">The search.</param>
    /// <returns>System.Int32.</returns>
    /// <autogeneratedoc />
    public static int IndexOf<T>(this IEnumerable<T> ar, T search)
        =>  ar.ToList().IndexOf(search);

    /// <summary>
    /// Enums the member.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <param name="enumMbr">The enum MBR.</param>
    /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
    /// <autogeneratedoc />
    public static object? EnumMember(this Type t, string enumMbr)
        => t.GetEnumNames().ToList().IndexOf(enumMbr) is int i && i >= 0? t.GetEnumValues().GetValue(i) : default;
}
