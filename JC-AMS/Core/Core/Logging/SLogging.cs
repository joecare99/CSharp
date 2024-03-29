﻿// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 09-24-2022
//
// Last Modified By : Mir
// Last Modified On : 10-13-2022
// ***********************************************************************
// <copyright file="SLogging.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

/// <summary>
/// The Logging namespace.
/// </summary>
/// <autogeneratedoc />
namespace JCAMS.Core.Logging
{
    /// <summary>
    /// Class TLogging.
    /// </summary>
    public static class SLogging
    {
        #region Properties
        /// <summary>
        /// Do write the log
        /// </summary>
        /// <autogeneratedoc />
        public static Func<object, string, object[], bool> DoWriteLog = (o, s, p) =>
        {
            switch (o) {
                case string f: return SFileHelpers.WriteToFile(f, s, p);
                case Stream fs: return SFileHelpers.WriteToStream(fs, s, p);
                case IProtocol pr: return pr.Write((int)p[0], s);
                case EventLog el: el.WriteEntry(s); return true;
                default: return false;
            }
        };

        /// <summary>
        /// Gets or sets the cx ams.
        /// </summary>
        /// <value>The cx ams.</value>
        /// <autogeneratedoc />
        public static string cxAMS { get; set; } = "JCAMS";
        /// <summary>
        /// Gets or sets a value indicating whether [x trigger error].
        /// </summary>
        /// <value><c>true</c> if [x trigger error]; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public static bool xTriggerError { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether [x application is starting].
        /// </summary>
        /// <value><c>true</c> if [x application is starting]; otherwise, <c>false</c>.</value>
        /// <autogeneratedoc />
        public static bool xAppIsStarting { get; set; } = true;

        /// <summary>
        /// Gets or sets the jt filter topic.
        /// </summary>
        /// <value>The jt filter topic.</value>
        /// <autogeneratedoc />
        public static ELogTopic jtFilterTopic { get; set; } = 0;

        /// <summary>
        /// Occurs when [error handler].
        /// </summary>
        /// <autogeneratedoc />
        public static event EventHandler<ErrorEventArgs> ErrorHandler;

        /// <summary>
        /// The m protocol
        /// </summary>
        private static IProtocol _Protocol;

        /// <summary>
        /// The m startup logfile
        /// </summary>
        private static string _StartupLogfile;

        /// <summary>
        /// The lb settabstops
        /// </summary>
        public const int LB_SETTABSTOPS = 402;

        /// <summary>
        /// Gets the protocol filename.
        /// </summary>
        /// <value>The protocol filename.</value>
        public static string ProtocolFilename => _Protocol == null ? "" : _Protocol.Filename;

        /// <summary>
        /// Gets a value indicating whether [protocol to file].
        /// </summary>
        /// <value><c>true</c> if [protocol to file]; otherwise, <c>false</c>.</value>
        public static bool ProtocolToFile => _Protocol != null && _Protocol.IsFile;

        /// <summary>
        /// Gets a value indicating whether [protocol to event log].
        /// </summary>
        /// <value><c>true</c> if [protocol to event log]; otherwise, <c>false</c>.</value>
        public static bool ProtocolToEventLog => _Protocol != null && _Protocol.IsEventLog;

        /// <summary>
        /// Gets or sets the event log.
        /// </summary>
        /// <value>The event log.</value>
        /// <autogeneratedoc />
        public static EventLog EventLog { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes static members of the <see cref="SLogging" /> class.
        /// </summary>
        static SLogging()
        {
            _StartupLogfile = $"c:\\{cxAMS}startup.txt"; // TODO !!!! Allgemeiner machen !!!!
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="oDest">The o dest.</param>
        /// <param name="Format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <autogeneratedoc />
        private static bool WriteLog(object oDest, string Format, params object[] args) 
            => DoWriteLog(oDest, Format, args);

        /// <summary>
        /// Sets the protocol.
        /// </summary>
        /// <param name="aVal">a value.</param>
        public static void SetProtocol(IProtocol aVal) 
            => _Protocol = aVal;

        /// <summary>
        /// Debugs the print.
        /// </summary>
        /// <param name="Format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void DebugPrint(string Format, params object[] args) 
            => Debug.Print("{0} {1}", DateTime.Now.ToString("HH:mm:ss.fff "), string.Format(Format, args));

        /// <summary>
        /// Debugs the print.
        /// </summary>
        /// <param name="T">The t.</param>
        public static void DebugPrint(string T) 
            => Debug.Print("{0} {1}", DateTime.Now.ToString("HH:mm:ss.fff "), T);

        /// <summary>
        /// Deletes the startup log.
        /// </summary>
        public static void DeleteStartupLog() 
            => SFileHelpers.DeleteFile(_StartupLogfile);

        /// <summary>
        /// Journalizes the specified ex.
        /// </summary>
        /// <param name="Ex">The ex.</param>
        /// <param name="name">The name.</param>
        public static void Log(Exception Ex, [CallerMemberName] string name = "")
        {
            if (xTriggerError)
            {
                ErrorHandler?.Invoke(name, new ErrorEventArgs(Ex));
            }
            Log(ELogTopic.Always, -1, SExceptionHelper.AsString(Ex), name);
        }

        /// <summary>
        /// Journalizes the specified format.
        /// </summary>
        /// <param name="Format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Log(string Format, params object[] args) 
            => Log(ELogTopic.Always, -1, Format, args);

        /// <summary>
        /// Journalizes the specified journalize topic.
        /// </summary>
        /// <param name="JournalizeTopic">The journalize topic.</param>
        /// <param name="Format">The format.</param>
        /// <param name="Arguments">The arguments.</param>
        public static void Log(ELogTopic JournalizeTopic, string Format, params object[] Arguments) 
            => Log(JournalizeTopic, -1, Format, Arguments);

        /// <summary>
        /// Journalizes the specified journalize topic.
        /// </summary>
        /// <param name="JournalizeTopic">The journalize topic.</param>
        /// <param name="IntervalMSec">The interval m sec.</param>
        /// <param name="Format">The format.</param>
        /// <param name="Arguments">The arguments.</param>
        public static void Log(ELogTopic JournalizeTopic, int IntervalMSec, string Format, params object[] Arguments)
        {
            try
            {
                if (xAppIsStarting && (JournalizeTopic == ELogTopic.Always || JournalizeTopic == ELogTopic.StartUp))
                {
                    WriteLog(_StartupLogfile, Format, Arguments);
                    WriteLog(_StartupLogfile, "\n",null);
                    return;
                }
                else if (JournalizeTopic != ELogTopic.Always 
                    && (jtFilterTopic & JournalizeTopic) < ELogTopic.Fail)
                {
                    return;
                }
            }
            catch (Exception)
            {
                WriteLog(_StartupLogfile, $"Journalize FAILED ({cxAMS}.Config)\n");
            }
            try
            {
                if (_Protocol != null)
                {
                    try
                    {
                        WriteLog(_Protocol, string.Format(Format, Arguments), IntervalMSec);
                    }
                    catch (Exception Ex2)
                    {
                        WriteLog(_StartupLogfile, "Protocol.Write FAILED\n{0}\n", Ex2.Message);
                    }
                }
                else if (SLogging.EventLog != null)
                {
                    try
                    {
                        WriteLog(_StartupLogfile, Format, Arguments);
                        WriteLog(_StartupLogfile, "\n");
                        WriteLog(SLogging.EventLog, string.Format(Format, Arguments));
                    }
                    catch (Exception Ex2)
                    {
                        WriteLog(_StartupLogfile, "{1}.Environment.EventLog.WriteEntry FAILED\n{0}\n", Ex2.Message, cxAMS);
                    }
                }
                else
                {
                    WriteLog(_StartupLogfile, Format, Arguments);
                    WriteLog(_StartupLogfile, "\n");
                }
            }
            catch (Exception Ex2)
            {
                try
                {
                    WriteLog(_StartupLogfile, "Journalize FAILED\n{0}\n", SExceptionHelper.AsString(Ex2));
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion
    }
}