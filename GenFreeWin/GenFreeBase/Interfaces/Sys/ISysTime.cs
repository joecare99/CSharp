using System;

namespace GenFree.Interfaces.Sys
{
    public interface ISysTime : BaseLib.Models.Interfaces.ISysTime
    {
        int TodayInt { get; }
    }
}
