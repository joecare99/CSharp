using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
            => DoLog($"CanExChanged({sender})={(sender as IRelayCommand)?.CanExecute(null)}");

        protected virtual void OnVMPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => DoLog($"PropChg({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? "")}");

        protected virtual void OnVMPropertyChanging(object? sender, PropertyChangingEventArgs e)
            => DoLog($"PropChgn({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? "")}");

        protected virtual void OnVMErrorsChanged(object? sender, DataErrorsChangedEventArgs e) 
            => DoLog($"ErrorsChanged({sender},{e.PropertyName})={string.Join(",", ((List<ValidationResult>)(sender as INotifyDataErrorInfo)!.GetErrors(e.PropertyName)).ConvertAll(o=>o.ErrorMessage))}");
    }
}
