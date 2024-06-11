using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Helper;

public class SysTime : ISysTime
{
    public static Func<DateTime> GetNow {get; set;} = () => DateTime.Now;
    public DateTime Now => GetNow();

    public DateTime Today => GetNow().Date;

}
