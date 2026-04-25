using System;

namespace gen_plusTests.Gen_FreeWin
{
    public class TestBase
    {
        private string _debugLog = string.Empty;

        protected void DoLog(string message)
        {
            _debugLog += message + Environment.NewLine;
            System.Diagnostics.Debug.WriteLine(message);
        } // End Sub DoLog
        
        protected void ErrorLog(Exception ex) => DoLog("Err: " + ex.Message + Environment.NewLine + ex.StackTrace);
        protected void ClearLog() => _debugLog = string.Empty;

        protected string DebugLog => _debugLog;
    }
}