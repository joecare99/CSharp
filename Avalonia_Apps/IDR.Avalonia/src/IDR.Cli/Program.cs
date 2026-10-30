using IDR.Core.Services;
using IDR.Infrastructure.Analysis;
using IDR.Infrastructure.Disassembly;
using IDR.Infrastructure.KnowledgeBase;
using IDR.Infrastructure.Pe;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Cli;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        using CancellationTokenSource cancellation = new();
        ConsoleCancelEventHandler handler = (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };
        Console.CancelKeyPress += handler;
        try
        {
            AnalysisRunner runner = new(
                new PeImageLoader(),
                new DelphiVersionDetector(),
                new KnowledgeBaseReader(),
                new BasicAnalysisService(new IcedInstructionDecoder()));
            return await new CommandLineHost(runner)
                .RunAsync(args, cancellation.Token)
                .ConfigureAwait(false);
        }
        finally
        {
            Console.CancelKeyPress -= handler;
        }
    }
}
