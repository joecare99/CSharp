// Ziel
// - Einfache Konvertierung von Dictionary<string,int> nach Dictionary<object,int>.
// - Optional: Semantik für Groß/Kleinschreibung (case-insensitive) beibehalten, falls die Quell-Dictionary das nutzt.

// Plan (Pseudocode)
// 1) Provide extension ToObjectKeyDictionary(IDictionary<string,int>):
//    - Use LINQ ToDictionary with key selector casting string to object.
// 2) Provide optional extension ToObjectKeyDictionaryIgnoreCase(IDictionary<string,int>):
//    - Create target Dictionary<object,int> with a custom IEqualityComparer<object> that treats object-keys of type string
//      case-insensitive (OrdinalIgnoreCase), otherwise falls back to default equality.
//    - Copy entries.
// 3) Keep API small and easy to use.

// Verwendung:
// var dictObj = myStringDict.ToObjectKeyDictionary();
// var dictObjIgnoreCase = myStringDict.ToObjectKeyDictionaryIgnoreCase();

using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalysis.Core.Utilities;

internal static class DictionaryHelpers
{
    public static Dictionary<object, int> ToObjectKeyDictionary(this IDictionary<string, int> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        return source.ToDictionary(kv => (object)kv.Key, kv => kv.Value);
    }

    public static Dictionary<object, int> ToObjectKeyDictionaryIgnoreCase(this IDictionary<string, int> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        var result = new Dictionary<object, int>(source.Count, ObjectStringIgnoreCaseComparer.Instance);
        foreach (var kv in source)
        {
            result[kv.Key] = kv.Value;
        }
        return result;
    }

    private sealed class ObjectStringIgnoreCaseComparer : IEqualityComparer<object>
    {
        public static readonly ObjectStringIgnoreCaseComparer Instance = new();

        public new bool Equals(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            if (x is string xs && y is string ys)
                return StringComparer.OrdinalIgnoreCase.Equals(xs, ys);

            return EqualityComparer<object>.Default.Equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            if (obj is null) return 0;
            if (obj is string s) return StringComparer.OrdinalIgnoreCase.GetHashCode(s);
            return obj.GetHashCode();
        }
    }
}