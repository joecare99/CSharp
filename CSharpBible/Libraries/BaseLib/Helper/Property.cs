﻿// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 08-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-10-2022
// ***********************************************************************
// <copyright file="Property.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// The Helper namespace of the BaseLib.
/// </summary>
/// <autogeneratedoc />
namespace BaseLib.Helper
{
	/// <summary>
	/// Class Property.
	/// </summary>
	public static class Property
    {
        /// <summary>
        /// Helper for setting properties
        /// <list type="bullet"><item>first tests the new <see cref="value" />if it differs from the old value in <see cref="data" /></item>
        /// <item>calles the validation predicate <see cref="validate" /> if it is not <see cref="null" />. </item>
        /// <list type="bullet"><item>if <c>true</c> the procedure continues</item>
        /// <item>if <c>false</c> the procedure stops and returns false</item>
        /// <item>an excepton in the validation also stops the procedure. The exception will NOT be handeled.</item>
        /// </list>
        /// <item> then <see cref="data" /> will be set to (new) <see cref="value" /></item>
        /// <item>at last calles the <see cref="action" /> (if not null). Exceptions in the action <strong>will</strong> be handeled (omitted)</item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The generic Type of the property</typeparam>
        /// <param name="data">The reference to the data-holder .</param>
        /// <param name="value">The (new) value.</param>
        /// <param name="validate">The validation predicate.</param>
        /// <param name="action">The action to be called on success.</param>
        /// <param name="propertyName">Name of the property that has to be changed.</param>
        /// <returns>
        ///   <c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET5_0_OR_GREATER || NULLABLE
		public static bool SetProperty<T>(ref T data,T value, Predicate<T>? validate=null, Action<string, T, T>? action = null, [CallerMemberName] string propertyName = "")
#else
        public static bool SetProperty<T>(ref T data,T value, Predicate<T> validate = null, Action<string, T, T> action = null, [CallerMemberName] string propertyName = "")
#endif
		{
			if (EqualityComparer<T>.Default.Equals(data, value)) return false;
            if (!validate?.Invoke(value) ?? false) return false;
            T old = data;
			data = value;
            try { 
			action?.Invoke(propertyName, old, value);
            }
            catch { }
			return true;
		}

        /// <summary>
        /// Sets the property <see cref="SetProperty{T}(ref T, T, Predicate{T}, Action{string, T, T}, string)" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET5_0_OR_GREATER || NULLABLE
		public static bool SetProperty<T>(ref T data,T value, Action<string, T, T> action, [CallerMemberName] string propertyName = "")
#else
        public static bool SetProperty<T>(ref T data, T value, Action<string, T, T> action, [CallerMemberName] string propertyName = "")
#endif
            => SetProperty(ref data,value,null,action,propertyName);

        /// <summary>
        /// Sets the property <see cref="SetProperty{T}(ref T, T, Predicate{T}, Action{string, T, T}, string)" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="data">The data.</param>
        /// <param name="action">The action.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET5_0_OR_GREATER || NULLABLE
		public static bool SetProperty<T>(this T value,ref T data, Action<string, T, T>? action, [CallerMemberName] string propertyName = "")
#else
        public static bool SetProperty<T>(this T value, ref T data, Action<string, T, T> action, [CallerMemberName] string propertyName = "")
#endif
            => SetProperty(ref data, value, null, action, propertyName);


        /// <summary>
        /// Sets the property <see cref="SetProperty{T}(ref T, T, Predicate{T}, Action{string, T, T}, string)" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="data">The data.</param>
        /// <param name="validate">The validation predicate.</param>
        /// <param name="action">The action.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET5_0_OR_GREATER || NULLABLE
		public static bool SetProperty<T>(this T value,ref T data, Predicate<T>? validate=null, Action<string, T, T>? action = null, [CallerMemberName] string propertyName = "")
#else
        public static bool SetProperty<T>(this T value, ref T data, Predicate<T> validate=null, Action<string, T, T> action = null, [CallerMemberName] string propertyName = "")
#endif
			=> SetProperty(ref data,value, validate, action,propertyName);

    }
}
