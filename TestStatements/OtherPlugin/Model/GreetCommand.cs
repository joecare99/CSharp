using System;
using OtherPlugin.Properties;
using PluginBase.Interfaces;

namespace OtherPlugin.Model;

public class GreetCommand : ICommand
{
    private IEnvironment? _env;

    public string Name { get => "greet"; }
    public string Description { get => Resources.greetDescription; }

    public void Initialize(IEnvironment env)
    {
        _env = env;
    }

    public int Execute()
    {
        _env?.ui.ShowMessage(Resources.msgGreet);
        return 1;
    }
}
