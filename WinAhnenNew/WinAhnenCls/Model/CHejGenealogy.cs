﻿using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using WinAhnenCls.Model.HejInd;
using WinAhnenCls.Model.HejMarr;

namespace WinAhnenCls.Model
{
    public class CHejGenealogy : IData, IGenealogy
    {
        public int MarriagesCount { get; set; }
        public int IndividualCount { get; set; }
        public int GetActID { get; set; }
        public int PlaceCount { get; set; }
        public int SourceCount { get; set; }
        public int SpouseCount { get; set; }
        public int ChildCount { get; set; }
        public IGenPerson ActualInd { get; set; }
   
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        object IDataRO<object>.Data => throw new NotImplementedException();
        NotifyCollectionChangedEventHandler IDataRO<object>.OnUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IList<IGenPerson> Individuals { get; set; }
        public IList<CHejMarriageData> Marriages { get; set; }

        public CHejMarriageData ActualMarriage { get; set; }
        public Func<IList<object>, IGenEntity> GetEntity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<IList<object>, IGenFact> GetFact { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<IList<object>, IGenEntity> GetSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<IList<object>, IGenMedia> GetMedia { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<IList<object>, IGenEntity> GetTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<IGenEntity> Entitys { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IList<IGenSources> Sources { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IList<IGenRepository> Repositorys { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IList<IGenPlace> Places { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IList<IGenMedia> Medias { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IList<IGenTransaction> Transactions { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public Guid UId { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public EGenType eGenType => throw new NotImplementedException();

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

        public IGenPerson PeekInd(int v)
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
            var sr = new StreamReader(lStr);
            Individuals = CPerson.ReadFromStream(sr).ToList();

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