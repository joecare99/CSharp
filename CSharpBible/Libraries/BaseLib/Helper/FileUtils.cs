using System;
using System.IO;

namespace BaseLib.Helper
{
    /// <summary>A static class with usefull file- &amp; path - routines</summary>
    public static class FileUtils
    {
        const string ExtSeparator = ".";
        private const string NewExt = ".~new";
        private const string BackupExt = ".bak";

        /// <summary>Changes the extension of a filename by a new extension.</summary>
        /// <param name="sFilename">The original filename.</param>
        /// <param name="sNewExt">The new extension.</param>
        /// <returns>The new filename</returns>
        /// <example>string sNewFile = ChangeFileExt("Hello.txt",".new");// gives "Hello.new"
        /// <code></code></example>
        public static string ChangeFileExt(this string sFilename, string sNewExt)
        {
            if (!sNewExt.Contains(ExtSeparator))
                sNewExt = ExtSeparator + sNewExt;
            string sFilepath = sFilename.Substring(0,sFilename.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, ':' })+1);
            return sFilepath + Path.GetFileNameWithoutExtension(sFilename) + sNewExt;
        }

        /// <summary>Writes the string to a file.</summary>
        /// <param name="sFilename">The filename.</param>
        /// <param name="sPayload">
        ///   <para>
        /// The data-payload.
        /// </para>
        /// </param>
        /// <returns>
        ///   <strong>true</strong> if fil was created.</returns>
        public static bool WriteStringToFile(string sFilename,string sPayload)
        {
            using (FileStream fs = File.OpenWrite(sFilename))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(sPayload);
            }
            return File.Exists(sFilename);
        }

        /// <summary>Read a file into a string.</summary>
        /// <param name="sFilename">The filename.</param>
        /// <returns>the data of the file</returns>
        public static string? ReadStringFromFile(string sFilename)
        {
            if (!File.Exists(sFilename)) return null;
            string sResult = "";
            using (FileStream fs = File.OpenRead(sFilename))
            using (StreamReader sw = new StreamReader(fs))
            {
                sResult = sw.ReadToEnd();
            }
            return sResult;
        }

        /// <summary>Saves a file
        /// in a safe way incl. backup</summary>
        /// <param name="actSaveFile">the original SaveFile-Action</param>
        /// <param name="sFilename">The filename.</param>
        /// <param name="oData">Some object-data.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static Boolean SaveFile(this Action<string, object?> actSaveFile, string sFilename, object? oData)
        {
            string sNewFilename = sFilename.ChangeFileExt(NewExt);
            string sBakFilename = sFilename.ChangeFileExt(BackupExt);
            // Delets "NewFile" if exists
            if (File.Exists(sNewFilename))
            {
                File.Delete(sNewFilename);
            }
            // Save the File as "NewFile"
            try
            {
                actSaveFile(sNewFilename, oData);
            }
            catch 
            {
                if (File.Exists(sNewFilename))
                    try
                    {
                        File.Delete(sNewFilename);
                    }
                    catch { } 
                throw;
            }
            if (!File.Exists(sNewFilename))
                return false;
            else
            {
                // Success
                // Delete "BakFile" if exists
                if (File.Exists(sBakFilename))
                {
                    File.Delete(sBakFilename);
                }
                // Do the Switch New -> File -> Bak
                if (sNewFilename != sFilename)
                    if (File.Exists(sFilename))
                        File.Replace(sNewFilename, sFilename, sBakFilename);
                    else
                        File.Move(sNewFilename, sFilename);
                return true;
            }          
        }
    }
}
