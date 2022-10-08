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
    /// Class TDataHelpers.
    /// </summary>
    public static class TDataHelpers
    {
        /// <summary>
        /// extracts th high-byte of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.Byte.</returns>
        public static byte HiByte(this short Number) => (byte)(Number >> 8);

        /// <summary>
        /// extracts th high-byte of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.Byte.</returns>
        public static byte HiByte(this ushort Number) => (byte)(Number >> 8);

        /// <summary>
        /// extracts th low-byte of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.Byte.</returns>
        public static byte LoByte(this ushort Number) => (byte)(Number & byte.MaxValue);

        /// <summary>
        /// extracts th low-byte of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.Byte.</returns>
        public static byte LoByte(this short Number) => (byte)(Number & byte.MaxValue);

        /// <summary>
        /// extracts th high-word of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt16.</returns>
        public static ushort HiWord(this int Number) => (ushort)(Number >> 16);

        /// <summary>
        /// extracts th high-word of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt16.</returns>
        public static ushort HiWord(this uint Number) => (ushort)(Number >> 16);

        /// <summary>
        /// Loes the word.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt16.</returns>
        public static ushort LoWord(this int Number) => (ushort)(Number & ushort.MaxValue);

        /// <summary>
        /// Loes the word.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt16.</returns>
        public static ushort LoWord(this uint Number) => (ushort)(Number & ushort.MaxValue);

        /// <summary>
        /// extracts th high-uDWord of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt32.</returns>
        public static uint HiDWord(this long Number) => (uint)(Number >> 32);

        /// <summary>
        /// extracts th high-uDWord of the value
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt32.</returns>
        public static uint HiDWord(this ulong Number) => (uint)(Number >> 32);

        /// <summary>
        /// extracts th low-byte of the value.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt32.</returns>
        public static uint LoDWord(this long Number) => (uint)(Number & uint.MaxValue);

        /// <summary>
        /// Loes the d word.
        /// </summary>
        /// <param name="Number">The number.</param>
        /// <returns>System.UInt32.</returns>
        public static uint LoDWord(this ulong Number) => (uint)(Number & uint.MaxValue);
    }
}