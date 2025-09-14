using AppWithPlugin.Model;
using AppWithPluginWpf.Model;
using AppWithPluginWpf.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace AppWithPluginWpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IEnvironment
{

    private IMessenger? _messanger;
    private IServiceProvider? _sp;
    private IServiceCollection? _sc;

    public IData data => _sp.GetRequiredService<IData>();

    public IUserInterface ui { get; set; }

    public IMessenger messaging => _sp.GetRequiredService<IMessenger>();

    public List<object> commands { get; private set; }

    public bool AddService<T, T2>()
    {
        try
        {
            _sc = (_sc ?? new ServiceCollection()).AddSingleton(typeof(T), typeof(T2));
            _sp = _sc!.BuildServiceProvider();
            return true;
        }
        catch (Exception ex)
        {
            // Logging
            return false;
        }
    }

    public T? GetService<T>() => _sp.GetService<T>();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Initialize resources or services here if needed

        _sp = (_sc = (_sc ?? new ServiceCollection())
            .AddTransient<IRandom, AppWithPlugin.Model.Random>()
            .AddTransient<ISysTime, SysTime>()
            .AddSingleton<ILogger, Logging>()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<IEnvironment>(this)

            .AddTransient<TerminalViewModel>()
            )
            .BuildServiceProvider();

        IList<string> pluginPaths =
            [
                "HelloPlugin",
            ];

        foreach (string pluginPath in Directory.EnumerateFiles(
            Path.Combine(Path.GetDirectoryName(typeof(App).Assembly.Location), "..", "PlugIns"), "*.dll"))
        {
            pluginPaths.Add(pluginPath);
        }

        commands = pluginPaths.SelectMany(pluginPath =>
        {
            Assembly pluginAssembly = LoadPlugin(pluginPath);
            return CreateCommands(pluginAssembly, this);
        }).ToList();

        commands.Add(new CommandProxy("list", "lists all commands", DoList));
    }

    private int DoList(object? param = null)
    {
        foreach (ICommand command in commands)
        {
            ui.WriteLine($"{command.Name}\t - {command.Description}");
        }
        return 0;
    }

    static IEnumerable<object> CreateCommands(Assembly assembly, IEnvironment env)
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

    static Assembly LoadPlugin(string relativePath)
    {
        string pluginLocation, assemblyPath;
        if (File.Exists(relativePath))
        {
            // Use the current directory
            pluginLocation = Path.GetFullPath(Path.GetDirectoryName(relativePath));
            assemblyPath = Path.GetFullPath(relativePath);
        }
        else
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(App).Assembly.Location))))));

            pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar), "Debug", "net6.0"));
            assemblyPath = Path.Combine(pluginLocation, relativePath) + ".dll";
        }

        Console.WriteLine($"Loading commands from: {pluginLocation}");
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);

        Assembly assembly = loadContext.LoadFromAssemblyPath(assemblyPath);

        if (!assembly?.IsFullyTrusted ?? false)
            return null;

        // Additional check for a valid signature (if applicable)
#if SIGNED_BUILD
        if (!IsAssemblySigned(assembly))
        {
            Console.WriteLine("Assembly is not signed or has an invalid signature.");
            return null;
        }
#endif
        return assembly;
    }
    static bool IsAssemblySigned(Assembly assembly)
    {
        try
        {
            var name = assembly.GetName();
            var publicKey = name.GetPublicKey();
            var hash = assembly.GetHashCode(); // 30015890
            return publicKey != null && publicKey.Length > 0;
        }
        catch
        {
            return false;
        }
    }

}

