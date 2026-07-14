using System;

namespace GenFree.Interfaces.VB;

public interface IProjectData
{
    void ClearProjectError();
    Exception CreateProjectError(int v);
    void EndApp();
    void SetProjectError(Exception ex, int erl = 0);
}
