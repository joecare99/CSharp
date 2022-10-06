// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 08-19-2022
//
// Last Modified By : Mir
// Last Modified On : 08-20-2022
// ***********************************************************************
// <copyright file="NotificationObject.cs" company="MVVM_BaseLib">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM.ViewModel
{
	/// <summary>
	/// Class NotificationObject.
	/// Implements the <see cref="INotifyPropertyChanged" />
	/// </summary>
	/// <seealso cref="INotifyPropertyChanged" />
	public class NotificationObject :  INotifyPropertyChanged
    {
#if NET5_0_OR_GREATER || NULLABLE
		public event PropertyChangedEventHandler? PropertyChanged;
#else
		/// <summary>
		/// Tritt ein, wenn sich ein Eigenschaftswert ändert.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
#endif

		/// <summary>
		/// Raises the [property changed] event.
		/// </summary>
		/// <param name="propertyName">Name of the property.
		/// If this field is not set, the [CallerMemberName] will automatically provided</param>
		protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (!string.IsNullOrEmpty(propertyName))
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Calls RaisePropertyChanged for each name in the array
		/// </summary>
		/// <param name="propertyNames">RaisePropertyChanged will be called for every element</param>
		protected void RaisePropertyChanged(params string[] propertyNames)
		{
			foreach (string s in propertyNames)
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(s));
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
		protected bool SetProperty<T>(ref T data, T value, Action<T, T> action, [CallerMemberName] string propertyName = "")
#else
		protected bool SetProperty<T>(ref T data, T value, Action<T, T> action, [CallerMemberName] string propertyName = "")
#endif
			=> SetProperty(ref data, value, null, action, propertyName);

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
		protected bool SetProperty<T>(ref T data, T value, string[]? propertyNames = null, Action<T, T>? action = null, [CallerMemberName] string propertyName = "")
#else
		protected bool SetProperty<T>(ref T data, T value, string[] propertyNames = null, Action<T, T> action = null, [CallerMemberName] string propertyName = "")
#endif
		{
			if (EqualityComparer<T>.Default.Equals(data, value)) return false;
			T old = data;
			data = value;
			RaisePropertyChanged(propertyName);
			if (propertyNames != null)
				RaisePropertyChanged(propertyNames);
			action?.Invoke(old, value);
			return true;
		}

	}
}
