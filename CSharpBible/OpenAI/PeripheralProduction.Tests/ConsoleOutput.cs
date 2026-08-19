using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PeripheralProduction.Tests;

internal static class ConsoleOutput
{
    private static readonly SemaphoreSlim Gate = new(1, 1);

    public static async Task<(T Result, string Output)> CaptureAsync<T>(Func<Task<T>> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        await Gate.WaitAsync();
        TextWriter original = Console.Out;
        using StringWriter output = new(CultureInfo.InvariantCulture);
        try
        {
            Console.SetOut(output);
            T result = await action();
            return (result, output.ToString());
        }
        finally
        {
            Console.SetOut(original);
            Gate.Release();
        }
    }
}
