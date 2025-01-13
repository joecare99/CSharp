using BaseLib.Interfaces;
using System;

namespace BaseLib.Helper;

public class SysTime : ISysTime
{
    public static Func<DateTime> GetNow {get; set;} = () => DateTime.Now;
    public DateTime Now => GetNow();
    public DateTime Today => GetNow().Date;

}
