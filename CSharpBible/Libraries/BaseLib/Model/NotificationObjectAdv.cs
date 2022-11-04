// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 08-19-2022
//
// Last Modified By : Mir
// Last Modified On : 08-20-2022
// ***********************************************************************
// <copyright file="NotificationObject.cs" company="MVVM_BaseLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Baselib.Model
{
    /// <summary>
    /// Class NotificationObject.
    /// Implements the <see cref="INotifyPropertyChangedAdv" />
    /// </summary>
    /// <seealso cref="INotifyPropertyChangedAdv" />
    public class NotificationObjectAdv :  INotifyPropertyChangedAdv
    {
#if NET5_0_OR_GREATER || NULLABLE
		public event PropertyChangedAdvEventHandler? PropertyChangedAdv;
#else
		/// <summary>
		/// Tritt ein, wenn sich ein Eigenschaftswert ändert.
		/// </summary>
		public event PropertyChangedAdvEventHandler PropertyChangedAdv;
#endif

		/// <summary>
		/// Raises the [property changed] event.
		/// </summary>
		/// <param name="propertyName">Name of the property.
		/// If this field is not set, the [CallerMemberName] will automatically provided</param>
		protected void RaisePropertyChangedAdv(object oldVal,object newVal,[CallerMemberName] string propertyName = "")
		{
			if (!string.IsNullOrEmpty(propertyName))
				this.PropertyChangedAdv?.Invoke(this, new PropertyChangedAdvEventArgs(propertyName,oldVal,newVal));
		}

		/// <summary>
		/// Calls RaisePropertyChanged for each name in the array
		/// </summary>
		/// <param name="propertyNames">RaisePropertyChanged will be called for every element</param>
		protected void RaisePropertyChangedAdv(params string[] propertyNames) //??
		{
			foreach (string s in propertyNames)
				this.PropertyChangedAdv?.Invoke(this, new PropertyChangedAdvEventArgs(s,null,null));
		}

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
		protected bool SetProperty<T>(ref T data, T value, Action<T, T>? action, [CallerMemberName] string propertyName = "")
#else
		protected bool SetProperty<T>(ref T data, T value, Action<T, T> action, [CallerMemberName] string propertyName = "")
#endif
			=> SetProperty(ref data, value, null, null, action, propertyName);

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
		protected bool SetProperty<T>(ref T data, T value,Predicate<T>? validate, Action<T, T>? action=null, [CallerMemberName] string propertyName = "")
#else
        protected bool SetProperty<T>(ref T data, T value,Predicate<T> validate, Action<T, T> action=null, [CallerMemberName] string propertyName = "")
#endif
            => SetProperty(ref data, value, null,validate, action, propertyName);


        /// <summary>
        /// Helper for setting properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyNames">The property names.</param>
        /// <param name="action">The action.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET5_0_OR_GREATER || NULLABLE
		protected bool SetProperty<T>(ref T data, T value, string[]? propertyNames, Action<T, T>? action, [CallerMemberName] string propertyName = "")
#else
        protected bool SetProperty<T>(ref T data, T value, string[] propertyNames, Action<T, T> action, [CallerMemberName] string propertyName = "")
#endif
         => SetProperty(ref data, value, propertyNames, null, action, propertyName);

        /// <summary>
        /// Helper for setting properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyNames">The property names.</param>
        /// <param name="action">The action.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
#if NET5_0_OR_GREATER || NULLABLE
		protected bool SetProperty<T>(ref T data, T value, string[]? propertyNames=null, Predicate<T>? validate=null,Action<T, T>? action = null, [CallerMemberName] string propertyName = "")
#else
        protected bool SetProperty<T>(ref T data, T value, string[] propertyNames=null ,Predicate<T> validate=null,Action<T, T> action = null, [CallerMemberName] string propertyName = "")
#endif
        {
            if (EqualityComparer<T>.Default.Equals(data, value)) return false;
			if (!validate?.Invoke(value) ?? false) return false; 
            T old = data;
            data = value;
            RaisePropertyChangedAdv(old, value, propertyName);
            if (propertyNames != null)
                RaisePropertyChangedAdv(propertyNames);
            action?.Invoke(old, value);
            return true;
        }

    }
}
