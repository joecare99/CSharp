using PluginBase.Interfaces;
using System;

namespace AppWithPlugin.Model
{
    public class SysTime : ISysTime
    {
        public DateTime Now => DateTime.Now;
    }
}
