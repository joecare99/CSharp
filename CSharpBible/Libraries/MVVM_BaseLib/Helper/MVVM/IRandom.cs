using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Helper.MVVM
{
    public interface IRandom
    {
        int Next(int v1, int v2);
        double NextDouble();
        int NextInt();
        void Seed(int value);
    }
}
