using AppWithPluginWpf.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PluginBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AppWithPluginWpf.ViewModels;
public partial class TerminalViewModel: ObservableObject, IUserInterface
{
    [ObservableProperty]
    public partial string Command { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<string> Output { get; private set; }
    [ObservableProperty]
    public partial string Title { get ; set ; }
    [ObservableProperty]
    public partial int SelectedIndex { get; set; }


    public Func<string, bool> DoShowMessage { get; set; } = message => true;

    public TerminalViewModel()
    {
        Command = string.Empty;
        Output = new();
        (App.Current as App).ui = this;
        foreach (var cmd in new List<CommandProxy> {
            new CommandProxy("clear","wipes the terminal", () => { Output.Clear(); return 0; } ),
            new CommandProxy("exit","quit the application", () => { App.Current.Shutdown(); return 0; } ),
            new CommandProxy("help","displays a help-page", () => { Output.Add("This is the help page."); return 0; } ),
            new CommandProxy("settitle","sets the title of the terminal", () => { Output.Clear(); return 0; } ),
        })
        {
            (App.Current as App).commands.Add(cmd);
        }
        Command = "list";
        ExecuteCommand();
    }

    [RelayCommand]
    public void ExecuteCommand()
    {
        try
        {
            // Simulate command execution and output
            if (string.IsNullOrWhiteSpace(Command))
            {
                ShowMessage("Command cannot be empty.");
                Output.Add("Command cannot be empty.");
                return;
            }
            foreach (ICommand cmd in (App.Current as App).commands)
            {
                if (Command.ToLower().StartsWith(cmd.Name) && (Command.Length == cmd.Name.Length || Command[cmd.Name.Length] == ' '))
                {
                    cmd.Execute();
                    Output.Add($"Executed command: {Command}");
                    Command = string.Empty;
                }
            }
            if (!string.IsNullOrWhiteSpace(Command))
            {
                ShowMessage("Command not found.");
                Output.Add("Command not found.");
            }
        }
        catch(Exception ex)
        {
            Output.Add(ex.Message);
        }

    }

    [RelayCommand]
    private void EnterKey(object Key)
    {        
        ExecuteCommand();
    }

    public bool ShowMessage(string message)
    {
        return DoShowMessage?.Invoke(message) ?? false;
    }

    public void WriteLine(string v)
    {
        Output.Add(v);
    }
}
