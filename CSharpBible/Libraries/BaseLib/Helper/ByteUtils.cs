using System;

namespace BaseLib.Helper
{
    public static class ByteUtils
    {

         ///<summary>
        /// return bit array with toggled SingleBit at Index
        /// </summary>
        public static int SwitchBit(this int bitarray, int index) 
            => bitarray ^ index.BitMask32();

         ///<summary>
        /// return Bit array with toggled SingleBit at Index
        /// </summary>
        public static long SwitchBit(this long bitarray, int index)
            => bitarray ^ index.BitMask64();

         ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static int SetBit(this int bitarray, int index)
            => bitarray | index.BitMask32();

         ///<summary>
        /// return bit array with set SingleBit at Index
        /// </summary>
        public static long SetBit(this long bitarray, int index)
            => bitarray | index.BitMask64();

         ///<summary>
        /// return bit array with cleared SingleBit at Index
        /// </summary>
        public static int ClearBit(this int bitarray, int index)
            => bitarray & ~index.BitMask32();

         ///<summary>
        /// return bit array with cleared SingleBit at Index
        /// </summary>
        public static long ClearBit(this long bitarray, int index)
            => bitarray & ~index.BitMask64();

        /// <summary>
        ///  returns int Bitmask with 32Bits at bit [Bitnumber] is 1 rest 0 
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static int BitMask32(this int bit) 
            => bit < 32 && bit >= 0 ? 0x01 << bit : throw new ArgumentException("out of range");

        /// <summary>
        /// returns unsinged Bitmask with 32Bits at BitNumber Bitsmaks is 1 rest 0
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static uint BitMask32(this uint bit) 
            => bit < 32 ? (uint)(0x01 << (int)bit) : throw new ArgumentException("out of range");

        /// <summary>
        /// returns long Bitmask with 64Bits at BitNumber Bitsmaks is 1 rest 0
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static long BitMask64(this int bit) 
            => bit < 64 && bit >= 0 ? 0x01L << bit : throw new ArgumentException("out of range");
    }
}
