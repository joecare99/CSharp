using BaseGenClasses.Helper;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Model;

[DataContract]
[JsonDerivedType(typeof(GenEntity), typeDiscriminator: nameof(GenEntity))]
public abstract class GenEntity : GenObject, IGenEntity
{
    #region Properties
    #region private properties
    private WeakReference<IGenealogy>? _WLowner;
    #endregion
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

    [JsonIgnore]
    public IGenealogy? Owner => (_WLowner?.TryGetTarget(out var t)??false)?t:null;
    #endregion
    abstract protected IGenFact? GetStartFactOfEntity();
    abstract protected IGenFact? GetEndFactOfEntity();
}