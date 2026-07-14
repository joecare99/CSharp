using GenFreeWin.Services.Models;
using GenFree.Data;

namespace GenFreeWin.Services.Interfaces
{
    /// <summary>
    /// Composes document structure fragments for NamenSuch output independently of rendering.
    /// </summary>
    public interface INamenSuchDocumentComposer
    {
        /// <summary>
        /// Composes a heading definition for the Berufe output block.
        /// </summary>
        /// <param name="eventArt">The event type being rendered.</param>
        /// <param name="eventCount">The amount of matching events.</param>
        /// <param name="occupationText">Localized text for occupation heading.</param>
        /// <param name="titleText">Localized text for title heading.</param>
        /// <param name="text70">Localized text for event 301 heading.</param>
        /// <param name="text444">Localized singular text for event 302 heading.</param>
        /// <param name="text445">Localized plural text for event 302 heading.</param>
        /// <returns>A heading definition or <c>null</c> if no heading applies.</returns>
        NamenSuchHeadingDefinition ComposeBerufeHeading(
            EEventArt eventArt,
            int eventCount,
            string occupationText,
            string titleText,
            string text70,
            string text444,
            string text445);

        /// <summary>
        /// Composes a Heidat event definition for prefix rendering and witness labeling.
        /// </summary>
        /// <param name="eventArt">The event type being rendered.</param>
        /// <param name="emitIndentedOutput">Value indicating whether output should start on a new indented line.</param>
        /// <param name="eventText">Localized event text.</param>
        /// <returns>A composed Heidat event definition.</returns>
        NamenSuchHeidatEventDefinition ComposeHeidatEvent(
            EEventArt eventArt,
            bool emitIndentedOutput,
            string eventText);
    }
}
