using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace GenFree2Base.Interfaces
{
    /// <summary>
    /// Interface IGenBase
    /// </summary>
    public interface IGenBase
    {
        /// <summary>
        /// Gets the u identifier.
        /// </summary>
        /// <value>The u identifier.</value>
        Guid UId { get; init; }
        /// <summary>
        /// Gets the type of the e gen.
        /// </summary>
        /// <value>The type of the e gen.</value>
        EGenType eGenType { get; init; }
    }
}