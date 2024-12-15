using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Interfaces;

public interface IHasValue
{
    object? Value { get; }
}
