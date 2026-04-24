using System.Reflection;

namespace TranspilerLib.Models;

/// <summary>
/// Provides file scanning related types and callbacks used by the transpiler to process resources and warnings.
/// </summary>
public class CFileScanner 
{
    /// <summary>
    /// Represents a callback that handles an individual resource file discovered by the scanner.
    /// </summary>
    /// <param name="Sender">The sender that triggered the callback, typically the scanner instance.</param>
    /// <param name="aFileName">The full path or name of the resource file being handled.</param>
    /// <param name="aOptions">Additional options or parameters associated with the resource handling.</param>
    public delegate void TResourceHandler(object? Sender, string aFileName, TStrings aOptions);

    /// <summary>
    /// Describes a mapping between a file extension and a corresponding <see cref="TResourceHandler"/>.
    /// </summary>
    public struct TResourceHandlerRecord
    { 
        string Ext;
        TResourceHandler Handler;
    }

    /// <summary>
    /// Represents a warning-number specific state used while scanning files.
    /// </summary>
    public struct TWarnMsgNumberState
    {
        int Number;
      //  TWarnMsgState State;
    }
    

}