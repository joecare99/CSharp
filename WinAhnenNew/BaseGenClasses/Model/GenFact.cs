using BaseGenClasses.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseGenClasses.Model;

[JsonDerivedType(typeof(GenFact), typeDiscriminator: nameof(GenFact))]

public class GenFact(IGenEntity _owner) : GenObject, IGenFact
{

    public EFactType eFactType { get; init ; }
    public IGenDate? Date { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IGenPlace? Place { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public IList<IGenSource?> Sources { get; init; } = new WeakLinkList<IGenSource>();
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public IGenEntity? MainEntity => (this as IHasOwner<IGenEntity>).Owner;
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    IGenEntity IHasOwner<IGenEntity>.Owner => _owner;
    [DataMember]
    public IList<IGenConnects?> Entities { get; init; } = [];
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public IList<IGenMedia?> Medias { get; init; } = new WeakLinkList<IGenMedia>();

    public override EGenType eGenType => EGenType.GenFact;
    [JsonConstructor()]
    private GenFact() :this(null!)
    {
    }
    public GenFact(IGenEntity Owner, EFactType eFactType):this(Owner)
    {
        this.eFactType = eFactType;
    }

    void IHasOwner<IGenEntity>.SetOwner(IGenEntity t)
    {
        if (t == null) return;
        _owner = t;
    }

}
