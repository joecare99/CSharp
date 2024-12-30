
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace TestStatements.Threading.Tasks.Tests;

public class MyPing(string[] asKnownHosts) : IPing
{
    // from StackOverflow
    public static PingReply CreateReply(IPStatus ipStatus, long rtt = 0, byte[]? buffer = null)
    {
        IPAddress address = IPAddress.Loopback;
        var args = new object?[5] { address, null, ipStatus, rtt, buffer };
        var types = new Type[5] { typeof(IPAddress), typeof(PingOptions), typeof(IPStatus), typeof(long), typeof(byte[]) };
        var conInfo = typeof(PingReply).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, types, null);
        var newObj = conInfo?.Invoke(args);
        return newObj is PingReply pr ? pr : throw new Exception("failure to create PingReply");
    }
    public PingReply Send(string hostNameOrAddress)
    {
        if (asKnownHosts.Contains(hostNameOrAddress))
        {
            return CreateReply(IPStatus.Success);
        }
        else
        {
            return CreateReply(IPStatus.TimedOut);
        }
    }
}