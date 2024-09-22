using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenFree2Base.Interfaces
{
    public interface IGenFact : IGenObject    
    {
        IGenDate Date { get; set; }
        IGenPlace Place { get; set; }
        IList<IGenSources> Sources { get; init; }
        IGenEntity MainEntity { get; init; }
        IList<IGenConnects> Entities { get; init; }
    }
}