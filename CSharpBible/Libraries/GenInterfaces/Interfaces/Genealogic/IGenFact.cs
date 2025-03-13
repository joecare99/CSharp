using GenInterfaces.Data;
using System.Collections.Generic;

namespace GenInterfaces.Interfaces.Genealogic
{
    public interface IGenFact : IGenObject
    {
        EFactType eFactType { get; init; }
        IGenDate Date { get; set; }
        IGenPlace Place { get; set; }
        string Data { get; set; }
        IList<IGenSources> Sources { get; init; }
        IGenEntity MainEntity { get; init; }
        IList<IGenConnects> Entities { get; init; }
    }
}