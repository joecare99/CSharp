namespace DataAnalysis.Core.Models;

public enum SyslogSeverity
{
    Trace = 8,
    Debug = 7,
    Notice = 5,
    Info = 6,
    Warn = 4,
    Error = 3,
    Critical = 2,
    Alert = 1,
    Fatal = 0,
    Unknown = -1
}
