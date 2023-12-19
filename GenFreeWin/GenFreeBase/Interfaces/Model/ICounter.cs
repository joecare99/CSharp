using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace GenFree.Interfaces.Model
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