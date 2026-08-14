using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System.Diagnostics;

namespace MVVM_38_CTDependencyInjection.Models;

public class CLog : ILog
{
    public void Log(string message) => Debug.WriteLine(message);
}
