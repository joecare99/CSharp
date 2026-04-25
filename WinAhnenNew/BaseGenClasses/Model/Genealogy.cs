using BaseGenClasses.Helper;
using BaseGenClasses.Persistence;
using BaseGenClasses.Model;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BaseGenClasses.Persistence.Interfaces;

namespace BaseGenClasses.Model;

public class Genealogy : IGenealogy, IGenealogyPersistenceContext, IGenealogyJournalContext, IRecipient<IGenTransaction>, IDisposable
{
    private readonly IMessenger _messanger;
    private IGenealogyPersistenceProvider? _persistenceProvider;
    private IGenEntity? _genLastChangedEntity;
    #region Properties
    public EGenType eGenType => EGenType.Genealogy;
    public Guid UId { get; init; }
    [JsonIgnore]
    public Func<IList<object?>, IGenEntity> GetEntity { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenFact> GetFact { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenSource> GetSource { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenMedia> GetMedia { get; set; }
    [JsonIgnore]
    public Func<IList<object?>, IGenTransaction> GetTransaction { get; set; }

    public IList<IGenEntity> Entitys { get; init; } = [];
    public IList<IGenSource> Sources { get; init; } = [];
    public IList<IGenPlace> Places { get; init; } = [];
    public IList<IGenRepository> Repositories { get ; init ; }= [];
    public IList<IGenMedia> Medias { get; init; } = [];
    public IList<IGenTransaction> Transactions { get; init; } = [];

    public bool xDirty { get; private set; }

    public event EventHandler<DirtyStateChangedEventArgs>? DirtyStateChanged;

    public event EventHandler<FlushRequestedEventArgs>? FlushRequested;

    public event EventHandler<FlushCompletedEventArgs>? Flushed;

    public event EventHandler<FlushFailedEventArgs>? FlushFailed;

    public event EventHandler<JournalEntryRecordedEventArgs>? JournalEntryRecorded;

    public IReadOnlyList<IGenTransaction> JournalEntries => Transactions.ToArray();

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

    private IGenSource _GetSource(IList<object?> o)
    {
        if ((o?.Count ?? 0) == 0)
            throw new ArgumentException("No data to create an entity");

        Guid? _uid = null;
        IGenFact? _fact = null;
        string? _data = null;
        Uri? _www = null;
        string? _description = null;

        foreach (var item in o!)
        {
            _uid = _uid ?? ((item is Guid g) ? g : null);
            _fact = _fact ?? ((item is IGenFact f) ? f : null);
            if (item is string s)
            {
                if (s.StartsWith("http")) _www = new(s);
                else if (s.Length < 100 && !s.Contains('\n')) _description = s;
                else if (_data == null) _data = s;
            }
        }

        if (_uid == null && (_fact == null || _data == null))
            throw new ArgumentException("Not enough data to create an entity");

        var knownFact = Sources.FirstOrDefault(e => e?.UId == _uid);
            return knownFact;

    }

    private IGenMedia _GetMedia(IList<object?> o)
    {
        throw new NotImplementedException();
    }

    private IGenTransaction _GetTransaction(IList<object?> o)
    {
        throw new NotImplementedException();
    }

    public void Receive(IGenTransaction message)
    {
        RecordIncomingTransaction(message);
        MarkDirty(null, "A genealogy transaction was recorded.");
    }

    public IGenTransaction RecordJournalEntry(IGenBase genClass, IGenBase genEntry, object? objData, object? objOldData)
    {
        if (genClass is null)
        {
            throw new ArgumentNullException(nameof(genClass));
        }

        if (genEntry is null)
        {
            throw new ArgumentNullException(nameof(genEntry));
        }

        var genTransaction = new GenTransaction
        {
            UId = Guid.NewGuid(),
            Class = genClass,
            Entry = genEntry,
            Data = objData,
            OldData = objOldData,
            Timestamp = DateTime.UtcNow,
            Prev = Transactions.LastOrDefault()
        };

        ((IHasOwner)genTransaction).SetOwner(this);
        return RecordIncomingTransaction(genTransaction);
    }

    public void AttachPersistenceProvider(IGenealogyPersistenceProvider persistenceProvider)
    {
        _persistenceProvider = persistenceProvider ?? throw new ArgumentNullException(nameof(persistenceProvider));
    }

    private IGenTransaction RecordIncomingTransaction(IGenTransaction genTransaction)
    {
        if (Transactions.LastOrDefault() is GenTransaction genPreviousTransaction)
        {
            genPreviousTransaction.SetNext(genTransaction);
        }

        Transactions.Add(genTransaction);
        JournalEntryRecorded?.Invoke(this, new JournalEntryRecordedEventArgs(genTransaction));
        return genTransaction;
    }

    public void MarkDirty(IGenEntity? genChangedEntity = null, string? sReason = null)
    {
        _genLastChangedEntity = genChangedEntity ?? _genLastChangedEntity;
        xDirty = true;
        DirtyStateChanged?.Invoke(this, new DirtyStateChangedEventArgs(true, genChangedEntity, sReason));
    }

    public async Task FlushAsync(
        IGenEntity? genRequestedEntity = null,
        GenealogyFlushScope eScope = GenealogyFlushScope.Auto,
        CancellationToken cancellationToken = default)
    {
        var genEntityToFlush = genRequestedEntity ?? _genLastChangedEntity;
        FlushRequested?.Invoke(this, new FlushRequestedEventArgs(genEntityToFlush, eScope, xDirty));

        if (!xDirty)
        {
            Flushed?.Invoke(this, new FlushCompletedEventArgs(genEntityToFlush, eScope));
            return;
        }

        if (_persistenceProvider is null)
        {
            return;
        }

        try
        {
            await _persistenceProvider.FlushAsync(this, genEntityToFlush, eScope, cancellationToken).ConfigureAwait(false);
            xDirty = false;
            DirtyStateChanged?.Invoke(this, new DirtyStateChangedEventArgs(false, genEntityToFlush, "The genealogy was flushed successfully."));
            Flushed?.Invoke(this, new FlushCompletedEventArgs(genEntityToFlush, eScope));
        }
        catch (Exception exException)
        {
            FlushFailed?.Invoke(this, new FlushFailedEventArgs(genEntityToFlush, eScope, exException));
            throw;
        }
    }

    public void Dispose()
    {
        _messanger.UnregisterAll(this);
        GC.SuppressFinalize(this);
    }

    #endregion
}
