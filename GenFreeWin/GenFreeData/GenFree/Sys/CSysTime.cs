using GenFree.Interfaces.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Sys
{
    public class CSysTime :ISysTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime Default => default;

    }
}
