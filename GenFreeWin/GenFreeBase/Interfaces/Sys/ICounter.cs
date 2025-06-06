﻿using GenFree.Interfaces.Model;
using BaseLib.Interfaces;

namespace GenFree.Interfaces.Sys
{
    public interface ICounter<T> : IHasValue<T>
    {
        void Reset();
        ICounter<T> Inc();
        ICounter<T> Dec();
    }

    public interface ICounter : ICounter<int>
    {
    }
}