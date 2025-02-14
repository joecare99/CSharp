using System;

namespace TestConsole.View
{
    /// <summary>
    /// Struct ConsoleCharInfo
    /// </summary>
    public struct ConsoleCharInfo
    {
        /// <summary>
        /// The ch
        /// </summary>
        public char ch;
        /// <summary>
        /// The FGR
        /// </summary>
        public ConsoleColor fgr;
        /// <summary>
        /// The BGR
        /// </summary>
        public ConsoleColor bgr;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleCharInfo"/> struct.
        /// </summary>
        public ConsoleCharInfo(bool init=false){
            ch = '\x00';
            fgr = ConsoleColor.Gray;
            bgr = ConsoleColor.Black;
        }
    }
}
