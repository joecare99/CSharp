#if NETFRAMEWORK
using System;

namespace BaseLib.Helper;

public static class RuntimeHelper2
{
    public static T[] GetSubArray<T>(this T[] array, int index, int length)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (index < 0 || length < 0 || index + length > array.Length)
            throw new ArgumentOutOfRangeException();
        var result = new T[length];
        Array.Copy(array, index, result, 0, length);
        return result;
    }
}
#endif