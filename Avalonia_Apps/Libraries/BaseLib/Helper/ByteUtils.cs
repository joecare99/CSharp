﻿// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="ByteUtils.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace BaseLib.Helper
{
    /// <summary>
    /// Class ByteUtils.
    /// </summary>
    /// <autogeneratedoc />
    public static class ByteUtils
    {

         ///<summary>
        /// return bit array with toggled SingleBit at Index
        /// </summary>
        public static int SwitchBit(this int bitArray, int index) 
            => bitArray ^ index.BitMask32();

        ///<summary>
        /// return bit array with toggled SingleBit at Index
        /// </summary>
        public static uint SwitchBit(this uint bitArray, int index)
            => bitArray ^ ((uint)index).BitMask32();

        ///<summary>
        /// return Bit array with toggled SingleBit at Index
        /// </summary>
        public static long SwitchBit(this long bitArray, int index)
            => bitArray ^ index.BitMask64();

         ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static int SetBit(this int bitArray, int index)
            => bitArray | index.BitMask32();

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static uint SetBit(this uint bitArray, int index)
            => bitArray | ((uint)index).BitMask32();

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static long SetBit(this long bitArray, int index)
            => bitArray | index.BitMask64();

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static int SetBit(this int bitArray, int index, bool xVal)
            => xVal ? bitArray.SetBit(index) : bitArray.ClearBit(index);

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static uint SetBit(this uint bitArray, int index, bool xVal)
            => xVal ? bitArray.SetBit(index) : bitArray.ClearBit(index);

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static long SetBit(this long bitArray, int index, bool xVal)
            => xVal ? bitArray.SetBit(index) : bitArray.ClearBit(index);

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static bool GetBit(this int bitArray, int index)
            => (bitArray & index.BitMask32())!=0;

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static bool GetBit(this uint bitArray, int index)
            => (bitArray & ((uint)index).BitMask32())!=0;

        ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static bool GetBit(this long bitArray, int index)
            => (bitArray & index.BitMask64()) != 0;

        ///<summary>
        /// return bit array with cleared SingleBit at Index
        /// </summary>
        public static int ClearBit(this int bitArray, int index)
            => bitArray & ~index.BitMask32();

        ///<summary>
        /// return bit array with cleared SingleBit at Index
        /// </summary>
        public static uint ClearBit(this uint bitArray, int index)
            => bitArray & ~((uint)index).BitMask32();

        ///<summary>
        /// return bit array with cleared SingleBit at Index
        /// </summary>
        public static long ClearBit(this long bitArray, int index)
            => bitArray & ~index.BitMask64();

        /// <summary>
        ///  returns int Bitmask with 32Bits at bit [Bit] is 1 rest 0 
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static int BitMask32(this int bit) 
            => bit < 32 && bit >= 0 ? 0x01 << bit : throw new ArgumentException("out of range");

        /// <summary>
        /// returns unsinged Bitmask with 32Bits at BitNumber Bit is 1 rest 0
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static uint BitMask32(this uint bit) 
            => bit < 32 ? (uint)(0x01 << (int)bit) : throw new ArgumentException("out of range");

        /// <summary>
        /// returns long Bitmask with 64Bits at BitNumber Bit is 1 rest 0
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static long BitMask64(this int bit) 
            => bit < 64 && bit >= 0 ? 0x01L << bit : throw new ArgumentException("out of range");
    }
}
