// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 06-17-2022
//
// Last Modified By : Mir
// Last Modified On : 06-17-2022
// ***********************************************************************
// <copyright file="IObservableCollection.cs" company="MVVM_BaseLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MVVM.ViewModel; 
	/// <summary>
	/// Represents a collection that is observable.
	/// </summary>
	/// <typeparam name="T">The type of elements contained in the collection.</typeparam>
	public interface IObservableCollection<T> : IList<T>, /*INotifyPropertyChangedEx,*/ INotifyCollectionChanged {
		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="items">The items.</param>
		void AddRange(IEnumerable<T> items);

		/// <summary>
		/// Removes the range.
		/// </summary>
		/// <param name="items">The items.</param>
		void RemoveRange(IEnumerable<T> items);
	}
