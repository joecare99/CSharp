using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces.Sys
{
    public interface ISysTime
    {
        DateTime Now { get; }
    }
}
