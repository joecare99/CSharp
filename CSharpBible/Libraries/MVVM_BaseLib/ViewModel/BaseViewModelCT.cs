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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MVVM.ViewModel
{
    /// <summary>
    /// Class BaseViewModel.
    /// Implements the <see cref="ObservableObject" />
    /// </summary>
    /// <seealso cref="ObservableObject" />
    public abstract class BaseViewModelCT : ObservableObject , IPropertyBinding
    {
        #region Properties
        /// <summary>
        /// Occurs when the property is changed.
        /// </summary>
		private readonly Dictionary<(string, object?), object?> _PropertyOldValue = new();

        /// <summary>
        /// The known parameters
        /// </summary>
		protected Dictionary<string, List<object?>> KnownParams = new();
        /// <summary>
        /// The command can execute binding
        /// </summary>
        private readonly Dictionary<string, List<(string,bool)>> _CommandCanExecuteBinding = new();
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        public BaseViewModelCT()
        {
            PropertyChanged += InternalBindingHandler;
        }

        /// <summary>
        /// Internals the binding handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void InternalBindingHandler(object? sender, PropertyChangedEventArgs e)
        {

            if (sender == this && _CommandCanExecuteBinding.TryGetValue(e.PropertyName ?? "", out var l))
                foreach (var t in l)
                    switch (GetType().GetProperty(t.Item1))
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
                            if (GetType().GetMethod(t.Item1) is System.Reflection.MethodInfo mi)
                            {
                                if (mi.GetParameters().Length == 0)
                                {
                                    var newVal = mi.Invoke(this, Array.Empty<object>());
                                    NewMethod(t, newVal);
                                }
                                else if (mi.GetParameters().Length == 1)
                                {
                                    if (KnownParams.ContainsKey(t.Item1))
                                        foreach (var para in KnownParams[t.Item1])
                                        {
                                            var newVal = mi.Invoke(this,
										new object?[]
                                            { para });
                                            NewMethod(t, newVal, para);
                                        }
                                }
                            }
                            break;
                    }

			void NewMethod((string, bool) t, object? newVal, object? para = null)
            {
                var oldVal = !_PropertyOldValue.ContainsKey((t.Item1, para)) ? null : _PropertyOldValue[(t.Item1, para)];
                if (newVal == null && oldVal == null) return;
                if (t.Item2 || !(newVal ?? oldVal!).Equals(newVal != null ? oldVal : null))
                {
                    if (!t.Item2)
                        _PropertyOldValue[(t.Item1, para)] = newVal;
                    OnPropertyChanged(t.Item1);
                }
            }
        }

        public bool AddPropertyDependency(string prop1, string prop2,bool xForce=false )
        {
            bool flag =
                GetType().GetProperty(prop1) is not null
                || GetType().GetMethod(prop1) is System.Reflection.MethodInfo mi
                    && mi.GetParameters().Length is 0 or 1;
            if (flag && _CommandCanExecuteBinding.TryGetValue(prop2, out var l))
                l.Add((prop1,xForce));
            else if (flag)
                _CommandCanExecuteBinding[prop2] = new() { (prop1, xForce) };
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
		public void AppendKnownParams(object? param,[CallerMemberName] string propertyName = "")
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (!KnownParams.ContainsKey(propertyName))
                    KnownParams[propertyName] =
						new List<object?> { param };
                else if (!KnownParams[propertyName].Contains(param))
                    KnownParams[propertyName].Add(param);
            }
        }

        public T FuncProxy<T,T2>(T2 param, Func<T2, T> f, [CallerMemberName] string propertyName = "")
        {
            if (! new[] { typeof(double), typeof(float) }.Contains(typeof(T2)))
                AppendKnownParams(param, propertyName);
            return f(param);
        }

        protected void CommandCanExecuteBindingClear()
            => _CommandCanExecuteBinding.Clear();
        #endregion
    }
}

