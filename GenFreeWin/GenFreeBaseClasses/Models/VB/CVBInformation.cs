using GenFree.Interfaces.VB;
using Microsoft.VisualBasic;
using System;

namespace GenFree;

public class CVBInformation : IVBInformation
{
    public (int Number, string Description, string Source, Action<int> Raise) Err()
    {
        return (Information.Err().Number, 
                Information.Err().Description, 
                Information.Err().Source, 
                (int v) => { Information.Err().Raise(v); });
    }
}
