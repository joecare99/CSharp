using System;
using System.Collections.Generic;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomGenealogy : IGenealogy
{
    public Guid UId { get; init; } = Guid.NewGuid();
    public EGenType eGenType => EGenType.Genealogy;
    public Func<IList<object?>, IGenEntity> GetEntity { get; set; } = _ => throw new NotSupportedException("Use the GEDCOM importer to create entities.");
    public Func<IList<object?>, IGenFact> GetFact { get; set; } = _ => throw new NotSupportedException("GEDCOM facts are not materialized by this importer.");
    public Func<IList<object?>, IGenSource> GetSource { get; set; } = _ => throw new NotSupportedException("GEDCOM sources are not materialized by this importer.");
    public Func<IList<object?>, IGenMedia> GetMedia { get; set; } = _ => throw new NotSupportedException("GEDCOM media are not materialized by this importer.");
    public Func<IList<object?>, IGenTransaction> GetTransaction { get; set; } = _ => throw new NotSupportedException("GEDCOM transactions are not supported by this importer.");
    public IList<IGenEntity> Entitys { get; init; } = new List<IGenEntity>();
    public IList<IGenSource> Sources { get; init; } = new List<IGenSource>();
    public IList<IGenRepository> Repositories { get; init; } = new List<IGenRepository>();
    public IList<IGenPlace> Places { get; init; } = new List<IGenPlace>();
    public IList<IGenMedia> Medias { get; init; } = new List<IGenMedia>();
    public IList<IGenTransaction> Transactions { get; init; } = new List<IGenTransaction>();
}
