using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_22_WpfCap.Model
{
    /// <summary>A class abstracting the Random function</summary>
    public class CRandom : IRandom
    {
        private Random _rnd;

        public CRandom() {
            _rnd = new Random();
        }

        /// <summary>Returns the <strong>next</strong> random number with the specified maximum (excl.).</summary>
        /// <param name="max">The maximum.</param>
        /// <returns>The random value</returns>
        public int Next(int max) => _rnd.Next(max);

        public void Seed(int seed)
        {
            _rnd = new Random(seed);
        }
    }
}
