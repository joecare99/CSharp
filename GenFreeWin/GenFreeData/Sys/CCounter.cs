﻿using GenFree.Interfaces.Sys;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Sys
{
    public class CCounter<T> : ICounter<T> where T : struct
    {
        static private Func<T, T>? _pred;
        static private Func<T, T>? _succ;

        public CCounter(Func<T, T> Pred, Func<T, T> Succ)
        {
            _pred ??= Pred;
            _succ ??= Succ;
            Reset();
        }
        public T Value { get; private set; }

        public ICounter<T> Dec()
        {
            Value = _pred?.Invoke(Value) ?? Value;
            return this;
        }

        public ICounter<T> Inc()
        {
            Value = _succ?.Invoke(Value) ?? Value;
            return this;
        }

        public void Reset()
        {
            Value = default;
        }
    }


}
