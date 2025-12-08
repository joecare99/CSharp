using System;

namespace BaseLib.Models.Interfaces;

public interface ISysTime
{
    DateTime Now { get; }
    DateTime Today { get; }
}
