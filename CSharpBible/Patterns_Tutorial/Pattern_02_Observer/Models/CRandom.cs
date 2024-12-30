using System;

namespace Pattern_02_Observer.Models
{
    public class CRandom : IRandom
    {
        private Random _rnd = new Random();
        public int Next(int maxValue)
        {
            return _rnd.Next(maxValue);
        }

        public void Seed(int seed)
        {
            _rnd = new Random(seed);
        }
    }
}
