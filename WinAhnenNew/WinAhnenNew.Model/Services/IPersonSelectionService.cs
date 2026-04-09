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
        /// Gets the currently selected person for the edit workflow.
        /// </summary>
        IGenPerson? SelectedPerson { get; }

        /// <summary>
        /// Gets the available persons that can be selected for editing.
        /// </summary>
        /// <returns>A read-only list of selectable genealogy persons.</returns>
        IReadOnlyList<IGenPerson> GetSelectablePersons();

        /// <summary>
        /// Sets the currently selected person for editing.
        /// </summary>
        /// <param name="genSelectedPerson">The selected person, or <see langword="null"/> to clear the selection.</param>
        void SetSelectedPerson(IGenPerson? genSelectedPerson);

        /// <summary>
        /// Persists the currently loaded genealogy including edits to the selected person.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Creates and persists a demo genealogy with the requested number of connected persons.
        /// </summary>
        /// <param name="iPersonCount">The number of persons to generate.</param>
        void CreateDemoGenealogy(int iPersonCount);
    }
}
