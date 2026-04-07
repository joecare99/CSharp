using System.Collections.Generic;
using GenInterfaces.Interfaces.Genealogic;

namespace WinAhnenNew.Services
{
    /// <summary>
    /// Provides person data for the selection page.
    /// </summary>
    public interface IPersonSelectionService
    {
        /// <summary>
        /// Gets the available persons that can be selected for editing.
        /// </summary>
        /// <returns>A read-only list of selectable genealogy persons.</returns>
        IReadOnlyList<IGenPerson> GetSelectablePersons();

        /// <summary>
        /// Creates and persists a demo genealogy with the requested number of connected persons.
        /// </summary>
        /// <param name="iPersonCount">The number of persons to generate.</param>
        void CreateDemoGenealogy(int iPersonCount);
    }
}
