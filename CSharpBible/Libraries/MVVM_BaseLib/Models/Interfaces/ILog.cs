using System;

namespace BaseLib.Models.Interfaces;

public interface ILog
{
    public void Log(string message);

    public void Log(string message, Exception exception);
}
