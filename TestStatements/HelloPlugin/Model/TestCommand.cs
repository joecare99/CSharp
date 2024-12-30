using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloPlugin.Properties;
using Microsoft.Extensions.Logging;
using PluginBase.Interfaces;

namespace HelloPlugin.Model;

public class TestCommand : ICommand
{
    private IEnvironment? _env;
    private IRandom? _rnd;
    private ILogger? _logger;
    private ISysTime? _time;

    public string Name { get => "test"; }
    public string Description { get => Resources.testDescription; }
    public void Initialize(IEnvironment env)
    {
        _env = env;
        _rnd = _env.GetService<IRandom>();
        _logger = _env.GetService<ILogger>();
        _time = _env.GetService<ISysTime>();
    }

    public int Execute()
    {
        _logger?.LogDebug($"{nameof(TestCommand)}.{nameof(Execute)}");
        if (_env != null)
           _env.ui.Title = Resources.msgTest;
        _env?.ui.ShowMessage(Resources.msgTest);
        _env?.ui.ShowMessage(string.Format(Resources.msgRandom,[_rnd?.Next()]));
        _env?.ui.ShowMessage(string.Format(Resources.msgCurrentTime,[_time?.Now]));
        return 0;
    }
}

