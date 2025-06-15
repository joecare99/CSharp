using BaseGenClasses.Helper;
using BaseGenClasses.Model;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using System.Text.Json.Serialization;
using BaseLib.Helper;

namespace BaseGenClasses.Model;

public class Genealogy : IGenealogy, IRecipient<IGenTransaction>, IDisposable
{
    private IMessenger _messanger;
    #region Properties
    public EGenType eGenType => EGenType.Genealogy;
    public Guid UId { get; init; }
    [JsonIgnore]
    public Func<IList<object?>, IGenEntity> GetEntity { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenFact> GetFact { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenEntity> GetSource { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenMedia> GetMedia { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenEntity> GetTransaction { get; set; }

    public IList<IGenEntity> Entitys { get; init; } = [];
    public IList<IGenSources> Sources { get; init; } = [];
    public IList<IGenPlace> Places { get; init; } = [];
    public IList<IGenRepository> Repositorys { get ; init ; }= [];
    public IList<IGenMedia> Medias { get; init; } = [];
    public IList<IGenTransaction> Transactions { get; init; } = [];

    #endregion

    #region Methods
    [JsonConstructor]
    private Genealogy() : this(IoC.GetRequiredService<IMessenger>()) { }
    public Genealogy(IMessenger messenger)
    {
        GetEntity = _GetEntity;
        GetFact = _GetFact;
        GetSource = _GetSource;
        GetMedia = _GetMedia;
        GetTransaction = _GetTransaction;
        (_messanger = messenger).Register(this);
    }

    private IGenEntity _GetEntity(IList<object?> o)
    {
        if ((o?.Count ?? 0) == 0)
            throw new ArgumentException("No data to create an entity");

        Guid? _uid = null;
        EGenType? _type = null;
        string? _name = null;
        DateTime? _date = null;
        foreach (var item in o!)
        {
            _uid = _uid ?? ((item is Guid g) ? g : null);
            _type = _type ?? ((item is EGenType t) ? t : null);
            _name = _name ?? ((item is string s) ? s : null);
            _date = _date ?? ((item is DateTime d) ? d : null);
        }
        if (_uid == null && _type == null)
            throw new ArgumentException("Not enough data to create an entity");

        var result = Entitys.FirstOrDefault(e => e.UId == _uid);
        if (result != null)
            return result;

        switch (_type)
        {
            case EGenType.GenPerson:
                if (_name == null)
                    result = new GenPerson() { UId = _uid ?? new Guid() };
                else
                {
                    result = new GenPerson() { UId = _uid ?? new Guid(), Name = _name };
                    if (_date != null)
                    {
                        result.AddEvent(EFactType.Birth, _date.Value, "");
                    }
                }
                break;
            case EGenType.GenFamily:
                if (_name == null)
                    result = new GenFamily() { UId = _uid ?? new Guid() };
                else
                {
                    result = new GenFamily() { UId = _uid ?? new Guid(), FamilyName = _name };
                    if (_date != null)
                    {
                        result.AddEvent(EFactType.Mariage, _date.Value, "");
                    }
                }
                break;
            default:
                throw new ArgumentException("Unknown entity type");
        }

        Entitys.Add(result);
        if (result is IHasOwner<IGenealogy> hasOwner) hasOwner.SetOwner(this);
        return result;
    }

    private IGenFact _GetFact(IList<object?> o)
    {
        if ((o?.Count ?? 0) == 0)
            throw new ArgumentException("No data to create an entity");

        Guid? _uid = null;
        EFactType? _type = null;
        IGenEntity? _entity = null;
        string? _data = null;
        foreach (var item in o!)
        {
            _uid = _uid ?? ((item is Guid g) ? g : null);
            _type = _type ?? ((item is EFactType t) ? t : null);
            _data = _data ?? ((item is string s) ? s : null);
            _entity = _entity ?? ((item is IGenEntity e) ? e : null);
        }
        if ((_uid == null && _type == null) || _entity == null)
            throw new ArgumentException("Not enough data to create an entity");

        var knownFact = _entity.Facts.FirstOrDefault(e => e?.UId == _uid);
        if (knownFact != null)
            return knownFact;

        var result = _entity.AddFact(_type!.Value, _data ?? "", _uid);

        return result;
    }

    private IGenEntity _GetSource(IList<object?> o)
    {
        throw new NotImplementedException();
    }

    private IGenMedia _GetMedia(IList<object?> o)
    {
        throw new NotImplementedException();
    }

    private IGenEntity _GetTransaction(IList<object?> o)
    {
        throw new NotImplementedException();
    }

    public void Receive(IGenTransaction message)
    {
        var _lastTA = Transactions.Where(ta => ta.Class == message.Class && ta.Entry == message.Entry).LastOrDefault();
        Transactions.Add(message);
    }

    public void Dispose()
    {
        _messanger.UnregisterAll(this);
        GC.SuppressFinalize(this);
    }

    #endregion
}
