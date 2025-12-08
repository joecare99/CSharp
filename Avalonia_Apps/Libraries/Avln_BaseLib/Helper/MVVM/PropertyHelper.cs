using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BaseLib.Helper.MVVM;

public static class PropertyHelper
{
    public static bool SetProperty<T>(this T value, ref T field, IRaisePropChangedEvents? This, [CallerMemberName] string propertyName = "")
        => value.SetProperty(ref field, null, This, propertyName);

    /// <summary>
    /// Helper for setting properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool SetProperty<T>(this T value, ref T field, Action<string,T?,T>? success=null, IRaisePropChangedEvents? This=null, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        T? old = field;
        field = value;
        SetTail(value, success, This, propertyName, old);
        return true;
    }

    /// <summary>
    /// Helper for setting properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool CondSetProperty<T>(this T value, ref T field, Func<string, T, T,bool>? cond = null, Action<string, T?, T>? success = null, IRaisePropChangedEvents? This = null,bool force=false, [CallerMemberName] string propertyName = "")
    {
        if (CondSetHead(value, field, cond, force, propertyName, out var old) != null) 
            return false;
        /*
        if (force || EqualityComparer<T>.Default.Equals(field, value)) return false;
        T? old = field;
        try { if (cond?.Invoke(propertyName, old, value) == false) return false; } 
        catch { return false; }*/
        field = value;
        SetTail(value, success, This, propertyName, old);
        return true;
    }

    /// <summary>
    /// Helper for setting properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static bool CondSetProperty<T>(this T value,T field, Action<T> Setter, 
        Func<string, T, T, bool>? cond = null, Action<string, T?, T>? success = null, 
        IRaisePropChangedEvents? This = null, bool force = false, [CallerMemberName] string propertyName = "")
    {
        if (CondSetHead(value, field, cond, force, propertyName, out var old) != null)
            return false;
        Setter(value);
        SetTail(value, success, This, propertyName, old);
        return true;
    }

    private static void SetTail<T>(T value, Action<string, T?, T>? success, IRaisePropChangedEvents? This, string propertyName, T? old)
    {
        try { success?.Invoke(propertyName, old, value); } catch { }
        This?.OnPropertyChanged(propertyName);
    }

    private static bool? CondSetHead<T>(T value, T field, Func<string, T, T, bool>? cond, bool force, string propertyName, out T? old)
    {
        old = field;
        if (!force && EqualityComparer<T>.Default.Equals(field, value)) return false;
        try { 
            if (cond?.Invoke(propertyName, old, value) == false) 
                return false; 
        }
        catch { 
            return false; 
        }
        return null;
    }
}
