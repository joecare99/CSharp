// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace JCAMS.Core.DataOperations
{
    /// <summary>
    /// Class TBitOperations.
    /// </summary>
    public static class SBitOperations
    {
        #region Methods
        /// <summary>
        /// Determines whether [is bit set] [the specified test].
        /// </summary>
        /// <param name="iVal">The test.</param>
        /// <param name="iBit">The zero based bit number.</param>
        /// <returns><c>true</c> if [is bit set] [the specified test]; otherwise, <c>false</c>.</returns>
        public static bool IsBitSet(this int iVal, int iBit) 
            => iBit > 32 || iBit < 0 ? false : (iVal & 1 << iBit) != 0;

        /// <summary>
        /// Determines whether [is bit set] [the specified test].
        /// </summary>
        /// <param name="lVal">The test.</param>
        /// <param name="iBit">The zero based bit number.</param>
        /// <returns><c>true</c> if [is bit set] [the specified test]; otherwise, <c>false</c>.</returns>
        public static bool IsBitSet(this long lVal, int iBit) 
            => iBit > 64 || iBit < 0 ? false : (lVal & 1L << iBit) != 0;

        /// <summary>
        /// Determines whether [is bit set] [the specified test].
        /// </summary>
        /// <param name="uVal">The test.</param>
        /// <param name="iBit">The zero based bit number.</param>
        /// <returns><c>true</c> if [is bit set] [the specified test]; otherwise, <c>false</c>.</returns>
        public static bool IsBitSet(this ulong uVal, int iBit) 
            => iBit > 64 || iBit < 0 ? false : ((long)uVal & 1L << iBit) != 0;

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="iVal">The org.</param>
        /// <param name="iBit">The zero based bit number.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns>System.Int32.</returns>
        public static int SetBit(this int iVal, int iBit, bool Value) 
            => Value ? iVal | (1 << iBit) : iVal & ~(1 << iBit);

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="lVal">The org.</param>
        /// <param name="iBit">The zero based bit number.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns>System.Int64.</returns>
        public static long SetBit(this long lVal, int iBit, bool Value)
            => Value ? lVal | (1L << iBit) : lVal & ~(1L << iBit);

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="uVal">The org.</param>
        /// <param name="iBit">The zero based bit number.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns>System.UInt64.</returns>
        public static ulong SetBit(this ulong uVal, int iBit, bool Value) 
            => Value ? (ulong)((long)uVal | (1L << iBit)) : (ulong)((long)uVal & ~(1L << iBit));

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="Org">The org.</param>
        /// <param name="ZeroBasedBitNumber">The zero based bit number.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        public static void SetBit(ref int Org, int ZeroBasedBitNumber, bool Value)
            => Org = SetBit(Org, ZeroBasedBitNumber, Value);
        

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="Org">The org.</param>
        /// <param name="ZeroBasedBitNumber">The zero based bit number.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        public static void SetBit(ref long Org, int ZeroBasedBitNumber, bool Value)
            =>  Org = SetBit(Org, ZeroBasedBitNumber, Value);
        #endregion
    }
}