namespace TranspilerLib.Interfaces.Code
{
    /// <summary>
    /// Provides optimization hooks for post-parse code structure clean-up and control-flow reconstruction.
    /// </summary>
    public interface ICodeOptimizer
    {
        /// <summary>
        /// Gets or sets the flag to disable while/do-while reconstruction. When <c>true</c>, such rewrites are suppressed.
        /// </summary>
        bool _noWhile { get; set; }

        /// <summary>
        /// Analyzes and potentially rewrites the supplied <paramref name="item"/> to simplify the block graph.
        /// </summary>
        /// <param name="item">The code block candidate to test and optimize.</param>
        void TestItem(ICodeBlock item);

        /// <summary>
        /// Simplifies deterministic conditional chains in the supplied code tree.
        /// </summary>
        /// <param name="root">The root block whose conditional statements are inspected.</param>
        void OptimizeIfChains(ICodeBlock root);

    }
}