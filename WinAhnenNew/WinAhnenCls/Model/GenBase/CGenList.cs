using Gen_BaseItf.Model.Interface;
using System.Collections;
using System.Collections.Generic;
using WinAhnenCls.Model.HejInd;

namespace WinAhnenCls.Model.GenBase
{
    public class CGenList<T> : IIndexedList<T> where T : IGenData
    {
        public CGenList(IGenListProvider<T> prov, EGenListType ltype)
        {
            Prov = prov;
            LType = ltype;
        }

        private IGenListProvider<T> Prov { get; }
        private EGenListType LType { get; }

        public int Count => Prov.Count(LType);

        public bool IsReadOnly => Prov.IsReadOnly(LType);

        public T this[object Idx] { get => Prov.GetGenList(Idx, LType); set => Prov.SetGenList(Idx, LType, value); }

        public object IndexOf(T item) => Prov.IndexOf(item, LType);

        public void Insert(object index, T item) => Prov.Insert(index, item, LType);

        public void RemoveAt(object index) => Prov.RemoveAt(index, LType);

        public void Add(T item) => Prov.Add(item, LType);

        public void Clear() => Prov.Clear(LType);

        public bool Contains(T item) => Prov.Contains(item, LType);

        public void CopyTo(T[] array, int arrayIndex) => Prov.CopyTo(array, arrayIndex, LType);

        public bool Remove(T item) => Prov.Remove(item, LType);

        public IEnumerator<T> GetEnumerator() => Prov.GetEnumerator(LType);

        IEnumerator IEnumerable.GetEnumerator() => Prov.GetEnumerator(LType);
    }
}
