using System;
using System.Collections;
using System.Collections.Generic;

namespace VBUnObfusicator.Models
{
    public partial class CSCode
    {
        public class ParentedItemsList<T> : IHasParents<T>, IList<T> where T : class, IHasParents<T>, IEquatable<T>
        {
            List<T> list = new();
            public T Parent { get; set; }
            public int Count => list.Count;

            public bool IsReadOnly => false;

            public T this[int index] { get => list[index]; set => list[index] = value; }

            public ParentedItemsList(T _parent) {
                Parent = _parent;
            }

            public int IndexOf(T item) => list.IndexOf(item);

            public void Insert(int index, T item)
            {
                if (!item.Parent?.Equals(Parent) ?? Parent != null)
                    item.Parent = Parent;
                list.Insert(index, item);   
            }

            public void RemoveAt(int index) => list.RemoveAt(index);

            public void Add(T item)
            {
                if (!item.Parent?.Equals(Parent) ?? Parent != null)
                    item.Parent = Parent;
                list.Add(item);
            }

            public void Clear() => list.Clear();

            public bool Contains(T item) => list.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array,arrayIndex);

            public bool Remove(T item) => list.Remove(item);

            public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
         }
    }
}
