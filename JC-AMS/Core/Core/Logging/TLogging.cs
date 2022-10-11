// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 09-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="TLogging.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace JCAMS.Core.Logging
{
    /// <summary>
    /// Class TLogging.
    /// </summary>
    public static class TLogging
    {
        #region Properties
        public static Func<object, string, object[], bool> DoWriteLog = (o, s, p) =>
        {
            switch (o) {
                case string f: return TFileHelpers.WriteToFile(f, s, p);
                case Stream fs: return TFileHelpers.WriteToStream(fs, s, p);
                case IProtocol pr: return pr.Write((int)p[0], s);
                case EventLog el: el.WriteEntry(s); return true;
                default: return false;
            }
        };

        public static string cxAMS { get; set; } = "JCAMS";
        public static bool xTriggerError { get; set; } = false;
        public static bool xAppIsStarting { get; set; } = true;

        public static ELogTopic jtFilterTopic { get; set; } = 0;

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

        public static EventLog EventLog { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes static members of the <see cref="TLogging"/> class.
        /// </summary>
        static TLogging()
        {
            _StartupLogfile = $"c:\\{cxAMS}startup.txt"; // TODO !!!! Allgemeiner machen !!!!
        }

        private static bool WriteLog(object oDest, string Format, params object[] args) 
            => DoWriteLog(oDest, Format, args);

        /// <summary>
        /// Sets the protocol.
        /// </summary>
        /// <param name="TestPath">The test path.</param>
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
            => TFileHelpers.DeleteFile(_StartupLogfile);

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
            Log(ELogTopic.Always, -1, TExceptionHelper.AsString(Ex), name);
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
                else if (TLogging.EventLog != null)
                {
                    try
                    {
                        WriteLog(_StartupLogfile, Format, Arguments);
                        WriteLog(_StartupLogfile, "\n");
                        WriteLog(TLogging.EventLog, string.Format(Format, Arguments));
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
                    WriteLog(_StartupLogfile, "Journalize FAILED\n{0}\n", TExceptionHelper.AsString(Ex2));
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion
    }
}