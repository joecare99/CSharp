// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="FileUtils.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using System;
using System.IO;

namespace BaseLib.Helper;

/// <summary>A static class with useful file- &amp; path - routines</summary>
public static class FileUtils
{
    const string ExtSeparator = ".";
    private const string NewExt = ".~new";
    private const string BackupExt = ".bak";

    private static IFile xFile = new FileProxy();

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
        using (Stream fs = xFile.OpenWrite(sFilename))
        using (StreamWriter sw = new(fs))
        {
            sw.Write(sPayload);
        }
        return xFile.Exists(sFilename);
    }

    /// <summary>Read a file into a string.</summary>
    /// <param name="sFilename">The filename.</param>
    /// <returns>the data of the file</returns>
    public static string? ReadStringFromFile(string sFilename)
    {
        if (!xFile.Exists(sFilename)) return null;
        string sResult = "";
        using (Stream fs = xFile.OpenRead(sFilename))
        using (StreamReader sw = new(fs))
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
        // Deletes "NewFile" if exists
        if (xFile.Exists(sNewFilename))
        {
             xFile.Delete(sNewFilename);
        }
        // Save the File as "NewFile"
        try
        {
            actSaveFile(sNewFilename, oData);
        }
        catch 
        {
            if (xFile.Exists(sNewFilename))
                try
                {
                    xFile.Delete(sNewFilename);
                }
                catch { } // Hard to get here !!
            throw;
        }
        if (!xFile.Exists(sNewFilename))
            return false;
        else
        {
            // Success
            // Delete "BakFile" if exists
            if (xFile.Exists(sBakFilename))
            {
                xFile.Delete(sBakFilename);
            }
            // Do the Switch New -> File -> Bak
            if (sNewFilename == sFilename) { }
            else

                if (xFile.Exists(sFilename))
                File.Replace(sNewFilename, sFilename, sBakFilename);
            else
                xFile.Move(sNewFilename, sFilename);
                return true;
        }          
    }
}
