using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_Base.Model
{
    /// <summary>
    /// Struct BlockDef
    /// </summary>
    public struct BlockDef
    {
        /// <summary>
        /// The b type
        /// </summary>
        public BlockType bType;
        /// <summary>
        /// The angle
        /// </summary>
        public BlockAngle angle;
        /// <summary>
        /// The b color
        /// </summary>
        public ConsoleColor bColor;
        /// <summary>
        /// The b koor
        /// </summary>
        public byte[] bKoor;
    }
    /// <summary>
    /// Class Defines.
    /// </summary>
    public class Defines
    {
        /// <summary>
        /// The block defines
        /// </summary>
        public static BlockDef[] BlockDefines = new BlockDef[]
        {
            // I- Type
            new BlockDef(){ bType = BlockType.I, bColor = ConsoleColor.Blue, angle = BlockAngle.Degr0, bKoor = new byte[]{ 0x34 , 0x54, 0x64 } },
            new BlockDef(){ bType = BlockType.I, bColor = ConsoleColor.Blue, angle = BlockAngle.Degr90, bKoor = new byte[]{ 0x43 , 0x45,0x46 } },
            new BlockDef(){ bType = BlockType.I, bColor = ConsoleColor.Blue, angle = BlockAngle.Degr180, bKoor = new byte[]{ 0x34, 0x54, 0x64 } },
            new BlockDef(){ bType = BlockType.I, bColor = ConsoleColor.Blue, angle = BlockAngle.Degr270, bKoor = new byte[]{ 0x43, 0x45, 0x46 } },
            // J- Type
            new BlockDef(){ bType = BlockType.J, bColor = ConsoleColor.Magenta, angle = BlockAngle.Degr0, bKoor = new byte[]{ 0x34, 0x54, 0x53 } },
            new BlockDef(){ bType = BlockType.J, bColor = ConsoleColor.Magenta, angle = BlockAngle.Degr90, bKoor = new byte[]{ 0x43, 0x45, 0x55 } },
            new BlockDef(){ bType = BlockType.J, bColor = ConsoleColor.Magenta, angle = BlockAngle.Degr180, bKoor = new byte[]{ 0x34, 0x35, 0x54 } },
            new BlockDef(){ bType = BlockType.J, bColor = ConsoleColor.Magenta, angle = BlockAngle.Degr270, bKoor = new byte[]{ 0x33, 0x43, 0x45 } },
            // L- Type
            new BlockDef(){ bType = BlockType.L, bColor = ConsoleColor.Green, angle = BlockAngle.Degr0, bKoor = new byte[]{ 0x34, 0x54, 0x55 } },
            new BlockDef(){ bType = BlockType.L, bColor = ConsoleColor.Green, angle = BlockAngle.Degr90, bKoor = new byte[]{ 0x43, 0x45, 0x35 } },
            new BlockDef(){ bType = BlockType.L, bColor = ConsoleColor.Green, angle = BlockAngle.Degr180, bKoor = new byte[]{ 0x54, 0x34, 0x33 } },
            new BlockDef(){ bType = BlockType.L, bColor = ConsoleColor.Green, angle = BlockAngle.Degr270, bKoor = new byte[]{ 0x53, 0x43, 0x45 } },
            // x
            new BlockDef(){ bType = BlockType.x, bColor = ConsoleColor.Yellow, angle = BlockAngle.Degr0, bKoor = new byte[]{ 0x34, 0x43, 0x33 } },
            new BlockDef(){ bType = BlockType.x, bColor = ConsoleColor.Yellow, angle = BlockAngle.Degr90, bKoor = new byte[]{ 0x34, 0x43, 0x33 } },
            new BlockDef(){ bType = BlockType.x, bColor = ConsoleColor.Yellow, angle = BlockAngle.Degr180, bKoor = new byte[]{ 0x34, 0x43, 0x33 } },
            new BlockDef(){ bType = BlockType.x, bColor = ConsoleColor.Yellow, angle = BlockAngle.Degr270, bKoor = new byte[]{ 0x34, 0x43, 0x33 } },
            // s
            new BlockDef(){ bType = BlockType.S, bColor = ConsoleColor.Cyan, angle = BlockAngle.Degr0, bKoor = new byte[]{ 0x34, 0x43, 0x53 } },
            new BlockDef(){ bType = BlockType.S, bColor = ConsoleColor.Cyan, angle = BlockAngle.Degr90, bKoor = new byte[]{ 0x43, 0x54, 0x55 } },
            new BlockDef(){ bType = BlockType.S, bColor = ConsoleColor.Cyan, angle = BlockAngle.Degr180, bKoor = new byte[]{ 0x34, 0x43, 0x53 } },
            new BlockDef(){ bType = BlockType.S, bColor = ConsoleColor.Cyan, angle = BlockAngle.Degr270, bKoor = new byte[]{ 0x43, 0x54, 0x55 } },
            // z
            new BlockDef(){ bType = BlockType.Z, bColor = ConsoleColor.DarkRed, angle = BlockAngle.Degr0, bKoor = new byte[]{ 0x34, 0x45, 0x55 } },
            new BlockDef(){ bType = BlockType.Z, bColor = ConsoleColor.DarkRed, angle = BlockAngle.Degr90, bKoor = new byte[]{ 0x43, 0x34, 0x35 } },
            new BlockDef(){ bType = BlockType.Z, bColor = ConsoleColor.DarkRed, angle = BlockAngle.Degr180, bKoor = new byte[]{ 0x34, 0x45, 0x55 } },
            new BlockDef(){ bType = BlockType.Z, bColor = ConsoleColor.DarkRed, angle = BlockAngle.Degr270, bKoor = new byte[]{ 0x43, 0x34, 0x35 } },
            // t
            new BlockDef(){ bType = BlockType.T, bColor = ConsoleColor.Red, angle = BlockAngle.Degr0, bKoor = new byte[]{ 0x34, 0x45, 0x54 } },
            new BlockDef(){ bType = BlockType.T, bColor = ConsoleColor.Red, angle = BlockAngle.Degr90, bKoor = new byte[]{ 0x43, 0x45, 0x34 } },
            new BlockDef(){ bType = BlockType.T, bColor = ConsoleColor.Red, angle = BlockAngle.Degr180, bKoor = new byte[]{ 0x34, 0x43, 0x54 } },
            new BlockDef(){ bType = BlockType.T, bColor = ConsoleColor.Red, angle = BlockAngle.Degr270, bKoor = new byte[]{ 0x43, 0x54, 0x45 } },
        };
    }
}
