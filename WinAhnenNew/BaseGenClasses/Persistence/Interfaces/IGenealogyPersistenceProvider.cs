using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Persistence.Interfaces;

/// <summary>
/// Compatibility alias for the central genealogy persistence contract.
/// </summary>
[Obsolete("Use GenInterfaces.Interfaces.Genealogic.IGenPersistence instead.")]
public interface IGenealogyPersistenceProvider : IGenPersistence
{
}
