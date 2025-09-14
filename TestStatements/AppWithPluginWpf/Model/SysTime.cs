using PluginBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWithPlugin.Model
{
    public class SysTime : ISysTime
    {
        public DateTime Now => DateTime.Now;
    }
}
