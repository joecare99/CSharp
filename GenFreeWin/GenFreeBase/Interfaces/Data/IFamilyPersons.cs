﻿using System.Collections.Generic;

namespace GenFree.Interfaces.Data;

public interface IFamilyPersons
{
    IArrayProxy<string> KiAText { get; }
    int Frau { get; set; }
    int Mann { get; set; }
    IList<(int nr, string aTxt)> Kinder { get; }
    IArrayProxy<int> Kind { get; }
    int ID { get; }

    void Clear();
}