using System;

namespace Basic_Del01_Action.Models
{
    public static class CQuickSort
    {
        public static void QuickSort<T>(this Span<T> data,Action<string>? DWrt=null) where T : IComparable<T>
        {
            int i=0, j=data.Length-1, c=0;
            T pivot = data[(data.Length-1) /2];
            DWrt?.Invoke($"Privot[{(data.Length-1) / 2}]= {pivot}: {String.Join(", ", data.ToArray())}");
            while (i < j)
            {
                while ((c=data[i].CompareTo(pivot)) < 0) i++;
                while (i<j && data[j].CompareTo(pivot) >= c) j--;
                if (i < j /*&& data[i].CompareTo(data[j]) > 0*/)
                {
                    // Swap
                    DWrt?.Invoke($"SW {i},{j}: {String.Join(", ", data.ToArray())}");
                    (data[j], data[i]) = (data[i], data[j]);
                }
                else break;
               // if (j==i+1 && data[j].CompareTo(pivot) == 0) j--;
            }
            DWrt?.Invoke($"Split at {j}: {String.Join(", ",data.ToArray() )}");
            // Recursive calls
            if (1 < j) QuickSort(data[..j],DWrt);
            if (j+1 < data.Length) QuickSort(data[(j+1)..],DWrt);
        }
    }
}
