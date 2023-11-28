using System.Collections.Generic;
using WinAhnenCls.Model.GenBase;

namespace WinAhnenCls.Model.HejInd
{
    public interface IGenListProvider<T> where T : IGenData
    {
        int Count(EGenListType lType);
        bool IsReadOnly(EGenListType lType);

        T GetGenList(object idx, EGenListType lType);
        void SetGenList(object idx, EGenListType lType, T value);
        object IndexOf(T item, EGenListType lType);
        void Insert(object index, T item, EGenListType lType);
        IEnumerator<T> GetEnumerator(EGenListType lType);
        void RemoveAt(object index, EGenListType lType);
        void Add(T item, EGenListType lType);
        void Clear(EGenListType lType);
        void CopyTo(T[] array, int arrayIndex, EGenListType lType);
        bool Contains(T item, EGenListType lType);
        bool Remove(T item, EGenListType lType);
    }
}