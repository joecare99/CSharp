using System;
using System.Collections.Generic;

namespace RnzTrauer.Core.Services.Interfaces;

/// <summary>
/// Represents an abstract database query used by the repository layer.
/// </summary>
public interface IDBQuery : IDisposable, System.Data.IDbCommand
{    
    /// <summary>
    /// Adds a named parameter to the query.
    /// </summary>
    void AddParameter(string sParameterName, object? xValue);

    /// <summary>
    /// Executes the query and returns rows as dictionaries.
    /// </summary>
    List<Dictionary<string, object?>> Execute();

    /// <summary>
    /// Executes the query and returns the first two columns as a string-long index.
    /// </summary>
    Dictionary<string, long> ExecuteIndex();
}
