using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase.Interfaces;

public interface IEnvironment
{
    IData data { get; }
    IUserInterface ui { get; }
    IMessenger messaging { get; }
    T? GetService<T>();

    bool AddService<T,T2>();
}
