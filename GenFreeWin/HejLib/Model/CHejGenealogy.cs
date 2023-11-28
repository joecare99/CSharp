using System;
using System.Collections.Specialized;
using System.IO;
using WinAhnenCls.Model.HejInd;
using WinAhnenCls.Model.HejMarr;

namespace WinAhnenCls.Model
{
    public class CHejGenealogy : IData
    {
        public int MarriagesCount { get; set; }
        public int IndividualCount { get; set; }
        public int GetActID { get; set; }
        public int PlaceCount { get; set; }
        public int SourceCount { get; set; }
        public CHejIndiData ActualInd { get; set; }
        public int SpouseCount { get; set; }
        public int ChildCount { get; set; }
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        object IDataRO<object>.Data => throw new NotImplementedException();

        NotifyCollectionChangedEventHandler IDataRO<object>.OnUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CHejMarriageData ActualMarriage { get; set; }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Append()
        {
            throw new NotImplementedException();
        }

        public void SetPlace(object value)
        {
            throw new NotImplementedException();
        }

        public void AppendSpouse()
        {
            throw new NotImplementedException();
        }

        public CHejIndiData PeekInd(int v)
        {
            throw new NotImplementedException();
        }

        public void Append(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Edit(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Post(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Cancel(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Delete(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public object GetData()
        {
            throw new NotImplementedException();
        }

        public void First(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Last(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Next(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Previous(object? sender = null)
        {
            throw new NotImplementedException();
        }

        public void Seek(int Id)
        {
            throw new NotImplementedException();
        }

        public bool EOF()
        {
            throw new NotImplementedException();
        }

        public bool BOF()
        {
            throw new NotImplementedException();
        }

        int IDataRO<object>.GetActID()
        {
            throw new NotImplementedException();
        }

        public void SetSource(object value)
        {
            throw new NotImplementedException();
        }

        public void ReadFromStream(Stream lStr)
        {
            throw new NotImplementedException();
        }

        public object? GetData(int v, EHejIndDataFields hind_idFather)
        {
            throw new NotImplementedException();
        }

        public void AppendParent(EHejIndDataFields hind_idFather)
        {
            throw new NotImplementedException();
        }

        public void GotoChild()
        {
            throw new NotImplementedException();
        }

        public void AppendAdoption(int v)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> OnStateChange;
        public event EventHandler<EventArgs> OnUpdate;
        public event EventHandler<EventArgs> OnDataChange;

    }
}