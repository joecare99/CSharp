using TranspilerLib.Data;

namespace TranspilerLib.Interfaces.Code
{
    /// <summary>
    /// Builds an <see cref="ICodeBlock"/> tree from a stream of <see cref="TokenData"/>.
    /// </summary>
    public interface ICodeBuilder
    {
        /// <summary>
        /// Creates a new builder state object used to accumulate blocks, labels, and gotos while parsing tokens.
        /// </summary>
        /// <param name="block">The root block the builder should attach newly created nodes to.</param>
        /// <returns>A fresh <see cref="ICodeBuilderData"/> context.</returns>
        ICodeBuilderData NewData(ICodeBlock block);

        /// <summary>
        /// Processes a single token and mutates the provided <paramref name="data"/> accordingly
        /// (creating/moving/emitting blocks and control-flow constructs as needed).
        /// </summary>
        /// <param name="tokenData">The token to handle.</param>
        /// <param name="data">The active builder state.</param>
        void OnToken(TokenData tokenData, ICodeBuilderData data);
    }
}