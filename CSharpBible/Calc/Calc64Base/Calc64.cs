﻿// ***********************************************************************
// Assembly         : Calc64Base
// Author           : Mir
// Created          : 08-27-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="Calc64.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calc64Base
{
    /// <summary>
    /// a 64 bit calculator-class
    /// </summary>
    public class Calc64
    {
        #region Properties
        #region private properties
        /// <summary>
        /// The accumulator
        /// </summary>
        private long _accumulator;
        /// <summary>
        /// The memory
        /// </summary>
        private long _memory;
        /// <summary>
        /// The register
        /// </summary>
        private long _register;

        /// <summary>
        /// The calculate operations
        /// </summary>
        private static Dictionary<string,CalcOperation> _calcOperations = new Dictionary<string, CalcOperation>();
        /// <summary>
        /// The calculate operations identifier
        /// </summary>
        private static Dictionary<int, CalcOperation> _calcOperationsID = new Dictionary<int, CalcOperation>();
        #endregion
        /// <summary>
        /// Gets the short desciptions.
        /// </summary>
        /// <value>The short desciptions.</value>
        public Dictionary<string, CalcOperation>.KeyCollection ShortDesciptions => _calcOperations.Keys;
        /// <summary>
        /// Gets the i ds.
        /// </summary>
        /// <value>The i ds.</value>
        public Dictionary<int, CalcOperation>.KeyCollection IDs => _calcOperationsID.Keys;
        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>The operations.</value>
        public Dictionary<int, CalcOperation>.ValueCollection Operations => _calcOperationsID.Values;
        /// <summary>
        /// Gets or sets the accumulator.
        /// </summary>
        /// <value>The accumulator.</value>
        public Int64 Accumulator { get=>_accumulator; set=> Property.SetProperty(ref _accumulator, value,PropChange); }
        /// <summary>
        /// Gets or sets the memory.
        /// </summary>
        /// <value>The memory.</value>
        public Int64 Memory { get => _memory; set => Property.SetProperty(ref _memory, value, PropChange); }
        /// <summary>
        /// Gets or sets the register.
        /// </summary>
        /// <value>The register.</value>
        public Int64 Register { get => _register; set => Property.SetProperty(ref _register, value, PropChange); }
#if NET5_0_OR_GREATER
        public Exception? LastError { get; set; }
#else
        /// <summary>
        /// Gets or sets the last error.
        /// </summary>
        /// <value>The last error.</value>
        public Exception LastError { get; set; }
#endif
        /// <summary>
        /// Occurs when [calculate operation changed].
        /// </summary>
        public event EventHandler<(string prop,object oldVal,object newVal )> CalcOperationChanged;
        /// <summary>
        /// Occurs when [calculate operation error].
        /// </summary>
        public event EventHandler<Exception> CalcOperationError;
        #endregion

        #region Methods
        #region static Methods
        /// <summary>
        /// Initializes static members of the <see cref="Calc64"/> class.
        /// </summary>
        static Calc64() {
            foreach (var op in StandardOperations.GetAll())
                RegisterOperation(op);
        }

        /// <summary>
        /// Registers the operation.
        /// </summary>
        /// <param name="calcOperation">The calculate operation.</param>
        public static void RegisterOperation(CalcOperation calcOperation)
        {
            if (!_calcOperations.ContainsKey(calcOperation.ShortDescr))
                _calcOperations.Add(calcOperation.ShortDescr,calcOperation);
            if (_calcOperationsID.ContainsKey(calcOperation.ID))
                calcOperation.SetID( _calcOperationsID.Keys.Max()+1);
            _calcOperationsID.Add(calcOperation.ID, calcOperation);
        }

        /// <summary>
        /// Determines whether [is register opeation] [the specified co].
        /// </summary>
        /// <param name="co">The co.</param>
        /// <returns><c>true</c> if [is register opeation] [the specified co]; otherwise, <c>false</c>.</returns>
        public static bool IsRegisterOpeation(
#if NET5_0_OR_GREATER
     CalcOperation?
#else
     CalcOperation
#endif
 co) => co?.NeedRegister ?? false;
        /// <summary>
        /// Determines whether [is register opeation] [the specified short desc].
        /// </summary>
        /// <param name="shortDesc">The short desc.</param>
        /// <returns><c>true</c> if [is register opeation] [the specified short desc]; otherwise, <c>false</c>.</returns>
        public static bool IsRegisterOpeation(string shortDesc) => IsRegisterOpeation(ToCalcOperation(shortDesc));
        /// <summary>
        /// Determines whether [is register opeation] [the specified identifier].
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if [is register opeation] [the specified identifier]; otherwise, <c>false</c>.</returns>
        public static bool IsRegisterOpeation(int id) => IsRegisterOpeation(ToCalcOperation(id));

        /// <summary>
        /// Converts to calcoperation.
        /// </summary>
        /// <param name="shortDesc">The short desc.</param>
        /// <returns>CalcOperation.</returns>
        public static
#if NET5_0_OR_GREATER
     CalcOperation?
#else
     CalcOperation
#endif
 ToCalcOperation(string shortDesc) =>
            (shortDesc != null && _calcOperations.ContainsKey(shortDesc)) ?
             _calcOperations[shortDesc] : null;

        /// <summary>
        /// Converts to calcoperation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>CalcOperation.</returns>
        public static
#if NET5_0_OR_GREATER
     CalcOperation?
#else
     CalcOperation
#endif
 ToCalcOperation(int id) =>
            _calcOperationsID.ContainsKey(id) ?  _calcOperationsID[id] : null;

        /// <summary>
        /// Properties the change.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop">The property.</param>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        protected void PropChange<T>(string prop, T oldVal, T newVal)
        {
            CalcOperationChanged?.Invoke(this, (prop, oldVal, newVal));
        }
        #endregion

        /// <summary>
        /// Does the opeation.
        /// </summary>
        /// <param name="co">The co.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool DoOpeation(
 #if NET5_0_OR_GREATER
     CalcOperation?
#else
     CalcOperation
        #endif
             co)
        {
            if (co == null) return false;
            LastError = null;
            var arg = CalcOperation.CreateArguments(co);
            var argCount = 0;
            if (co.NeedAccumulator) arg[argCount++] = _accumulator;
            if (co.NeedRegister) arg[argCount++] = _register;
            if (co.NeedMemory) arg[argCount++] = _memory;
            try
            {
                if (co.Execute(ref arg))
                {
                    argCount = 0;
                    if (co.NeedAccumulator) Accumulator = (Int64)arg[argCount++];
                    if (co.NeedRegister) Register = (Int64)arg[argCount++];
                    if (co.NeedMemory) Memory = (Int64)arg[argCount++];
                    return true;
                }
            }
            catch (Exception e)
            {
                LastError = e;
                CalcOperationError?.Invoke(this, e);
                return false;
            }
            return false;

        }
        /// <summary>
        /// Does the opeation.
        /// </summary>
        /// <param name="shortDesc">The short desc.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool DoOpeation(string shortDesc) => DoOpeation(ToCalcOperation(shortDesc));
        /// <summary>
        /// Does the opeation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool DoOpeation(int id) => DoOpeation(ToCalcOperation(id));

#endregion
    }
}
