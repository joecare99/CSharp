using System;
using System.Collections.Generic;

namespace Basic_Del02_Filter.Models
{
    public static class CFilter
    {
        public static T[] Filter<T>(this T[] data,Predicate<T> predicate)
        {
            List<T> values = new();
            foreach (var item in data)
            {
                if (predicate.Invoke(item)) values.Add(item);
            }   
            return values.ToArray();
        }
    }
}
