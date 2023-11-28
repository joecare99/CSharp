using BaseLib.Helper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Helper.MVVM
{
    public class CRandom :IRandom
    {
        private Random _random;

        public CRandom() 
        { 
            _random = new Random();
        }

        public int Next(int v1, int v2) => _random.Next(v1, v2);

        public double NextDouble() => _random.NextDouble();

        public int NextInt() => _random.Next();

        public void Seed(int value) => _random = new Random(value);
    }
}
