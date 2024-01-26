// ***********************************************************************
// Assembly         : MVVM_BaseLibTests
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 01-26-2024
// ***********************************************************************
// <copyright file="BaseTestViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using BaseLib.Helper.MVVM;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace MVVM.ViewModel
{
    /// <summary>
    /// Class BaseTestViewModel.
    /// Implements the <see cref="MVVM.ViewModel.BaseTestViewModel" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="MVVM.ViewModel.BaseTestViewModel" />
    public class BaseTestViewModel<T> : BaseTestViewModel where T : class, INotifyPropertyChanged, new()
    {
        /// <summary>
        /// The get model
        /// </summary>
        protected Func<T> GetModel = () => new T();
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        /// <summary>
        /// The test model with events
        /// </summary>
        protected T testModel;
        /// <summary>
        /// The test model2 without events
        /// </summary>
        protected T testModel2;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        /// <summary>
        /// Initializes the test-models for this instance.
        /// </summary>
        [TestInitialize]
        public virtual void Init()
        {
            testModel = GetModel();
            testModel2 = GetModel();
            if (testModel is INotifyPropertyChanged inpc)
                inpc.PropertyChanged += OnVMPropertyChanged;
            if (testModel is INotifyPropertyChanging inpcg)
                inpcg.PropertyChanging += OnVMPropertyChanging;
            if (testModel is INotifyDataErrorInfo indei)
                indei.ErrorsChanged += OnVMErrorsChanged;
            foreach (var p in typeof(T).GetProperties())
                if (p.CanRead && p.GetValue(testModel) is IRelayCommand irc)
                    irc.CanExecuteChanged += OnCanExChanged;
        }

        protected static IEnumerable<object[]> TestModelProperies => typeof(T).GetProperties().Select(o => new object[] { o.Name, o.PropertyType.TC(), o.CanRead, o.CanWrite });

        [DataTestMethod]
        [DynamicData(nameof(TestModelProperies))]
        public virtual void TestModelProperiesTest(string sPropName, TypeCode tPropType, bool xCanRead, bool xCanWrite)
        {
            var p = typeof(T).GetProperty(sPropName);
            Assert.IsNotNull(p);
            Assert.AreEqual(xCanRead, p!.CanRead);
            Assert.AreEqual(xCanWrite, p!.CanWrite);
        }
    }

    /// <summary>
    /// Class BaseTestViewModel.
    /// Implements the <see cref="MVVM.ViewModel.BaseTestViewModel" />
    /// </summary>
    /// <seealso cref="MVVM.ViewModel.BaseTestViewModel" />
    public class BaseTestViewModel
    {
        /// <summary>
        /// The debug log
        /// </summary>
        private string _debugLog = "";

        /// <summary>
        /// Gets the debug log.
        /// </summary>
        /// <value>The debug log.</value>
        protected string DebugLog => _debugLog;

        /// <summary>
        /// Does the log.
        /// </summary>
        /// <param name="v">The v.</param>
        protected void DoLog(string v)
            => _debugLog += $"{v}{Environment.NewLine}";

        /// <summary>
        /// Clears the log.
        /// </summary>
        protected void ClearLog() => _debugLog = "";

        /// <summary>
        /// Handles the <see cref="E:CanExecuteChanged" /> event of the command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnCanExChanged(object? sender, EventArgs e)
            => DoLog($"CanExChanged({sender})={(sender as IRelayCommand)?.CanExecute(null)}");

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event of the model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVMPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => DoLog($"PropChg({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? "")}");

        /// <summary>
        /// Handles the <see cref="E:PropertyChanging" /> event of the model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVMPropertyChanging(object? sender, PropertyChangingEventArgs e)
            => DoLog($"PropChgn({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? "")}");

        /// <summary>
        /// Handles the <see cref="E:ErrorsChanged" /> event of the model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataErrorsChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVMErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
            => DoLog($"ErrorsChanged({sender},{e.PropertyName})={string.Join(",", ((List<ValidationResult>)(sender as INotifyDataErrorInfo)!.GetErrors(e.PropertyName)).ConvertAll(o => o.ErrorMessage))}");
    }
}
