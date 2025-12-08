using System;

namespace BaseLib.Model.Tests
{
    public class BaseTest
    {
        private string _debugLog = "";

        protected virtual void ClearLog()
        {
            _debugLog = string.Empty;
        }

        protected string DebugLog => _debugLog;

        protected virtual void DoLog(string st)
            => _debugLog += $"{st}{Environment.NewLine}";
    }
}
