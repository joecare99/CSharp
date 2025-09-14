using System;

namespace GenFree.Interfaces.VB;

public interface IVBInformation
{
    (int Number,string Description,string Source, Action<int> Raise) Err();
    bool IsDBNull(object v);
}