﻿// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="T.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using JCAMS.Core.Extensions;
using JCAMS.Core.Logging;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Text;
namespace JCAMS.Core
{
    public static class SVariableHandling
    {
        private const string cBaseKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\JC-Soft"; // Todo: allgemeiner machen
        private const string cSelectStFromConfig = "SELECT * FROM System_Config WHERE Enabled=1 AND AppName = '{0}' AND Secname = '{1}' AND KeyName = '{2}' AND {3}";
        private const string cUpdateConfigValue = "UPDATE System_Config SET Value = '{0}' WHERE Enabled=1 AND AppName = '{1}' AND SecName = '{2}' AND KeyName = '{3}' AND {4} ";
        private const string cInsertConfigValue = "INSERT INTO System_Config (AppName,SecName,KeyName,Instance,Value) VALUES ('{0}','{1}','{2}',{3},'{4}')";

        public static string sAppName { get; set; } = "JC-AMS";
        public static string sIniFile { get; set; } = "";

        public static Func<string, string, string, int, string> LoadInstVar { get; set; } = default;
        public static Func<string, string, string, int, string, bool> SaveInstVar { get; set; } = default;
        public static Func<int> GetInstance { get; set; } = ()=>0;

        /// <summary>
        /// The m load instanced variable counter
        /// </summary>
        private static int _LoadInstancedVarCounter=0;

        public static bool LoadInstancedVar(string AppName, string SectionName, string KeyName, int Instance, out string Value)
        {
            Value = LoadInstVar?.Invoke(AppName, SectionName, KeyName, Instance);
            return !string.IsNullOrEmpty(Value);
        }

        public static bool SaveInstancedVar(string AppName, string SectionName, string KeyName, int Instance, string Value)
        {
            return SaveInstVar?.Invoke(AppName, SectionName, KeyName, Instance, Value) ?? false;
        }

            /*
                    /// <summary>
                    /// Loads the instanced variable.
                    /// </summary>
                    /// <param name="AppName">Name of the application.</param>
                    /// <param name="SectionName">Name of the section.</param>
                    /// <param name="KeyName">Name of the key.</param>
                    /// <param name="Instance">The instance.</param>
                    /// <param name="Value">The value.</param>
                    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
                    public static bool LoadInstancedVar(string AppName, string SectionName, string KeyName, int Instance, out string Value)
                    {
                        int Cnt = 0;
                        Value = "";
                        switch (CJCAMS.ConfigSource)
                        {
                            case EConfigSource.IniFile:
                                {
                                    StringBuilder sData = new StringBuilder(" ", 1024);
                                    Cnt = TWinAPI.GetPrivateProfileString(SectionName, KeyName, "", sData, 1024, CJCAMS.ApplicationIniFile);
                                    if (Cnt > 0)
                                    {
                                        Value = sData.ToString().Substring(0, Cnt);
                                    }
                                    break;
                                }
                            case EConfigSource.Database:
                                {
                                    if (!CJCAMS.State.DatabaseAvailable)
                                    {
                                        return false;
                                    }
                                    _LoadInstancedVarCounter++;
                                    string sInstance = $"Instance = {Instance}";
                                    if (Instance < 1)
                                    {
                                        sInstance = $"({sInstance} OR Instance is null)";
                                    }
                                    string sAppName = (AppName != null && AppName.Length > 0) ? AppName : "";
                                    CSQLQuery Q = new CSQLQuery();
                                    Q.Execute(cSelectStFromConfig, AppName,SectionName,KeyName,sInstance);
                                    if (Q.Read())
                                    {
                                        Value = Q.String("Value");
                                    }
                                    else if (CConfiguration.General.Mode.StartUpMode)
                                    {
                                        if (!CJCAMS.State.AppStarting)
                                        {
                                            TLogging.Journalize("create missing System_Config ({0},{1},{2},{3})", AppName, SectionName, KeyName, Instance);
                                        }
                                        SaveInstancedVar(AppName, SectionName, KeyName, Instance, "");
                                    }
                                    else if (!CJCAMS.State.AppStarting)
                                    {
                                        TLogging.Journalize("Not allowed to create missing System_Config ({0},{1},{2},{3}) (not in startup mode!)", AppName, SectionName, KeyName, Instance);
                                    }
                                    Q.Delete();
                                    break;
                                }
                        }
                        return Value.Length > 0;
                    }

                    /// <summary>
                    /// Saves the instanced variable.
                    /// </summary>
                    /// <param name="AppName">Name of the application.</param>
                    /// <param name="SectionName">Name of the section.</param>
                    /// <param name="KeyName">Name of the key.</param>
                    /// <param name="Instance">The instance.</param>
                    /// <param name="Value">The value.</param>
                    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
                    public static bool SaveInstancedVar(string AppName, string SectionName, string KeyName, int Instance, string Value)
                    {
                        switch (CJCAMS.ConfigSource)
                        {
                            case EConfigSource.IniFile:
                                TWinAPI.WritePrivateProfileString(SectionName, KeyName, Value, CJCAMS.ApplicationIniFile);
                                break;
                            case EConfigSource.Database:
                                {
                                    if (!CJCAMS.State.DatabaseAvailable)
                                    {
                                        return false;
                                    }
                                    CSQLQuery Q = new CSQLQuery();
                                    string sInstance = $"Instance = {Instance}";
                                    if (Instance < 1)
                                    {
                                        sInstance = $"({sInstance} OR Instance is null)";
                                    }
                                    string sAppName = (AppName != null && AppName.Length > 0) ? AppName : "";
                                    if (!Q.Execute(cUpdateConfigValue, Value, AppName, SectionName, KeyName, sInstance))
                                    {
                                        sInstance = ((Instance < 1) ? "null" : Instance.ToString());
                                        Q.Execute(cInsertConfigValue, AppName, SectionName, KeyName, sInstance, Value);
                                    }
                                    Q.Delete();
                                    break;
                                }
                        }
                        return true;
                    }

            */

            /// <summary>
            /// Loads the variable.
            /// </summary>
            /// <param name="AppName">Name of the application.</param>
            /// <param name="SectionName">Name of the section.</param>
            /// <param name="KeyName">Name of the key.</param>
            /// <param name="Value">The value.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public static bool LoadVar(string AppName, string SectionName, string KeyName, out string Value)
        {
            Value = "";
            if (SectionName.ToLower().Contains("control") || SectionName.ToLower().Contains("s.m.a.r.t.") || SectionName.ToLower().Contains("functionkey"))
            {
                return LoadInstancedVar(AppName, SectionName, KeyName, GetInstance(), out Value);
            }
            return LoadInstancedVar(AppName, SectionName, KeyName, 0, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out string Value)
        {
            Value = "";
            return LoadVar(sAppName, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out int Value)
        {
            Value = 0;
            return LoadVar(sAppName, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out int Value)
        {
            string Text = "";
            Value = 0;
            LoadVar(AppName, SectionName, KeyName, out Text);
            return int.TryParse(Text, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out double Value)
        {
            Value = 0.0;
            return LoadVar(sAppName, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out double Value)
        {
            string Text = "";
            Value = 0.0;
            LoadVar(AppName, SectionName, KeyName, out Text);
            return double.TryParse(Text, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out decimal Value)
        {
            Value = 0m;
            return LoadVar(sAppName, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out decimal Value)
        {
            Value = 0m;
            if (!LoadVar(AppName, SectionName, KeyName, out double d))
            {
                return false;
            }
            Value = (decimal)d;
            return true;
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out bool Value)
        {
            Value = false;
            return LoadVar(sAppName, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out bool Value)
        {
            string Text = "";
            Value = false;
            LoadVar(AppName, SectionName, KeyName, out Text);
            int.TryParse(Text, out int I);
            Value = (I != 0);
            return true;
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out DateTime Value)
        {
            Value = DateTime.MinValue;
            return LoadVar(sAppName, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out DateTime Value)
        {
            string Text = "";
            Value = new DateTime(1970, 1, 1, 0, 0, 0);
            LoadVar(AppName, SectionName, KeyName, out Text);
            return DateTime.TryParse(Text, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out Point Value)
        {
            return LoadVar(null, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out Point Value)
        {
            string Text = "";
            LoadVar(AppName, SectionName, KeyName, out Text);
            string[] aText = Text.Split('/');
            if (aText.Length > 1)
            {
                Value = new Point(Convert.ToInt32(aText[0]), Convert.ToInt32(aText[1]));
                return true;
            }
            Value = new Point(0, 0);
            return false;
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string SectionName, string KeyName, out Size Value)
        {
            Value = new Size(0, 0);
            return LoadVar(sAppName, SectionName, KeyName, out Value);
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out Size Value)
        {
            Value = new Size(0, 0);
            if (!LoadVar(AppName, SectionName, KeyName, out Point P))
            {
                return false;
            }
            Value.Width = P.X;
            Value.Height = P.Y;
            return true;
        }

        /// <summary>
        /// Loads the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVar(string AppName, string SectionName, string KeyName, out Font Value)
        {
            string Text = "";
            LoadVar(AppName, SectionName, KeyName, out Text);
            string[] aText = Text.Split('/');
            if (aText.Length > 2)
            {

                Value = new Font(aText[0], aText[1].AsFloat() / 100f, (FontStyle)aText[2].AsInt32(), GraphicsUnit.Point);
                return true;
            }
            Value = new Font("Arial", 8f, FontStyle.Regular, GraphicsUnit.Point);
            return false;
        }

        /// <summary>
        /// Loads the variable from file.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVarFromFile(string SectionName, string KeyName, out string Value)
        {
            StringBuilder sData = new StringBuilder(" ", 1024);
            int Cnt = 0;
            Value = "";
            try
            {
                Cnt = SWinAPI.GetPrivateProfileString(SectionName, KeyName, "", sData, 1024, sIniFile);
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                throw;
            }
            if (Cnt > 0)
            {
                Value = sData.ToString().Substring(0, Cnt);
            }
            return Value.Length > 0;
        }

        /// <summary>
        /// Loads the variable from registry.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool LoadVarFromRegistry(string AppName, string SectionName, string KeyName, out string Value)
        {
            Value = "";
            StringBuilder sKeyname = new StringBuilder();
            sKeyname.Append(SVariableHandling.cBaseKey);
            if (AppName.Length > 0)
            {
                sKeyname.AppendFormat("\\{0}", AppName);
            }
            sKeyname.AppendFormat("\\{0}", SectionName);
            try
            {
                string[] aValue = (string[])Registry.GetValue(sKeyname.ToString(), KeyName, new string[1]
                {
                    string.Empty
                });
                if (aValue != null && aValue.Length > 0 && aValue[0] != string.Empty)
                {
                    Value = aValue[0];
                }
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                try
                {
                    Value = (string)Registry.GetValue(sKeyname.ToString(), KeyName, string.Empty);
                }
                catch (Exception Ex2)
                {
                    SLogging.Log(Ex2);
                    return false;
                }
            }
            return Value.Length > 0;
        }

        /// <summary>
        /// Saves the instanced variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Instance">The instance.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveInstancedVar(string AppName, string SectionName, string KeyName, int Instance, int Value)
        {
            return SaveInstancedVar(AppName, SectionName, KeyName, Instance, Value.ToString());
        }

        /// <summary>
        /// Saves the instanced variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Instance">The instance.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveInstancedVar(string AppName, string SectionName, string KeyName, int Instance, bool Value)
        {
            return SaveInstancedVar(AppName, SectionName, KeyName, Instance, Value ? "1" : "0");
        }

        /// <summary>
        /// Saves the instanced variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Instance">The instance.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveInstancedVar(string AppName, string SectionName, string KeyName, int Instance, Point Value)
        {
            return SaveInstancedVar(AppName, SectionName, KeyName, Instance, $"{Value.X}/{Value.Y}");
        }

        /// <summary>
        /// Saves the instanced variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Instance">The instance.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveInstancedVar(string AppName, string SectionName, string KeyName, int Instance, double Value)
        {
            SaveInstancedVar(AppName, SectionName, KeyName, Instance, Value.ToString());
            return true;
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, string Value)
        {
            if (SectionName.ToLower().Contains("control") || SectionName.ToLower().Contains("s.m.a.r.t.") || SectionName.ToLower().Contains("functionkey"))
            {
                return SaveInstancedVar(AppName, SectionName, KeyName, GetInstance(), Value);
            }
            return SaveInstancedVar(AppName, SectionName, KeyName, 0, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string SectionName, string KeyName, string Value)
        {
            return SaveVar(sAppName, SectionName, KeyName, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string SectionName, string KeyName, int Value)
        {
            return SaveVar(sAppName, SectionName, KeyName, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, int Value)
        {
            SaveVar(AppName, SectionName, KeyName, Value.ToString());
            return true;
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string SectionName, string KeyName, double Value)
        {
            return SaveVar(sAppName, SectionName, KeyName, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, double Value)
        {
            SaveVar(AppName, SectionName, KeyName, Value.ToString());
            return true;
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string SectionName, string KeyName, bool Value)
        {
            return SaveVar(sAppName, SectionName, KeyName, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">if set to <c>true</c> [value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, bool Value)
        {
            SaveVar(AppName, SectionName, KeyName, Convert.ToInt32(Value).ToString());
            return true;
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string SectionName, string KeyName, DateTime Value)
        {
            return SaveVar(sAppName, SectionName, KeyName, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, DateTime Value)
        {
            SaveVar(AppName, SectionName, KeyName, Value.ToString());
            return true;
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string SectionName, string KeyName, Point Value)
        {
            return SaveVar(sAppName, SectionName, KeyName, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, Point Value)
        {
            return SaveVar(AppName, SectionName, KeyName, $"{Value.X}/{Value.Y}");
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string SectionName, string KeyName, Size Value)
        {
            return SaveVar(sAppName, SectionName, KeyName, Value);
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, Size Value)
        {
            return SaveVar(AppName, SectionName, KeyName, $"{Value.Width}/{Value.Height}");
        }

        /// <summary>
        /// Saves the variable.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVar(string AppName, string SectionName, string KeyName, Font Value)
        {
            return SaveVar(AppName, SectionName, KeyName, $"{Value.Name}/{Value.Size * 100f}/{(int)Value.Style}");
        }

        /// <summary>
        /// Saves the variable to registry.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="SectionName">Name of the section.</param>
        /// <param name="KeyName">Name of the key.</param>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SaveVarToRegistry(string AppName, string SectionName, string KeyName, string Value)
        {
            string[] aValue = new string[1]
            {
                Value
            };
            StringBuilder sKeyname = new StringBuilder();
            sKeyname.Append(cBaseKey);
            if (AppName.Length > 0)
            {
                sKeyname.AppendFormat("\\{0}", AppName);
            }
            sKeyname.AppendFormat("\\{0}", SectionName);
            try
            {
                Registry.SetValue(sKeyname.ToString(), KeyName, aValue, RegistryValueKind.MultiString);
                return true;
            }
            catch (Exception Ex)
            {
                SLogging.Log(Ex);
                return false;
            }
        }
    }
}