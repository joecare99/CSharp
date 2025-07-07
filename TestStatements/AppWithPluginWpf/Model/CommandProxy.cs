using PluginBase.Interfaces;
using System;

namespace AppWithPluginWpf.Model;

public class CommandProxy(string name,string desc, Func<int> exec, Action<IEnvironment>? init=null) : ICommand 
{
    public string Name => name;

    public string Description => desc;

    public int Execute() => exec();

    public void Initialize(IEnvironment env) => init?.Invoke(env);
}