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
	public abstract class BaseViewModel : NotificationObject, IPropertyBinding
	{
		#region Properties
		/// <summary>
		/// Occurs when the property is changed.
		/// </summary>
#if NET5_0_OR_GREATER || NULLABLE
		private Dictionary<(string, object?), object?> _PropertyOldValue = new();
		protected Dictionary<string, List<object?>> KnownParams = new();
#else
		private Dictionary<(string,object), object> _PropertyOldValue = new Dictionary<(string,object), object>();
		/// <summary>
		/// The known parameters
		/// </summary>
		protected Dictionary<string, List<object>> KnownParams = new Dictionary<string, List<object>>();
#endif
        /// <summary>
        /// The command can execute binding
        /// </summary>
        private Dictionary<string, List<string>> _CommandCanExecuteBinding = new ();
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

			if (sender == this && _CommandCanExecuteBinding.TryGetValue(e.PropertyName, out var l))
			foreach (var t in l)
				switch (GetType().GetProperty(t))
				{
					case System.Reflection.PropertyInfo pi when pi.GetValue(this) is IRelayCommand rc:
						rc.NotifyCanExecuteChanged();
						break;

					case System.Reflection.PropertyInfo pi:
						{
							var newVal = pi.GetValue(this);
							NewMethod(t, newVal);
						}
						break;
					default:
						if (GetType().GetMethod(t) is System.Reflection.MethodInfo mi)
						{
							if (mi.GetParameters().Length == 0)
							{
								var newVal = mi.Invoke(this, new object[] { });
								NewMethod(t, newVal);
							}
							else if (mi.GetParameters().Length == 1)
							{
								if (KnownParams.ContainsKey(t))
									foreach (var para in KnownParams[t])
									{
										var newVal = mi.Invoke(this,
#if NET5_0_OR_GREATER || NULLABLE
										new object?[]
#else
								new object[] 
#endif
										{ para });
										NewMethod(t, newVal, para);
									}
							}
						}
						break;
				}


#if NET5_0_OR_GREATER || NULLABLE
			void NewMethod(string t, object? newVal, object? para = null)
#else
			void NewMethod(string t, object newVal, object para = null )
#endif
			{
				var oldVal = !_PropertyOldValue.ContainsKey((t, para)) ? null : _PropertyOldValue[(t, para)];
				if (newVal == null && oldVal == null) return;
				if (!
#if NET5_0_OR_GREATER || NULLABLE
					(newVal ?? oldVal!)
#else
					(newVal ?? oldVal)
#endif
					.Equals(newVal != null ? oldVal : null))
				{
					_PropertyOldValue[(t, para)] = newVal;
					RaisePropertyChanged(t);
				}
			}
		}

        public bool AddPropertyDependency(string prop1, string prop2)
        {
			bool flag =
				GetType().GetProperty(prop1) is System.Reflection.PropertyInfo
				|| GetType().GetMethod(prop1) is System.Reflection.MethodInfo mi
					&& mi.GetParameters().Length is 0 or 1;
			if (flag && _CommandCanExecuteBinding.TryGetValue(prop2, out var l))
				l.Add(prop1);
			else if (flag) 
				_CommandCanExecuteBinding[prop2] = new() { prop1 };
            return flag;
        }

        public bool RemovePropertyDependency(string prop1, string prop2)
        {
            throw new System.NotImplementedException();
        }

		/// <summary>
		/// Appends the known parameters.
		/// </summary>
		/// <param name="param">The parameter.</param>
		/// <param name="propertyName">Name of the property.</param>
#if NET5_0_OR_GREATER || NULLABLE
		public void AppendKnownParams(object? param,[CallerMemberName] string propertyName = "")
#else
		public void AppendKnownParams(object param,[CallerMemberName] string propertyName = "")
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

        protected void CommandCanExecuteBindingClear() 
			=> _CommandCanExecuteBinding.Clear();
        #endregion
    }
}

