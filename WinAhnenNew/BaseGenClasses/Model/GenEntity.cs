using BaseGenClasses.Helper;
using GenInterfaces.Interfaces;
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
    private WeakReference<IGenealogy>? _WLowner; //This is used to avoid circular references in the serialization process
    #endregion
    [DataMember]
    public IList<IGenFact?> Facts { get; init; } = new WeakLinkList<IGenFact>();
    [DataMember]
    public IList<IGenConnects?> Connects { get; init; }= new WeakLinkList<IGenConnects>();
    [JsonIgnore]

    public IGenFact? Start => GetStartFactOfEntity();
    [JsonIgnore]

    public IGenFact? End => GetEndFactOfEntity();

    [JsonIgnore]
    public IList<IGenSource?> Sources { get; init; } = new WeakLinkList<IGenSource>();

    [JsonIgnore]
    public IList<IGenMedia?> Media { get; init; } = new WeakLinkList<IGenMedia>();

    [JsonIgnore]
    public IGenealogy? Owner => (_WLowner?.TryGetTarget(out var t)??false)?t:null;
    #endregion
    abstract protected IGenFact? GetStartFactOfEntity();
    abstract protected IGenFact? GetEndFactOfEntity();

    void IHasOwner<IGenealogy>.SetOwner(IGenealogy t)
    {
        if (t == null) return;
        _WLowner = new(t);
    }
}