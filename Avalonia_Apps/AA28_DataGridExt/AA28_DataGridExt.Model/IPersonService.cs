using System.Collections.Generic;

namespace AA28_DataGridExt.Model;

/// <summary>
/// Provides departments and sample persons for the data grid sample.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Gets the available departments.
    /// </summary>
    Department[] GetDepartments();

    /// <summary>
    /// Gets the next random integer within the provided range.
    /// </summary>
    int GetNext(int minimum, int maximum);

    /// <summary>
    /// Gets the sample persons shown in the grid.
    /// </summary>
    IEnumerable<Person> GetPersons();
}
