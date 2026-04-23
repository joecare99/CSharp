using GenFree.Interfaces.Sys;
using System;

namespace GenFree.Sys
{
    public class CSysTime : BaseLib.Models.SysTime, ISysTime
    {
        public int TodayInt => Today.Year * 10000 + Today.Month * 100 + Today.Day;
    }
}
