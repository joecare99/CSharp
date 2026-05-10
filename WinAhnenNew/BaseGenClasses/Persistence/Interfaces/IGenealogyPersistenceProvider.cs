using System;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence.Interfaces;

/// <summary>
/// Compatibility alias for the central genealogy persistence contract.
/// </summary>
[Obsolete("Use GenInterfaces.Interfaces.Genealogic.IGenPersistence instead.")]
public interface IGenealogyPersistenceProvider : IGenPersistence
{
}
