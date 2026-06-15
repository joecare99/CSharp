using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Avalonia.ViewModels;

public class BaseTestViewModel
{
    private string _debugLog = string.Empty;

    protected string DebugLog => _debugLog;

    protected void DoLog(string value)
        => _debugLog += $"{value}{Environment.NewLine}";

    protected void ClearLog() => _debugLog = string.Empty;

    protected virtual void OnCanExChanged(object? sender, EventArgs e)
        => DoLog($"CanExChanged({sender?.GetType().Name})={(sender as IRelayCommand)?.CanExecute(null)}");

    protected virtual void OnVMPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => DoLog($"PropChg({sender?.GetType().Name},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? string.Empty)}");

    protected virtual void OnVMPropertyChanging(object? sender, PropertyChangingEventArgs e)
        => DoLog($"PropChgn({sender?.GetType().Name},{e.PropertyName})={sender?.GetProp(e.PropertyName ?? string.Empty)}");

    protected virtual void OnVMErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        => DoLog($"ErrorsChanged({sender?.GetType().Name},{e.PropertyName})={string.Join(",", ((List<ValidationResult>)(sender as INotifyDataErrorInfo)!.GetErrors(e.PropertyName)).ConvertAll(o => o.ErrorMessage))}");
}