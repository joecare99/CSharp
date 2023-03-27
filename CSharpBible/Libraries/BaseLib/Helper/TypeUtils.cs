using System;
using System.Collections.Generic;
using System.Globalization;

namespace BaseLib.Helper
{
    public static class TypeUtils
    {
        private static Dictionary<Type, (string name, Func<object?, object?, bool>? compare, Func<object, object>? convert)> _typeDic = new(){
            {typeof(int),(nameof(System.Int32),(o1,o2)=>(int?)o1<(int?)o2,(o)=>Convert.ToInt32(o)) },
            {typeof(uint),(nameof(System.UInt32),(o1,o2)=>(uint ?) o1 <(uint ?) o2,(o)=>Convert.ToUInt32(o)) },
            {typeof(float),(nameof(System.Single),(o1,o2)=>(float ?) o1 <(float ?) o2,(o)=>Convert.ToSingle(o,CultureInfo.InvariantCulture)) },
            {typeof(double),(nameof(System.Double),(o1,o2)=>(double ?) o1 <(double ?) o2,(o)=>Convert.ToDouble(o,CultureInfo.InvariantCulture)) },
            {typeof(bool),(nameof(System.Boolean),(o1,o2)=>!(bool)o1!&&(bool)o2!,(o)=>Convert.ToBoolean(o)) },
            {typeof(long),(nameof(System.Int64),(o1,o2)=>(long ?) o1 <(long ?) o2,(o)=>Convert.ToInt64(o)) },
            {typeof(ulong),(nameof(System.UInt64),(o1,o2)=>(ulong ?) o1 <(ulong ?) o2,(o)=>Convert.ToUInt64(o)) },
            {typeof(short),(nameof(System.Int16),(o1,o2)=>(short ?) o1 <(short ?) o2,(o)=>Convert.ToInt16(o)) },
            {typeof(ushort),(nameof(System.Int16),(o1,o2)=>(ushort ?) o1 <(ushort ?) o2,(o)=>Convert.ToUInt16(o)) },
            {typeof(byte),(nameof(System.Byte),(o1,o2)=>(byte ?) o1 <(byte ?) o2,(o)=>Convert.ToByte(o)) },
            {typeof(sbyte),(nameof(System.SByte),(o1,o2)=>(sbyte ?) o1 <(sbyte ?) o2,(o)=>Convert.ToSByte(o)) },
            {typeof(string),(nameof(System.String),null,(o)=>Convert.ToString(o,CultureInfo.InvariantCulture)) }
            };
        public static TypeCode TC(this Type type) => Type.GetTypeCode(type);
        public static Type ToType(this TypeCode tc) => Type.GetType("System."+tc.ToString(),false,true);

        public static bool Compare(this Type type, object? o1, object? o2)
            => _typeDic.ContainsKey(type) ? _typeDic[type].compare?.Invoke(o1, o2)??false : false;

        public static object? Get(this Type type, object? o1)
            => (type.TC(), o1) switch {
                (TypeCode tc, object o) when o.GetType().TC() == tc 
                    => o1,
                (_, string s) when string.IsNullOrWhiteSpace(s) && _typeDic.ContainsKey(type) 
                    => _typeDic[type].convert?.Invoke(0),
                (_, null) when _typeDic.ContainsKey(type) 
                    => _typeDic[type].convert?.Invoke(0),
                (_, _) when _typeDic.ContainsKey(type) 
                    => _typeDic[type].convert?.Invoke(o1!),
                _ => o1
            };

        public static Type ToType(this string type)
        {
            try
            {
                return Type.GetType($"System.{type}", true, true);
            }
            catch (Exception)
            {
                return typeof(object);
            }
        }

        public static bool CheckLimit<T>(this T val, object? min, object? max) where T : struct 
            => !typeof(T).Compare(min, max) 
            || (!typeof(T).Compare(val, min) 
               && !typeof(T).Compare(max, val));

        public static bool IsBetweenIncl(this IComparable val, IComparable min, IComparable max)
            => (min.CompareTo(val) <=0)
               && (max.CompareTo(val) >=0);
        public static bool IsBetweenExcl(this IComparable val, IComparable min, IComparable max)
            => (min.CompareTo(val) < 0)
               && (max.CompareTo(val) > 0);

    }
}
