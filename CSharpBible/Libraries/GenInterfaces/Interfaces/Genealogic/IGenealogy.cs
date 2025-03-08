using GenFree2Base.Interfaces;
using System;
using System.Collections.Generic;

namespace GenInterfaces.Interfaces.Genealogic
{
    public interface IGenealogy : IGenBase
    {
        
        Func<IList<object>,IGenEntity> GetEntity { get; set; }
        IList<IGenEntity> Entitys { get; init; }
        IList<IGenSources> Sources { get; init; }
        IList<IGenTransaction> Transactions { get; init; }
    }
}