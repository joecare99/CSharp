using System;
using System.Collections.Generic;

namespace RnzTrauer.Core.Services.Interfaces;

/// <summary>
/// Represents an abstract database command used by the repository layer.
/// </summary>
public interface IDBCommand : IDisposable, System.Data.IDbCommand
{ 

    /// <summary>
    /// Gets the last inserted identity value.
    /// </summary>
    long LastInsertedId { get; }

    /// <summary>
    /// Adds a named parameter to the command.
    /// </summary>
    void AddParameter(string sParameterName, object? xValue);

   
}
