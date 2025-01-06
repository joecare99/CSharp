using System.Collections.Generic;

namespace GenInterfaces.Interfaces.Genealogic
{
    public interface IGenEntity : IGenObject
    {
        IList<IGenFact> Facts { get; init; }
        IList<IGenConnects> Connects { get; init; }
        IGenFact Start { get; }

        IGenFact End { get; }

    }
}