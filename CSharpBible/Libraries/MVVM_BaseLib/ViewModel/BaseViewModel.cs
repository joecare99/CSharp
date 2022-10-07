// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 08-20-2022
// ***********************************************************************
// <copyright file="BaseViewModel.cs" company="MVVM_BaseLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM.ViewModel
{
	/// <summary>
	/// Class BaseViewModel.
	/// Implements the <see cref="MVVM.ViewModel.NotificationObject" />
	/// </summary>
	/// <seealso cref="MVVM.ViewModel.NotificationObject" />
	public abstract class BaseViewModel : NotificationObject {
		#region Properties
		/// <summary>
		/// Occurs when the property is changed.
		/// </summary>
#if NET5_0_OR_GREATER || NULLABLE
		private Dictionary<(string,object?), object?> PropertyOldValue = new Dictionary<(string,object?), object?>();
		protected Dictionary<string, List<object?>> KnownParams = new Dictionary<string, List<object?>>();
#else
		private Dictionary<(string,object), object> PropertyOldValue = new Dictionary<(string,object), object>();
		/// <summary>
		/// The known parameters
		/// </summary>
		protected Dictionary<string, List<object>> KnownParams = new Dictionary<string, List<object>>();
#endif
		/// <summary>
		/// The command can execute binding
		/// </summary>
		protected List<(string, string)> CommandCanExecuteBinding = new List<(string, string)>();
		#endregion

		#region Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseViewModel"/> class.
		/// </summary>
		public BaseViewModel()
		{
			PropertyChanged += InternalBindingHandler;
		}

#if NET5_0_OR_GREATER || NULLABLE
		private void InternalBindingHandler(object? sender, PropertyChangedEventArgs e)
#else
		/// <summary>
		/// Internals the binding handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
		private void InternalBindingHandler(object sender, PropertyChangedEventArgs e)
#endif
		{

			foreach (var t in CommandCanExecuteBinding)
			{
				if (t.Item1 == e.PropertyName && GetType().GetProperty(t.Item2)?.GetValue(this) is IRelayCommand rc)
					rc.NotifyCanExecuteChanged();
				else
				if (t.Item1 == e.PropertyName && GetType().GetMethod(t.Item2) is System.Reflection.MethodInfo mi)
				{

					if (mi.GetParameters().Length == 0)
                    {
						var newVal = mi.Invoke(this, new object[] { });
						NewMethod(t.Item2, newVal);
                    }
					else if (mi.GetParameters().Length == 1)
						foreach (var para in KnownParams[t.Item2])
                        {
	  				  	    var newVal = mi.Invoke(this,
#if NET5_0_OR_GREATER || NULLABLE
								new object?[] 
#else
								new object[] 
#endif
								{ para });
							NewMethod(t.Item2, newVal, para);
						}
				}
				else if (t.Item1 == e.PropertyName && GetType().GetProperty(t.Item2) is System.Reflection.PropertyInfo pi)
				{
					var newVal = pi.GetValue(this);
					NewMethod(t.Item2, newVal);
				}
			}

#if NET5_0_OR_GREATER || NULLABLE
			void NewMethod(string t, object? newVal, object? para = null )
#else
			void NewMethod(string t, object newVal, object para = null )
#endif
			{
				var oldVal = !PropertyOldValue.ContainsKey((t,para)) ? null : PropertyOldValue[(t, para)];
				if (newVal == null && oldVal == null) return;
				if (!
#if NET5_0_OR_GREATER || NULLABLE
					(newVal ?? oldVal!)
#else
					(newVal ?? oldVal)
#endif
					.Equals(newVal != null ? oldVal : null))
				{
					PropertyOldValue[(t, para)] = newVal;
					RaisePropertyChanged(t);
				}
			}
		}

#if NET5_0_OR_GREATER || NULLABLE
		protected void AppendKnownParams(object? param,[CallerMemberName] string propertyName = "")
#else
		/// <summary>
		/// Appends the known parameters.
		/// </summary>
		/// <param name="param">The parameter.</param>
		/// <param name="propertyName">Name of the property.</param>
		protected void AppendKnownParams(object param,[CallerMemberName] string propertyName = "")
#endif
		{
			if (!string.IsNullOrEmpty(propertyName))
			{
				if (!KnownParams.ContainsKey(propertyName))
					KnownParams[propertyName]=
#if NET5_0_OR_GREATER || NULLABLE
						new List<object?> { param };
#else
						new List<object> { param };
#endif
				else if (!KnownParams[propertyName].Contains(param))
 					KnownParams[propertyName].Add(param);
			}
		}
		
#endregion
	}
}

