using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace GenFree2Base.Interfaces
{
    public interface IGenEntity : IGenObject
    {
        IList<IGenFact> Facts { get; init; }
        IList<IGenConnects> Connects { get; init; }
        IGenFact Start { get; }

        IGenFact End { get; }

    }
}