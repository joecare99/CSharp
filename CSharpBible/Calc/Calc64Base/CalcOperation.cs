﻿// ***********************************************************************
// Assembly         : Calc64Base
// Author           : Mir
// Created          : 08-27-2022
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="CalcOperation.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using System;

/// <summary>
/// The Calc64Base namespace.
/// </summary>
/// <autogeneratedoc />
namespace Calc64Base
{

    /// <summary>
    /// Class CalcOperation.
    /// </summary>
    public abstract class CalcOperation
    {
        /// <summary>
        /// Struct ClcOpSetting
        /// </summary>
        public struct ClcOpSetting
        {
            /// <summary>
            /// The need accumulator
            /// </summary>
            public bool NeedAccumulator;
            /// <summary>
            /// The need register
            /// </summary>
            public bool NeedRegister;
            /// <summary>
            /// The need memory
            /// </summary>
            public bool NeedMemory;
            /// <summary>
            /// Initializes a new instance of the <see cref="ClcOpSetting" /> struct.
            /// </summary>
            /// <param name="a">if set to <c>true</c> [a].</param>
            /// <param name="r">if set to <c>true</c> [r].</param>
            /// <param name="m">if set to <c>true</c> [m].</param>
            public ClcOpSetting(bool a = false, bool r = false, bool m = false)
            {
                NeedAccumulator = a; 
                NeedRegister = r; 
                NeedMemory = m;
            }

            /// <summary>
            /// Gets the argument count.
            /// </summary>
            /// <value>The argument count.</value>
            public int ArgCount => 
                (NeedAccumulator ? 1 : 0) + (NeedMemory ? 1 : 0) + (NeedRegister ? 1 : 0);
        }

        #region Properties
        #region private Properties
        /// <summary>
        /// The short description
        /// </summary>
        private string _shortDescr;
        /// <summary>
        /// The identifier
        /// </summary>
        private int _id;
        /// <summary>
        /// The last identifier
        /// </summary>
        private static int _lastID=0;
        /// <summary>
        /// The long description
        /// </summary>
        private string _longDescr;
        /// <summary>
        /// The setting
        /// </summary>
        private ClcOpSetting _setting;
        #endregion
        /// <summary>
        /// Gets or sets a value indicating whether [need accumulator].
        /// </summary>
        /// <value><c>true</c> if [need accumulator]; otherwise, <c>false</c>.</value>
        public bool NeedAccumulator { get=>_setting.NeedAccumulator; protected set=> _setting.NeedAccumulator=value; }
        /// <summary>
        /// Gets or sets a value indicating whether [need register].
        /// </summary>
        /// <value><c>true</c> if [need register]; otherwise, <c>false</c>.</value>
        public bool NeedRegister { get => _setting.NeedRegister; protected set => _setting.NeedRegister = value; }
        /// <summary>
        /// Gets or sets a value indicating whether [need memory].
        /// </summary>
        /// <value><c>true</c> if [need memory]; otherwise, <c>false</c>.</value>
        public bool NeedMemory { get => _setting.NeedMemory; protected set => _setting.NeedMemory = value; }
        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <value>The setting.</value>
        public ClcOpSetting Setting => _setting;
        /// <summary>
        /// Gets the short description.
        /// </summary>
        /// <value>The short description.</value>
        public string ShortDescr => _shortDescr;
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int ID { get => _id; private set => _id = value; }

        /// <summary>
        /// Gets the long description.
        /// </summary>
        /// <value>The long description.</value>
        public string LongDescr => _longDescr;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CalcOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="id">The identifier.</param>
        protected CalcOperation(string shortDescr, string longDescr, int id = -1 )
        {
            this._shortDescr = shortDescr;
            this._longDescr = longDescr;
            if (id == -1)
            {
                ID = ++_lastID;
            }
            else
            {
                _lastID = 
                ID =  id;
            }
        }

        /// <summary>
        /// Executes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool Execute(ref object[] o);

        /// <summary>
        /// Creates the arguments.
        /// </summary>
        /// <param name="co">The co.</param>
        /// <returns>System.Object[].</returns>
        public static object[] CreateArguments(CalcOperation co) => new object[co.Setting.ArgCount];

        /// <summary>
        /// Sets the identifier.
        /// </summary>
        /// <param name="v">The v.</param>
        internal void SetID(int v) => ID = v;
        #endregion
    }

    /// <summary>
    /// Class UnaryOperation.
    /// Implements the <see cref="Calc64Base.CalcOperation" />
    /// </summary>
    /// <seealso cref="Calc64Base.CalcOperation" />
    public class UnaryOperation : CalcOperation
    {
        #region Properties
        /// <summary>
        /// The function
        /// </summary>
        private Func<Int64, Int64> _func;

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        public Func<Int64, Int64> Function { get => _func; set=> Property.SetProperty(ref _func, value);}
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public UnaryOperation(string shortDescr,int id, string longDescr, Func<Int64, Int64> func) : base(shortDescr, longDescr, id)
        {
            NeedAccumulator = true;
            _func = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public UnaryOperation(string shortDescr, string longDescr, Func<Int64, Int64> func) : this(shortDescr, -1, longDescr, func) { }

        /// <summary>
        /// Executes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool Execute(ref object[] o)
        {
            if (o.Length>0 && o[0] is Int64 akk)
            {
                o[0] = _func?.Invoke(akk) ?? akk;
                return true;
            }
            return false;
        }
        #endregion
    }


    /// <summary>
    /// Class BinaryOperation.
    /// Implements the <see cref="Calc64Base.CalcOperation" />
    /// </summary>
    /// <seealso cref="Calc64Base.CalcOperation" />
    public class BinaryOperation : CalcOperation
    {
        /// <summary>
        /// The function
        /// </summary>
        protected Func<Int64, Int64, Int64> _func;

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        public Func<Int64, Int64, Int64> Function { get => _func; set => Property.SetProperty(ref _func, value); }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public BinaryOperation(string shortDescr,int id, string longDescr, Func<Int64, Int64, Int64> func) : base(shortDescr, longDescr,id)
        {
            NeedRegister = true;
            NeedAccumulator = true;
            _func = func;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public BinaryOperation(string shortDescr, string longDescr, Func<Int64, Int64, Int64> func) : this(shortDescr, -1, longDescr, func) { }

        /// <summary>
        /// Executes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool Execute(ref object[] o)
        {
            if (o.Length>1 && o[0] is Int64 akk && o[1] is Int64 reg)
            {
                o[0] = _func?.Invoke(akk,reg) ?? akk;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Class FromMemOperation.
    /// Implements the <see cref="Calc64Base.BinaryOperation" />
    /// </summary>
    /// <seealso cref="Calc64Base.BinaryOperation" />
    public class FromMemOperation : BinaryOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromMemOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public FromMemOperation(string shortDescr, string longDescr, Func<Int64, Int64, Int64> func) : this(shortDescr, -1, longDescr, func) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="FromMemOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public FromMemOperation(string shortDescr, int id, string longDescr, Func<Int64, Int64, Int64> func) : base(shortDescr, id, longDescr, func)
        {
            NeedRegister = false;
            NeedMemory = true;
        }
    }

    /// <summary>
    /// Class ToMemOperation.
    /// Implements the <see cref="Calc64Base.BinaryOperation" />
    /// </summary>
    /// <seealso cref="Calc64Base.BinaryOperation" />
    public class ToMemOperation : BinaryOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToMemOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public ToMemOperation(string shortDescr, string longDescr, Func<Int64, Int64, Int64> func) : this(shortDescr, -1, longDescr, func) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToMemOperation" /> class.
        /// </summary>
        /// <param name="shortDescr">The short description.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="longDescr">The long description.</param>
        /// <param name="func">The function.</param>
        public ToMemOperation(string shortDescr, int id, string longDescr, Func<Int64, Int64, Int64> func) : base(shortDescr, id, longDescr,func)
        {
            NeedRegister = false;
            NeedMemory = true;
        }

        /// <summary>
        /// Executes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool Execute(ref object[] o)
        {
            if (o.Length > 0 && o[0] is Int64 akk && o[1] is Int64 mem)
            {
                o[1] = _func?.Invoke(akk,mem) ?? mem;
                return true;
            }
            return false;
        }
    }
}