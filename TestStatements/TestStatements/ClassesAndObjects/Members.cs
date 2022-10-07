// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-13-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Members.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.ClassesAndObjects
{
    /// <summary>
    /// Class Members.
    /// </summary>
    public class Members
    {
        // Konstanten
        /// <summary>
        /// The constant string
        /// </summary>
        public static readonly String ConstString = "This is a constant string!";

        // Felder
        /// <summary>
        /// The field count
        /// </summary>
        public static int FieldCount;

        // Delegate
#if NET5_0_OR_GREATER
		public static EventHandler? OnChange { get; set; }
#else
        /// <summary>
        /// Gets or sets the on change.
        /// </summary>
        /// <value>The on change.</value>
        public static EventHandler OnChange { get; set; }
#endif
        // Methoden
        /// <summary>
        /// as the method.
        /// </summary>
        public static void aMethod()
        {
            FieldCount = 3;
        }

        // Eigenschaften
        /// <summary>
        /// Gets or sets a property.
        /// </summary>
        /// <value>a property.</value>
        public static int aProperty
        {
            get { return FieldCount; }
            set
            {
                if (FieldCount != value)
                {
#if NET5_0_OR_GREATER
					OnChange?.Invoke(null, new EventArgs());
#else
					OnChange?.Invoke(null, null);					
#endif
					FieldCount = value;
                }
            }
        }

        // Indexer
        /// <summary>
        /// Gets or sets the <see cref="System.Int32" /> with the specified i.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns>System.Int32.</returns>
        public int this[int i]
        {
            get => i + FieldCount; set => FieldCount = value - i;
        }

    }
}
