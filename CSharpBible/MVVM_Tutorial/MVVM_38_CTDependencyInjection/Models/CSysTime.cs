using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System;

namespace MVVM_38_CTDependencyInjection.Models
{
    public class CSysTime : ISysTime
    {
        public static Func<DateTime> GetNow = () => DateTime.Now;
        public DateTime Now => GetNow();
    }
}
