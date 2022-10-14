using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;

namespace JCAMS.Core.Extensions
{
    /// <summary>
    /// Class StringExtension.
    /// </summary>
    public static class SStringExtension
    {
        /// <summary>
        /// Gets the encapsulated.
        /// </summary>
        /// <param name="Text">The text.</param>
        /// <param name="StartTag">The start tag.</param>
        /// <param name="StopTag">The stop tag.</param>
        /// <returns>System.String.</returns>
        public static string GetEncapsulated(string Text, string StartTag, string StopTag)
        {
            int A = (StartTag != null) ? Text.IndexOf(StartTag) : 0;
            int B = (StopTag != null) ? Text.LastIndexOf(StopTag) : Text.Length;
            if (A == -1 && B == -1)
            {
                return "";
            }
            if (Math.Sign(A) != Math.Sign(B))
            {
                return "";
            }
            A += StartTag.Length;
            return Text.Substring(A, B - A);
        }

        private static readonly string[][] HTMLstrDef=new string[][] {
            new string[] {"&", "&amp;" },
            new string[] {"<", "&lt;" },
            new string[] {">", "&gt;" },
            new string[] {"ä", "&auml;" },
            new string[] {"Ä", "&Auml;" },
           new string[] {"ö", "&ouml;"},
           new string[] {"Ö", "&Ouml;"},
           new string[] {"ü", "&uuml;"},
           new string[] {"Ü", "&Uuml;"},
           new string[] { "ß", "&szlig;" }
        };

        /// <summary>
        /// Determines whether [is unc path] [the specified s path].
        /// </summary>
        /// <param name="sPath">The s path.</param>
        /// <returns><c>true</c> if [is unc path] [the specified s path]; otherwise, <c>false</c>.</returns>
        public static bool IsUNCPath(this string sPath)
        {
            return sPath.StartsWith("\\\\");
        }

        /// <summary>
        /// Determines whether [is FTP path] [the specified s path].
        /// </summary>
        /// <param name="sPath">The s path.</param>
        /// <returns><c>true</c> if [is FTP path] [the specified s path]; otherwise, <c>false</c>.</returns>
        public static bool IsFTPPath(this string sPath)
        {
            return sPath.StartsWith("ftp://");
        }

        /// <summary>
        /// Determines whether [is drive path] [the specified s path].
        /// </summary>
        /// <param name="sPath">The s path.</param>
        /// <returns><c>true</c> if [is drive path] [the specified s path]; otherwise, <c>false</c>.</returns>
        public static bool IsDrivePath(this string sPath)
        {
            return sPath.Substring(1, 2) == ":\\" && char.IsLetter(sPath[0]);
        }

        /// <summary>
		/// Determines whether [is date time] [the specified value].
		/// </summary>
		/// <param name="val">The value.</param>
		/// <returns><c>true</c> if [is date time] [the specified value]; otherwise, <c>false</c>.</returns>
		public static bool IsDateTime(this string val)
        {
            return DateTime.TryParse(val, out _);
        }

        /// <summary>
        /// Determines whether the specified value is double.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns><c>true</c> if the specified value is double; otherwise, <c>false</c>.</returns>
        public static bool IsDouble(this string val)
        {
            Regex r = new Regex("((\\b[0-9]+)?\\.)?\\b[0-9]+([eE][-+]?[0-9]+)?\\b");
            return r.IsMatch(val);
        }

        /// <summary>
        /// Ases the HTML.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.String.</returns>
        public static string AsHTML(this string s)
        {
            var result = s;
            for (var i = 0; i < HTMLstrDef.Length; i++)
                result = result.Replace(HTMLstrDef[i][0], HTMLstrDef[i][1]);
            return result;
        }

        /// <summary>
		/// Ases the HTML.
		/// </summary>
		/// <param name="C">The c.</param>
		/// <returns>System.String.</returns>
		public static string AsHTML(this Color C)
        {
            return $"#{C.R:X2}{C.G:X2}{C.B:X2}";
        }

        /// <summary>
		/// Gets the first int32.
		/// </summary>
		/// <param name="sText">The s text.</param>
		/// <returns>System.Int32.</returns>
		public static long GetFirstNumber(this string sText)
        {
            if (string.IsNullOrEmpty(sText)) return 0;
            int I = 0;
            while (I < sText.Length 
                && !char.IsNumber(sText[I]) 
                && (sText[I]!='-' 
                    || (I+1 < sText.Length  
                        && !char.IsNumber(sText[I+1])))) I++;
            var sSubText = sText.Substring(I);
            if (string.IsNullOrEmpty(sSubText)) return 0;
            I = 0;
            while (I < sSubText.Length && (char.IsNumber(sSubText[I]) || (I==0 && sSubText[I] == '-')) ) I++;
            return sSubText.Substring(0, I).AsInt64();
        }

        /// <summary>
        /// Count the Characters in sVal.
        /// </summary>
        /// <param name="sVal">The given text.</param>
        /// <param name="cChar">The character to count.</param>
        /// <returns>System.Int32.</returns>
        public static int CharCnt(this string sVal, char cChar)
        {
            if (string.IsNullOrEmpty(sVal)) return 0;
            int result = 0;
            for  (int I = 0; I < sVal.Length; I++)
              if (sVal[I] == cChar)  
                    result++;            
            return result;
        }

        public static Stream String2Stream(string sText,bool xBase64=false)
        {
            if (sText == null) return null;
            var result = new MemoryStream();
            byte[] b;
            if (xBase64)
                b = Convert.FromBase64String(sText);
            else
                b = new UTF8Encoding().GetBytes(sText);
            result.Write(b,0,b.Length);
            result.Position= 0L;
            return result;
        }

        public static string AsCompString(this Stream stream)
        {
            if (stream == null) return "";
            using (var nstream = new MemoryStream())
            using (var gz = new GZipStream(nstream, CompressionLevel.Optimal,true))
            {
                stream.CopyTo(gz);
                gz.Close();                
                nstream.Position=0L;
                var b = new byte[nstream.Length - nstream.Position];
                nstream.Read(b, 0, b.Length);
                return Convert.ToBase64String(b);
            }

        }


        public static string AsBase64String(this Stream stream)
        {
            if (stream == null) return "";
            return Convert.ToBase64String(stream.Stream2AByte());
        }

        private static byte[] Stream2AByte(this Stream stream)
        {
            if (stream == null) return null;
            var result = new byte[stream.Length - stream.Position];
            stream.Read(result, 0, result.Length);
            return result;
        }

        public static string Dump(this Stream stream)
        {
            if (stream == null) return "";
            var oP = stream.Position;
            var b = new byte[stream.Length-oP];
            stream.Read(b, 0, b.Length);
            stream.Position = oP;

            string l1 = "", l2 = "",result ="";
            string sFormat = "{0:X4}: {1} | {2}\r\n";
            if (b.Length >= 1 << 24)
                sFormat = "{0:X8}: {1} | {2}\r\n";
            else if (b.Length >= 1 << 16)
                sFormat = "{0:X6}: {1} | {2}\r\n";

            for (int i = 0; i < b.Length; i++)
            {
                l1 += $"{b[i]:X2} ";
                l2 += $"{(b[i] > 31 && b[i] < 128 ? (char)b[i] : '.')}";
                if (i % 16 == 7) l1 += ": ";
                if (i % 16 == 15)
                {
                    result += String.Format(sFormat, i - 15, l1, l2);
                    l1 = l2 = "";
                }
            }
            if (!string.IsNullOrEmpty(l1))
            {
                l1 = l1.PadRight(50);
                result += String.Format(sFormat, b.Length &~0xf, l1, l2);
            }
            return result;

        }
    }
}
