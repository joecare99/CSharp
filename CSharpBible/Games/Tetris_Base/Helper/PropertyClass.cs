using System;
using System.Collections.Generic;

namespace Tetris_Base.Helper {
	/// <summary>
	/// Class PropertyClass.
	/// </summary>
	public class PropertyClass {

		/// <summary>
		/// Helper for setting properties
		/// </summary>
		/// <typeparam name="T">Generic type of the method</typeparam>
		/// <param name="data">the data-field that is about to be changed</param>
		/// <param name="value">the new data-value</param>
		/// <param name="action">the action is executed right <b>after</b> data is changed</param>
		/// <param name="preAction">the action is executed right <b>before</b> data is changed</param>
		/// <returns></returns>
		public static bool SetProperty<T>(ref T data, T value, Action<T, T>? action = null, Action<T, T>? preAction = null) {
			//  if (value == null && data == null) return false;
			//  if ((data ?? value).Equals((data == null) ? data : value)) return false;
			if (EqualityComparer<T>.Default.Equals(data, value)) return false;
			var old = data;
			preAction?.Invoke(data, value);
			data = value;
			action?.Invoke(old, value);
			return true;
		}

		/// <summary>
		/// Helper for setting properties
		/// </summary>
		/// <typeparam name="T">Generic type of the method</typeparam>
		/// <param name="data">the data-field that is about to be changed</param>
		/// <param name="value">the new data-value</param>
		/// <param name="preAction">the action is executed right <b>before</b> data is changed</param>
		/// <returns></returns>
		public static bool SetPropertyP<T>(ref T data, T value, Action<T, T>? preAction = null) =>
			SetProperty<T>(ref data, value, null ,preAction);
	
	}
}
