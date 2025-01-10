using System;

namespace BaseLib.Interfaces;

public interface ISysTime
{
    DateTime Now { get; }
    DateTime Today { get; }
}
