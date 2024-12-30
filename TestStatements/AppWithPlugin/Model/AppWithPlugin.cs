using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginBase.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppWithPlugin.Model;

public class AppWithPlugin : IEnvironment, IUserInterface
{
    static Assembly LoadPlugin(string relativePath)
    {
        // Navigate up to the solution root
        string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(typeof(Program).Assembly.Location))))));

        string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar),"Debug", "net6.0"));
        Console.WriteLine($"Loading commands from: {pluginLocation}");
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyPath(Path.Combine(pluginLocation,relativePath)+".dll");
    }

    static IEnumerable<ICommand> CreateCommands(Assembly assembly,IEnvironment env)
    {
        int count = 0;

        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(ICommand).IsAssignableFrom(type))
            {
                ICommand result = Activator.CreateInstance(type) as ICommand;
                if (result != null)
                {
                    result.Initialize(env);
                    count++;
                    yield return result;
                }
            }
        }

        if (count == 0)
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }
    }

    IEnumerable<ICommand>? commands;
    private IMessenger? _messanger;
    private IServiceProvider? _sp;

    public IData data { get => throw new NotImplementedException(); }
    public IUserInterface ui { get => this; }
    public IMessenger messaging { get => _messanger ?? new WeakReferenceMessenger(); }
    public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Initialize(string[] args)
    {
        _sp = new ServiceCollection()
            .AddTransient<IRandom, Model.Random>()
            .AddTransient<ISysTime, Model.SysTime>()
            .AddSingleton<ILogger, Logging>()
            .BuildServiceProvider();


        string[] pluginPaths =
            [
                "HelloPlugin"
            ];

        commands = pluginPaths.SelectMany(pluginPath =>
    {
        Assembly pluginAssembly = LoadPlugin(pluginPath);
        return CreateCommands(pluginAssembly,this);
    }).ToList();
        Console.WriteLine("AppWithPlugin is initialized.");
    }

    public void Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && args[0] == "/d")
            {
                Console.WriteLine("Waiting for any key...");
                Console.ReadLine();
            }


            if (args.Length == 0)
            {
                Console.WriteLine("Commands: ");
                foreach (ICommand command in commands)
                {
                    Console.WriteLine($"{command.Name}\t - {command.Description}");
                }
            }
            else
            {
                foreach (string commandName in args)
                {
                    Console.WriteLine($"-- {commandName} --");

                    ICommand command = commands.FirstOrDefault(c => c.Name.ToUpper() == commandName.ToUpper());
                    if (command == null)
                    {
                        Console.WriteLine("No such command is known.");
                        return;
                    }

                    command.Execute();

                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public T? GetService<T>()
    {
        return _sp.GetService<T>();
    }

    public bool AddService<T>(T service)
    {
        throw new NotImplementedException();
    }

    public bool ShowMessage(string message)
    {
        Console.WriteLine(message);
        return true;
    }
}
