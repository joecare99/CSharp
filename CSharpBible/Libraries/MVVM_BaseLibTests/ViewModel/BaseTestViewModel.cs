using BaseLib.Helper;
using System;
using System.ComponentModel;

namespace MVVM.ViewModel
{
    public class BaseTestViewModel
    {
        private string _debugLog="";

        protected string DebugLog => _debugLog;

        protected void DoLog(string v) 
            => _debugLog += $"{v}{Environment.NewLine}";

        protected void ClearLog() => _debugLog = "";

        protected virtual void OnCanExChanged(object? sender, EventArgs e)
            => DoLog($"OnCanExChanged: o:{sender}");

        protected virtual void OnVMPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => DoLog($"PropChg({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName)}");

        protected virtual void OnVMPropertyChanging(object? sender, PropertyChangingEventArgs e)
            => DoLog($"PropChgn({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName)}");

    }
}
