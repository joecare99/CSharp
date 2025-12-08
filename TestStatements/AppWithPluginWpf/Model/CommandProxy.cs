using PluginBase.Interfaces;
using System;

namespace AppWithPluginWpf.Model;

public class CommandProxy(string name,string desc, Func<object?, int> exec, Action<IEnvironment>? init=null) : ICommand 
{
    public string Name => name;

    public string Description => desc;

    public int Execute(object? param = null) => exec(param);

    public void Initialize(IEnvironment env) => init?.Invoke(env);
}