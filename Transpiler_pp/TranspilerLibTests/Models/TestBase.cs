using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BaseLib.Helper;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace TranspilerLib.Models.Tests
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
{
    public class TestBase
    {
        protected void DoLog(string? message)
        {
            DebugLog += $"{message}{Environment.NewLine}";
            System.Diagnostics.Debug.WriteLine(message);
        } // End Sub DoLog

        protected void ErrorLog(Exception ex)
            => DoLog("Err: " + ex.Message + Environment.NewLine + ex.StackTrace);
        protected void ClearLog() => DebugLog = string.Empty;

        protected string DebugLog { get; private set; } = string.Empty;

        protected virtual void OnCanExChanged(object? sender, EventArgs e)
            => DoLog($"CanExChanged({sender})={(sender as IRelayCommand)?.CanExecute(null)}");

        protected virtual void OnVMPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => DoLog($"PropChg({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? "")}");

        protected virtual void OnVMPropertyChanging(object? sender, PropertyChangingEventArgs e)
            => DoLog($"PropChgn({sender},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? "")}");

        protected virtual void OnVMErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
            => DoLog($"ErrorsChanged({sender},{e.PropertyName})={string.Join(",", ((List<ValidationResult>)(sender as INotifyDataErrorInfo)!.GetErrors(e.PropertyName)).ConvertAll(o => o.ErrorMessage))}");

    }
}