﻿
// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 07-11-2020
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;

namespace JCAMS.Core
{
    /// <summary>
    /// Class TFileHelpers.
    /// </summary>
    public static class SFileHelpers
    {
        #region Properties
        private const string cPathPattern = "^(([a-zA-Z]\\:)|(\\\\))(\\\\{1}|((\\\\{1})[^\\\\]([^/:*?<>\"|]*))+)$";

        /// <summary>
        /// The m last error
        /// </summary>
        public static string m_LastError;

        /// <summary>
        /// The m last error
        /// </summary>
        private static object _SyncObject = new object();

        /// <summary>
        /// Gets or sets a value indicating whether [x start up mode].
        /// </summary>
        /// <value><c>true</c> if [x start up mode]; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public static bool xStartUpMode { get; set; } = true;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the file security.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="account">The account.</param>
        /// <param name="rights">The rights.</param>
        /// <param name="controlType">Type of the control.</param>
        public static void AddFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            try
            {
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
            }
        }

        /// <summary>
        /// Adds the file security everyone.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="rights">The rights.</param>
        /// <param name="controlType">Type of the control.</param>
        public static void AddFileSecurityEveryone(string fileName, FileSystemRights rights, AccessControlType controlType)
        {
            try
            {
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
            }
        }

        /// <summary>
        /// Checks the and create directory.
        /// </summary>
        /// <param name="sPath">The path.</param>
        /// <returns>System.String.</returns>
        public static string CheckAndCreateDirectory(string sPath)
        {
            switch (sPath)
            {
                case string s when string.IsNullOrEmpty(s): return "";
                case string s when s.ToUpper().Contains("<DATABASE>"): return "<DATABASE>";
                case string s when s.ToUpper().Contains("<EVENTLOG>"): return "<EVENTLOG>";
                case string s when !s.EndsWith("\\"): sPath += "\\";
                    break;
                default:
                    break;
            }
            try
            {
                if (xStartUpMode && !Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
                return sPath;
            }
            catch (DirectoryNotFoundException)
            {
                SLogging.Log("Path '{0}' not found", sPath);
            }
            catch (Exception Ex)
            {
                SLogging.Log("Path '{0}'", sPath);
                SLogging.Log(Ex);
            }
            return "";
        }

        /// <summary>
        /// Checks the file exists.
        /// </summary>
        /// <param name="FileWithPath">The file with path.</param>
        /// <param name="RaiseError">if set to <c>true</c> [raise error].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckFileExists(string FileWithPath, bool _) 
            => File.Exists(FileWithPath);

        /// <summary>
        /// Checks the path accessibility.
        /// </summary>
        /// <param name="sPath">The s path.</param>
        /// <param name="RaiseError">if set to <c>true</c> [raise error].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckPathAccessibility(string sPath, bool RaiseError) => sPath switch
        {
            string s when s.Substring(1, 2) == ":\\" && (!IsNetworkDrive(sPath.Substring(0, 1), RaiseError)) => false,
            string s when !(new Regex(cPathPattern).IsMatch(s)) => false,
            _ => true,
        };

        /// <summary>
        /// Deletes all subfolders.
        /// </summary>
        /// <param name="DirectoryPath">The directory path.</param>
        public static void DeleteAllSubfolders(string DirectoryPath)
        {
            string[] files = Directory.GetFiles(DirectoryPath);
            foreach (string fileName in files)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                }
            }
            files = Directory.GetDirectories(DirectoryPath);
            foreach (string directoryName in files)
            {
                DeleteAllSubfolders(directoryName);
                try
                {
                    Directory.Delete(directoryName, recursive: true);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="DirectoryPath">The directory path.</param>
        /// <param name="Recursiv">if set to <c>true</c> [recursiv].</param>
        /// <param name="DeleteFiles">if set to <c>true</c> [delete files].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DeleteDirectory(string DirectoryPath, bool Recursiv, bool DeleteFiles)
        {
            if (!Directory.Exists(DirectoryPath))
            {
                return false;
            }
            if (!Recursiv && !DeleteFiles)
            {
                Directory.Delete(DirectoryPath, recursive: false);
                if (Directory.Exists(DirectoryPath))
                {
                    return false;
                }
                return false;
            }
            if (Recursiv && !DeleteFiles)
            {
                Directory.Delete(DirectoryPath, recursive: true);
                if (Directory.Exists(DirectoryPath))
                {
                    return false;
                }
                return false;
            }
            if (Recursiv && DeleteFiles)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(DirectoryPath);
                DeleteAllSubfolders(DirectoryPath);
                Directory.Delete(DirectoryPath, recursive: true);
                if (Directory.Exists(DirectoryPath))
                {
                    return false;
                }
                return false;
            }
            if (!Recursiv && DeleteFiles)
            {
                string[] files = Directory.GetFiles(DirectoryPath);
                foreach (string fileName in files)
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                    }
                }
                Directory.Delete(DirectoryPath, recursive: false);
                if (Directory.Exists(DirectoryPath))
                {
                    return false;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DeleteFile(string FileName)
        {
            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                return !File.Exists(FileName);
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                SLogging.Log("FileName:" + FileName);
                return false;
            }
        }

        /// <summary>
        /// Deletes the files older than seconds.
        /// </summary>
        /// <param name="SourcePath">The source path.</param>
        /// <param name="SearchPattern">The search pattern.</param>
        /// <param name="KeepSeconds">The keep seconds.</param>
        /// <param name="UsernameSrc">The username source.</param>
        /// <param name="PasswordSrc">The password source.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DeleteFilesOlderThanSeconds(string SourcePath, string SearchPattern, int KeepSeconds, string UsernameSrc, string PasswordSrc)
        {
            return true;
        }

        /// <summary>
        /// Deletes the files older than x days.
        /// </summary>
        /// <param name="Path">The path.</param>
        /// <param name="Extension">The extension.</param>
        /// <param name="KeepLastDays">The keep last days.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DeleteFilesOlderThanXDays(string Path, string Extension, int KeepLastDays) 
            => KeepLastDays >= 0 && DeleteFilesOlderThanXSeconds(Path, Extension, KeepLastDays * 60 * 60 * 24);

        /// <summary>
        /// Deletes the files older than x seconds.
        /// </summary>
        /// <param name="Path">The path.</param>
        /// <param name="Extension">The extension.</param>
        /// <param name="KeepLastSeconds">The keep last seconds.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DeleteFilesOlderThanXSeconds(string Path, string Extension, long KeepLastSeconds)
        {
            int Cnt = 0;
            try
            {
                if (KeepLastSeconds < 0)
                {
                    return false;
                }
                DirectoryInfo DI = new DirectoryInfo(Path);
                FileInfo[] aFiles = (Extension.ToLower().Contains("*") || Extension.ToLower().Contains(".")) ? DI.GetFiles(Extension, SearchOption.AllDirectories) : DI.GetFiles("*." + Extension);
                FileInfo[] array = aFiles;
                foreach (FileInfo FI in array)
                {
                    if ((DateTime.Now - FI.LastWriteTime).TotalSeconds > (double)KeepLastSeconds)
                    {
                        SLogging.Log("Delete File {0}\\{1}", Path, FI.Name);
                        FI.Delete();
                        Cnt++;
                    }
                }
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
            }
            return Cnt > 0;
        }

        /// <summary>
        /// Determines whether [is cad filename] [the specified file name].
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns><c>true</c> if [is cad filename] [the specified file name]; otherwise, <c>false</c>.</returns>
        public static bool IsCADFilename(string FileName)
        {
            FileName = FileName.ToLower();
            return FileName.EndsWith(".dxf") || FileName.EndsWith(".dwg");
        }

        /// <summary>
        /// Determines whether [is image filename] [the specified file name].
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns><c>true</c> if [is image filename] [the specified file name]; otherwise, <c>false</c>.</returns>
        public static bool IsImageFilename(string FileName)
        {
            FileName = FileName.ToLower();
            return FileName.EndsWith(".bmp") || FileName.EndsWith(".jpg") || FileName.EndsWith(".jpeg") || FileName.EndsWith(".png") || FileName.EndsWith(".gif");
        }

        /// <summary>
        /// Determines whether [is nc filename] [the specified file name].
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns><c>true</c> if [is nc filename] [the specified file name]; otherwise, <c>false</c>.</returns>
        public static bool IsNCFilename(string FileName)
        {
            FileName = FileName.ToLower();
            return FileName.Contains("punch");
        }

        /// <summary>
        /// Determines whether [is network drive] [the specified drive letter].
        /// </summary>
        /// <param name="DriveLetter">The drive letter.</param>
        /// <param name="RaiseError">if set to <c>true</c> [raise error].</param>
        /// <returns><c>true</c> if [is network drive] [the specified drive letter]; otherwise, <c>false</c>.</returns>
        public static bool IsNetworkDrive(string DriveLetter, bool RaiseError)
        {
            DriveInfo DI = new DriveInfo(DriveLetter);
            if (DI.IsReady)
            {
                return true;
            }
            if (RaiseError)
            {
                m_LastError = "DriveLetter";
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is text filename] [the specified file name].
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns><c>true</c> if [is text filename] [the specified file name]; otherwise, <c>false</c>.</returns>
        public static bool IsTextFilename(string FileName)
        {
            FileName = FileName.ToLower();
            return FileName.EndsWith(".txt") || FileName.EndsWith(".dat");
        }

        /// <summary>
        /// Determines whether [is valid path] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if [is valid path] [the specified path]; otherwise, <c>false</c>.</returns>
        public static bool IsValidPath(string path) 
            => new Regex(cPathPattern).IsMatch(path);

        /// <summary>
        /// Moves the file.
        /// </summary>
        /// <param name="SourceFilename">The source filename.</param>
        /// <param name="TargetFilename">The target filename.</param>
        /// <param name="State">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MoveFile(string SourceFilename, string TargetFilename, out int State)
        {
            try
            {
                if (!File.Exists(SourceFilename))
                {
                    State = -1;
                    return false;
                }
                File.Copy(SourceFilename, TargetFilename, overwrite: true);
                if (!File.Exists(TargetFilename))
                {
                    State = -2;
                    return false;
                }
                File.Delete(SourceFilename);
                if (File.Exists(SourceFilename))
                {
                    State = -3;
                    return false;
                }
                State = 1;
                return true;
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                SLogging.Log("SourceFilename: {0}, TargetFilename: {1}:", SourceFilename, TargetFilename);
                State = -4;
                return false;
            }
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="Content">The content.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ReadFile(string FileName, out List<string> Content)
        {
            StreamReader TXTDoc = null;
            Content = new List<string>();
            for (int I = 0; I < 3; I++)
            {
                try
                {
                    TXTDoc = new StreamReader(FileName, Encoding.Default);
                    while (!TXTDoc.EndOfStream)
                    {
                        Content.Add(TXTDoc.ReadLine());
                    }
                    TXTDoc.Close();
                    return true;
                }
                catch (Exception Ex)
                {
                    SLogging.Log(Ex);
                }
            }

            SLogging.Log("ReadFile {0}: failed", FileName);
            TXTDoc?.Close();
            return false;
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns>System.String[].</returns>
        public static string[] ReadFile(string FileName)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    return new string[0];
                }
                return File.ReadAllLines(FileName);
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                return new string[0];
            }
        }

        /// <summary>
        /// Removes the file security.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="account">The account.</param>
        /// <param name="rights">The rights.</param>
        /// <param name="controlType">Type of the control.</param>
        public static void RemoveFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            try
            {
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
            }
        }

        /// <summary>
        /// Replaces the in file.
        /// </summary>
        /// <param name="OldFile">The old file.</param>
        /// <param name="OldString">The old string.</param>
        /// <param name="NewString">Creates new string.</param>
        /// <param name="NewFile">Creates new file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ReplaceInFile(string OldFile, string OldString, string NewString, string NewFile = null)
        {
            if (NewFile != null && string.Empty.Equals(NewFile))
            {
                return false;
            }
            if (!ReadFile(OldFile, out List<string> aLine))
            {
                return false;
            }
            bool fReplaced = false;
            try
            {
                for (int I = 0; I < aLine.Count; I++)
                {
                    if (aLine[I].Contains(OldString))
                    {
                        fReplaced = true;
                        aLine[I] = aLine[I].Replace(OldString, NewString);
                    }
                }
                if (!fReplaced)
                {
                    return false;
                }
                if (NewFile == null)
                {
                    DeleteFile(OldFile);
                }
                else
                {
                    DeleteFile(NewFile);
                }
                for (int I = 0; I < aLine.Count; I++)
                {
                    if (NewFile == null)
                    {
                        WriteToFile(OldFile, aLine[I] + "\n", Append: true);
                    }
                    else
                    {
                        WriteToFile(NewFile, aLine[I] + "\n", Append: true);
                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                return false;
            }
        }

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="OutBuffer">The out buffer.</param>
        /// <param name="Arguments">The arguments.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool WriteToFile(string FileName, string OutBuffer, params object[] Arguments) 
            => WriteToFile(FileName, string.Format(OutBuffer, Arguments), Append: true, Encoding.Unicode);

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="OutBuffer">The out buffer.</param>
        /// <param name="Append">if set to <c>true</c> [append].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool WriteToFile(string FileName, string OutBuffer, bool Append) 
            => WriteToFile(FileName, OutBuffer, Append, Encoding.Unicode);

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="OutBuffer">The out buffer.</param>
        /// <param name="Append">if set to <c>true</c> [append].</param>
        /// <param name="Encoding">The encoding.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool WriteToFile(string FileName, string OutBuffer, bool Append, Encoding Encoding)
        {
            lock (_SyncObject)
            {
                try
                {
                    StreamWriter SW = new StreamWriter(FileName, Append, Encoding, 1024);
                    SW.AutoFlush = true;
                    SW.Write(OutBuffer);
                    SW.Close();
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public static bool WriteToStream(Stream fs, string s, object[] p)
        {
            using (StreamWriter SW = new StreamWriter(fs, Encoding.Default, 1024, true))
            {
                SW.AutoFlush = true;
                SW.Write(s,p);
                return true;
            };
        }
        #endregion
    }
}