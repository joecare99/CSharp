// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="TWinAPI.cs" company="JC-Soft">
//     Copyright © JC-Soft GmbH 2019-2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Runtime.InteropServices;
using System.Text;
namespace JCAMS.Core
{
    /// <summary>
    /// Class TWinAPI.
    /// </summary>
    public static class TWinAPI
    {

        /// <summary>
        /// Gets the private profile string.
        /// </summary>
        /// <param name="lpAppName">Name of the lp application.</param>
        /// <param name="lpKeyName">Name of the lp key.</param>
        /// <param name="lpDefault">The lp default.</param>
        /// <param name="lpreturnedString">The lpreturned string.</param>
        /// <param name="nSize">Size of the n.</param>
        /// <param name="lpFileName">Name of the lp file.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpreturnedString, int nSize, string lpFileName);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="wMsg">The w MSG.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int wMsg, int wParam, ref int lParam);

        /// <summary>
        /// Writes the private profile string.
        /// </summary>
        /// <param name="lpAppName">Name of the lp application.</param>
        /// <param name="lpKeyName">Name of the lp key.</param>
        /// <param name="lpString">The lp string.</param>
        /// <param name="lpFileName">Name of the lp file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
    }
}