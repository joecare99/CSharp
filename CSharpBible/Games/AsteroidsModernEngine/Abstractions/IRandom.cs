using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidsModern.Engine.Abstractions;

public interface IRandom
{
    int Next(int minValue, int maxValue);
    float NextSingle();
}
