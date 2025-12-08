using System.Collections.Generic;
using TranspilerLib.Data;

namespace TranspilerLib.Interfaces.Code
{
    /// <summary>
    /// Represents the mutable state used by an <see cref="ICodeBuilder"/> while constructing an <see cref="ICodeBlock"/> tree.
    /// </summary>
    public interface ICodeBuilderData
    {
        /// <summary>
        /// Gets or sets the current block being appended to or modified.
        /// </summary>
        ICodeBlock actualBlock { get; set; }
        /// <summary>
        /// Gets or sets the map of encountered label identifiers to their corresponding blocks.
        /// </summary>
        Dictionary<string, ICodeBlock> labels { get; set; }
        /// <summary>
        /// Gets or sets the list of encountered goto statements for later target resolution.
        /// </summary>
        List<ICodeBlock> gotos { get; set; }
        /// <summary>
        /// Gets or sets a flag used by builders to detect statement boundaries (e.g., after breaks or strings).
        /// </summary>
        bool xBreak { get; set; }
        /// <summary>
        /// Gets or sets the last processed <see cref="CodeBlockType"/> to aid context-sensitive decisions.
        /// </summary>
        CodeBlockType cbtLast { get; set; }
    }
}