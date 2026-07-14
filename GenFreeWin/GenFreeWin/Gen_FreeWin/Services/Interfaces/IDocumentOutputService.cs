using GenFreeWin.Services.Models;
using GenFree.Interfaces;

namespace GenFreeWin.Services.Interfaces
{
    /// <summary>
    /// Provides document-specific output helpers independent from ViewModel state.
    /// </summary>
    public interface IDocumentOutputService
    {
        /// <summary>
        /// Removes trailing whitespace and line terminators from the document.
        /// </summary>
        /// <param name="document">The target document.</param>
        void Retweg3(IDocument document);

        /// <summary>
        /// Removes trailing whitespace from the document.
        /// </summary>
        /// <param name="document">The target document.</param>
        /// <returns><c>true</c> if trailing whitespace was removed; otherwise <c>false</c>.</returns>
        bool TrimEnd(IDocument document);

        /// <summary>
        /// Renders a composed heading definition into the target document.
        /// </summary>
        /// <param name="document">The target document.</param>
        /// <param name="headingDefinition">The heading definition to render.</param>
        void RenderHeading(IDocument document, NamenSuchHeadingDefinition headingDefinition);

        /// <summary>
        /// Renders a composed Heidat event prefix into the target document.
        /// </summary>
        /// <param name="document">The target document.</param>
        /// <param name="eventDefinition">The composed event definition to render.</param>
        void RenderHeidatEventPrefix(IDocument document, NamenSuchHeidatEventDefinition eventDefinition);
    }
}
