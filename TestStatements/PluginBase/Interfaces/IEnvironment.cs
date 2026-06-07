using CommunityToolkit.Mvvm.Messaging;

namespace PluginBase.Interfaces;

public interface IEnvironment
{
    IData data { get; }
    IUserInterface ui { get; }
    IMessenger messaging { get; }
    T? GetService<T>();

    bool AddService<T,T2>();
}
