﻿// ***********************************************************************
// Assembly         : MVVM_BaseLib
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

namespace BaseLib.Helper
{
	/// <summary>
	/// Class Property.
	/// </summary>
	public static class Property
    {
        /// <summary>
        /// Helper for setting properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

#if NET5_0_OR_GREATER || NULLABLE
		public static bool SetProperty<T>(ref T data,T value, Action<string, T, T> action, [CallerMemberName] string propertyName = "")
#else
        public static bool SetProperty<T>(ref T data, T value, Action<string, T, T> action, [CallerMemberName] string propertyName = "")
#endif
            => SetProperty(ref data,value,null,action,propertyName);

#if NET5_0_OR_GREATER || NULLABLE
		public static bool SetProperty<T>(this T value,ref T data, Action<string, T, T>? action, [CallerMemberName] string propertyName = "")
#else
        public static bool SetProperty<T>(this T value, ref T data, Action<string, T, T> action, [CallerMemberName] string propertyName = "")
#endif
            => SetProperty(ref data, value, null, action, propertyName);


#if NET5_0_OR_GREATER || NULLABLE
		public static bool SetProperty<T>(this T value,ref T data, Predicate<T>? validate=null, Action<string, T, T>? action = null, [CallerMemberName] string propertyName = "")
#else
    public static bool SetProperty<T>(this T value, ref T data, Predicate<T> validate=null, Action<string, T, T> action = null, [CallerMemberName] string propertyName = "")
#endif
			=> SetProperty(ref data,value, validate, action,propertyName);

    }
}
