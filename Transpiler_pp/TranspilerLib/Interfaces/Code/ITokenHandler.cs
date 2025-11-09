using System;
using TranspilerLib.Models;

namespace TranspilerLib.Interfaces.Code
{
    /// <summary>
    /// Abstraction for a stateful token handler used during lexical analysis.
    /// </summary>
    public interface ITokenHandler
    {
        /// <summary>
        /// Sets the reserved words used to distinguish identifiers from keywords during scanning.
        /// </summary>
        string[] reservedWords { set; }

        /// <summary>
        /// Retrieves a handler delegate for the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The current tokenizer state.</param>
        /// <param name="handler">When this method returns, contains the state handler delegate if found.</param>
        /// <returns><c>true</c> if a handler for the state exists; otherwise, <c>false</c>.</returns>
        bool TryGetValue(int state, out Action<ICodeBase.TokenDelegate?, string, TokenizeData> handler);
    }
}