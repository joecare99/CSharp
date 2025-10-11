using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.Models;

/// <summary>
/// Defines the severity or kind of diagnostic messages that can be emitted by components in this library.
/// </summary>
public enum EMessageType
{
    /// <summary>
    /// A fatal error indicating that processing cannot continue.
    /// </summary>
    mtFatal,
    /// <summary>
    /// A non-recoverable error condition for a particular operation.
    /// </summary>
    mtError,
    /// <summary>
    /// A recoverable issue that may affect output or behavior.
    /// </summary>
    mtWarning,
    /// <summary>
    /// An informational note providing additional context.
    /// </summary>
    mtNote,
    /// <summary>
    /// A suggestion or hint to improve readability or performance.
    /// </summary>
    mtHint,
    /// <summary>
    /// General informational message.
    /// </summary>
    mtInfo,
    /// <summary>
    /// Debug-only message useful during development.
    /// </summary>
    mtDebug
}
