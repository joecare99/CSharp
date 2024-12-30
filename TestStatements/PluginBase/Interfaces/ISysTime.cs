using System;

namespace PluginBase.Interfaces;

public interface ISysTime
{
    DateTime Now { get; }
}