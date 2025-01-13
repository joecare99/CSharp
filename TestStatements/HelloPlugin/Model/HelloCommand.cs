using System;
using HelloPlugin.Properties;
using PluginBase.Interfaces;

namespace HelloPlugin.Model;

public class HelloCommand : ICommand
{
    private IEnvironment? _env;

    public string Name { get => "hello"; }
    public string Description { get => Resources.helloDescription; }

    public void Initialize(IEnvironment env)
    {
        _env = env;
    }

    public int Execute()
    {
        _env?.ui.ShowMessage(Resources.msgHello);
        return 1;
    }
}
