using GenInterfaces.Interfaces.Genealogic;
using System.Runtime.Serialization;

namespace BaseGenClasses.Model;

public abstract class GenEntity : GenObject, IGenEntity
{
    [DataMember]
    public IList<IGenFact> Facts { get; init; }
    [DataMember]
    public IList<IGenConnects> Connects { get; init; }

    public IGenFact? Start => GetStartFactOfEntity();

    public IGenFact? End => GetEndFactOfEntity();

    public IList<IGenSources> Sources { get; init; }

    abstract protected IGenFact? GetStartFactOfEntity();
    abstract protected IGenFact? GetEndFactOfEntity();
}