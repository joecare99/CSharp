namespace PluginBase.Interfaces;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    void Initialize(IEnvironment env);
    int Execute();
}

