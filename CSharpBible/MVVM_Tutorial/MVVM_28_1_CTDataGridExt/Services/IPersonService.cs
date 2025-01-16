using MVVM_28_1_CTDataGridExt.Models;
using System.Collections.Generic;

namespace MVVM_28_1_CTDataGridExt.Services;

public interface IPersonService
{
    Department[] GetDepartments();
    int GetNext(int mn, int mx);
    IEnumerable<Person> GetPersons();
}