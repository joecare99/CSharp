using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLib.Helper
{
    public static class ClassHelper
    {
        public static object? GetProp<T>(this T cls, string propName) where T: class 
            => cls.GetType().GetProperty(propName)?.GetValue(cls);
        public static T2? GetProp<T,T2>(this T cls, string propName,T2 def) where T : class
            => (T2?)cls.GetType().GetProperty(propName,def?.GetType())?.GetValue(cls) ?? def;
        public static void SetProp<T, T2>(this T cls, string propName, T2 newVal) where T : class
            => cls.GetType().GetProperty(propName, newVal?.GetType() ?? typeof(T2))?.SetValue(cls,newVal);
        public static int IndexOf<T>(this IEnumerable<T> ar,T search)
            =>  ar.ToList().IndexOf(search);
        public static object? EnumMember(this Type t, string enumMbr)
            => t.GetEnumNames().ToList().IndexOf(enumMbr) is int i && i>=0? t.GetEnumValues().GetValue(i) :default;
    }
}
