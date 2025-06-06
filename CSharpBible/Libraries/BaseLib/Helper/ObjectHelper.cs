﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BaseLib.Interfaces;

namespace BaseLib.Helper;

public static class ObjectHelper
{
    public static int AsInt(this object? obj, int def = default) => obj switch
    {
        int i => i,
        uint ui => unchecked((int)ui),
        string s => int.TryParse(s, out int i) ? i : def,
        IHasValue f => f.Value.AsInt(),
        null => def,
        DBNull => def,
        IConvertible c => c.ToInt32(CultureInfo.InvariantCulture),
        _ => def
    };

    public static long AsLong(this object? obj, int def = default) => obj switch
    {
        long i => i,
        ulong ul => unchecked((long)ul),
        string s => long.TryParse(s, out long i) ? i : def,
        IHasValue f => f.Value.AsLong(),
        null => def,
        DBNull => def,
        IConvertible c => c.ToInt64(CultureInfo.InvariantCulture),
        _ => def
    };


    public static T AsEnum<T>(this object? obj) where T : struct, Enum => obj switch
    {
        T t => t,
        int i when i < Enum.GetValues(typeof(T)).Length => (T)Enum.ToObject(typeof(T), i),
        string s when Enum.TryParse<T>(s, out var t) => t,
//        string s when int.TryParse(s, out var i) => (T)Enum.ToObject(typeof(T), i),
        string s=> default,
        IHasValue f => f.Value.AsEnum<T>(),
        null => default,
        DBNull => default,
        uint ui => (T)Enum.ToObject(typeof(T), unchecked((int)ui)),
        IConvertible c => (T)Enum.ToObject(typeof(T), c.ToInt32(CultureInfo.InvariantCulture)),
        _ => default
    };

    public static DateTime AsDate(this object? obj) => obj switch
    {
        DateTime dt => dt,
        int i when (i%100 is >0 and <32) && (i/100%100 is >0 and <13) => new(i / 10000, i % 10000 / 100, i % 100),
        int i when i == 0 || i>10000000 => default,
        IHasValue f => f.Value.AsDate(),
        string s when !s.Contains('.') && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) => dt,
        string s when DateTime.TryParse(s, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var dt) => dt,
        string s when int.TryParse(s, out var i) && (i % 100 is > 0 and < 32) && (i / 100 % 100 is > 0 and < 13) => new(i / 10000, i % 10000 / 100, i % 100),
        string s when !int.TryParse(s, out var i) || i==0 => default,
        long l => new(l),
        uint ui => DateTime.FromOADate(unchecked((int)ui)),
        char c => new(c+1900,1,1),
        DBNull => default,
        IConvertible c => DateTime.FromOADate(c.ToDouble(CultureInfo.InvariantCulture)),
        _ => default,
    };

    public static double AsDouble(this object? obj, CultureInfo? culture = null) => obj switch
    {
        double d => d,
        string s when double.TryParse(s, NumberStyles.Float, culture ?? CultureInfo.InvariantCulture, out var d) => d,
        string s => default,
        IHasValue f => f.Value.AsDouble(culture),
        null => default,
        DBNull => default,
        char c => c,
        IConvertible c => c.ToDouble(culture ?? CultureInfo.InvariantCulture),
        _ => default
    };

    public static bool AsBool(this object? obj) => obj switch
    {
        bool x => x,
        IHasValue f => f.Value.AsBool(),
        null => default,
        DBNull => default,
        string s when bool.TryParse(s, out var b) => b,
        string s when s!="1" => false,
        char c => c is '1' or 'T' or 't',
        IConvertible c => !((IConvertible)0).Equals(c),
        _ => default
    };

    public static Guid AsGUID(this object? obj) => obj switch
    {
        Guid g => g,
        IHasValue f => f.Value.AsGUID(),
        null => default,
        DBNull => default,
        string s when Guid.TryParse(s, out var g) => g,
        string s when int.TryParse(s, out var i) => new Guid(i, 0, 0, new byte[8]),
        string s => default,
        uint ui => new Guid(unchecked((int)ui), 0, 0, new byte[8]),
        IConvertible c => new Guid(c.ToInt32(CultureInfo.InvariantCulture),0,0,new byte[8]),
        _ => default
    };

    public static T SetRet<T, T2>(this T2 obj, Action<T2> action, T v)
    {
        action(obj);
        return v;
    }
}
public static class ObjectHelper2
{
    public static void SetIndex<T>(this Dictionary<int, T> dic, T value, int index) => dic[index + 1] = value;
    public static int GetIndex<T>(this Dictionary<int, T> dic, T value) => dic.Where((itm) => itm.Value?.Equals(value) ?? false)
        .FirstOrDefault().Key - 1;

}
public class ControlArray<T> : Dictionary<int, T> { public EventHandler DoubleClick { get; set; } };
