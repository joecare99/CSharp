using GenFree2Base.Interfaces;
using System.Collections.Generic;

namespace GenInterfaces.Interfaces.Genealogic
{
    public interface IGenealogy : IGenBase
    {
        IList<IGenEntity> Entitys { get; init; }
        IList<IGenSources> Sources { get; init; }
        IList<IGenTransaction> Transactions { get; init; }
    }
}