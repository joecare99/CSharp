using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_38_CTDependencyInjection.Models
{
    public class CSysTime : ISysTime
    {
        public static Func<DateTime> GetNow = () => DateTime.Now;
        public DateTime Now => GetNow();
    }
}
