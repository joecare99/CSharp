using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.ViewModels.Interfaces;

public interface IProjectData
{
    void ClearProjectError();
    Exception CreateProjectError(int v);
    void EndApp();
    void SetProjectError(Exception ex, int erl=0);
}
