using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestStatements.Diagnostics
{
    public static class DebugExample
    {

        public static void Main()
        { 
                DebugWriteExample("Some Debugging");
                DebugDivideExample(10, 5);
//                NormalDivideExample(10, 0);
                DebugDivideExample(10, 0);         
        }

        public static int DebugDivideExample(int v1, int v2)
        {
            if (Debugger.IsAttached)
              Debug.Assert(v2 != 0,"Divisor must not be zero");
            if (v2 == 0) return 0;
            return v1 / v2;
        }
        public static int NormalDivideExample(int v1, int v2)
        {
            return v1 / v2;
        }

        public static void DebugWriteExample(string v)
        {
            Debug.Write(v);
        }
    }
}
