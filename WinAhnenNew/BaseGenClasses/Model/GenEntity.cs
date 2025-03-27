using BaseGenClasses.Helper;
using GenInterfaces.Interfaces.Genealogic;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Model;

[DataContract]
[JsonDerivedType(typeof(GenEntity), typeDiscriminator: nameof(GenEntity))]
public abstract class GenEntity : GenObject, IGenEntity
{
    [DataMember]
    public IList<IGenFact> Facts { get; init; } = new WeakLinkList<IGenFact>();
    [DataMember]
    public IList<IGenConnects> Connects { get; init; }= new WeakLinkList<IGenConnects>();
    [JsonIgnore]

    public IGenFact? Start => GetStartFactOfEntity();
    [JsonIgnore]

    public IGenFact? End => GetEndFactOfEntity();

    [JsonIgnore]
    public IList<IGenSources> Sources { get; init; } = new WeakLinkList<IGenSources>();

    [JsonIgnore]
    public IList<IGenMedia> Media { get; init; } = new WeakLinkList<IGenMedia>();

    abstract protected IGenFact? GetStartFactOfEntity();
    abstract protected IGenFact? GetEndFactOfEntity();
}