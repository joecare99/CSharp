using System;

namespace BaseLib.Interfaces;

public interface ILog
{
    public void Log(string message);

    public void Log(string message, Exception exception);
}
