// ***********************************************************************
// Assembly         : Permutation
// Author           : Mir
// Created          : 07-30-2022
//
// Last Modified By : Mir
// Last Modified On : 07-31-2022
// ***********************************************************************
// <copyright file="PermutatedRange.cs" company="Permutation">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutation.Model
{
    /// <summary>
    /// Class PermutatedRange.
    /// </summary>
    public class PermutatedRange
    {
        /// <summary>
        /// The prange
        /// </summary>
        int[] _prange;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermutatedRange"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="pp">The pp.</param>
        public PermutatedRange(int size, int pp)
        {
           _prange = new int[size];
            for (int i = 0; i < _prange.Length; i++)
                _prange[i] = i;
            int next2Power = (int)Math.Pow(2,Math.Ceiling(Math.Log(Math.Sqrt(size), 2)));
            Int64 ringsize = (next2Power) * (next2Power);
            for (Int64 i = 0; i < pp; i++)
                if ((i + 2) * (next2Power-1) % ringsize <size && (i + 1) * (next2Power + 1) % ringsize < size)
                switchval(ref _prange[(i+2) * (next2Power - 1) % ringsize], ref _prange[(i+1) * (next2Power + 1) % ringsize]);            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermutatedRange"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public PermutatedRange(int size) : this(size, (int)Math.Pow(Math.Pow(2, Math.Ceiling(Math.Log(Math.Sqrt(size), 2))),2) / 2)
        {
        }

        /// <summary>
        /// Switchvals the specified v1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        private void switchval<T>(ref T v1, ref T v2)
        {
            (v1,v2)=(v2,v1);
        }

        /// <summary>
        /// Gets the <see cref="System.Int32"/> with the specified ix.
        /// </summary>
        /// <param name="ix">The ix.</param>
        /// <returns>System.Int32.</returns>
        public int this[int ix] { get=> ix<_prange.Length? _prange[ix]:-1; } 
    }
}
