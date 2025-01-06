using GenFree.Interfaces.Sys;
using System;

namespace GenFree.Sys
{
    public class CSysTime : ISysTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime Default => default;

    }
}
