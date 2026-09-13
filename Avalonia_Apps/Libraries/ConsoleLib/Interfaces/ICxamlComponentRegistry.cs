using System;
using ConsoleLib;

namespace ConsoleLib.Interfaces;

public interface ICxamlComponentRegistry
{
    void Register(string xmlName, Func<IServiceProvider, CxamlLoadContext, IControl> factory);
    bool TryCreate(string xmlName, CxamlLoadContext context, out IControl control);
}
