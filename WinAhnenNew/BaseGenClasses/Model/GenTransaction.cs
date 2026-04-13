using BaseGenClasses.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Model;

/// <summary>
/// Represents a genealogy journal transaction.
/// </summary>
[JsonDerivedType(typeof(GenTransaction), typeDiscriminator: nameof(GenTransaction))]
public sealed class GenTransaction : GenObject, IGenTransaction, IHasOwner
{
    private object? _owner;
    private IGenTransaction? _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenTransaction"/> class.
    /// </summary>
    public GenTransaction()
    {
        Items = new IndexedList<object, string>(static objItem => objItem.GetHashCode().ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public override EGenType eGenType => EGenType.Genealogy;

    /// <inheritdoc />
    public IGenBase Class { get; init; } = null!;

    /// <inheritdoc />
    public IGenBase Entry { get; init; } = null!;

    /// <inheritdoc />
    public object? Data { get; init; }

    /// <inheritdoc />
    public object? OldData { get; init; }

    /// <inheritdoc />
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <inheritdoc />
    public IGenTransaction? Prev { get; init; }

    /// <inheritdoc />
    [JsonIgnore]
    public IIndexedList<object> Items { get; }

    /// <inheritdoc />
    [JsonIgnore]
    public IGenTransaction Next => _next ?? this;

    /// <inheritdoc />
    [JsonIgnore]
    public IGenTransaction First => Prev?.First ?? this;

    /// <inheritdoc />
    [JsonIgnore]
    public IGenTransaction Last => _next?.Last ?? this;

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsFirst => Prev is null;

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsLast => _next is null;

    /// <inheritdoc />
    [JsonIgnore]
    public object? Owner => _owner;

    /// <summary>
    /// Links the current transaction to a subsequent one.
    /// </summary>
    /// <param name="genNext">The next transaction.</param>
    public void SetNext(IGenTransaction? genNext)
    {
        _next = genNext;
    }

    /// <inheritdoc />
    public void SetOwner(object tOwner)
    {
        _owner = tOwner;
    }
}
