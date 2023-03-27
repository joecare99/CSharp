namespace BaseLib.Helper
{
    /// <summary>A static class with usefull string-routines.</summary>
    public static class StringUtils
    {
        /// <summary>
        ///   <para>
        /// Makes the specified string quotable.<br />-&gt; by escaping special Characters (Linefeed, NewLine, Tab ...)</para>
        ///   <para>
        ///     <br />
        ///   </para>
        /// </summary>
        /// <param name="aStr">the string .</param>
        /// <returns>Quoted/escaped string</returns>
        /// <remarks>Does the opposite of <see cref="StringUtils.UnQuote()" /><br />In other words: Puts a given text into one Textline.</remarks>
        public static string Quote(this string aStr)
        {
            try
            {
                return aStr
                    .Replace("\\", "\\\\")
                    .Replace("\t", "\\t")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n");
            }
            catch { return ""; }
        }
        /// <summary>Unquotes the given string.<br />by un-escaping special characters (Linefeed, Newline, Tab ...)</summary>
        /// <param name="aStr">a string.</param>
        /// <returns>the unquoted/unescaped string</returns>
        /// <remarks>Does the opposite of <see cref="StringUtils.Quote()" /><br />In other words: Takes a given textline and extracts the (original) text.</remarks>
        public static string UnQuote(this string aStr) 
            => (aStr ?? "")
                  .Replace("\\\\", "\\\u0001")
                  .Replace("\\t", "\t")
                  .Replace("\\r", "\r")
                  .Replace("\\n", "\n")
                 .Replace("\\\u0001", "\\");

        public static string Format(this string aStr, params object[] par)
            => string.Format(aStr, par); 
    }
}
