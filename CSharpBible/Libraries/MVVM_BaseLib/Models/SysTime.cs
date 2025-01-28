using BaseLib.Models.Interfaces;
using System;

namespace BaseLib.Models;

public class SysTime : ISysTime
{
    public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
    public DateTime Now => GetNow();
    public DateTime Today => GetNow().Date;

}
