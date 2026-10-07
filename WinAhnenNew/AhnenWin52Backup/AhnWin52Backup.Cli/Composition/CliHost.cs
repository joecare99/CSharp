using System;
using System.IO;
using System.ComponentModel;
using AhnWin52Backup.Cli.Abstractions;
using AhnWin52Backup.Cli.Commands;
using AhnWin52Backup.Cli.Credentials;
using AhnWin52Backup.Cli.Infrastructure;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Cli.Composition;

internal static class CliHost
{
    public static int Run(string[] arguments)
    {
        LightweightServiceCollection services = new();
        services.AddSingleton<IConsoleAdapter>(static _ => new SystemConsoleAdapter());
        services.AddSingleton<IPasswordCredentialStore>(static _ => new WindowsCredentialStore());
        services.AddSingleton<HejCodec>(static _ => new HejCodec());
        services.AddSingleton<IHejReader>(static provider => provider.GetRequiredService<HejCodec>());
        services.AddSingleton<IHejWriter>(static provider => provider.GetRequiredService<HejCodec>());
        services.AddSingleton<IHejInspectionService>(
            static provider => new HejInspectionService(provider.GetRequiredService<IHejReader>()));
        services.AddSingleton<IParadoxTableReader>(static _ => new ParadoxTableReader());
        services.AddSingleton<IHejDatabaseExportService>(
            static provider => new HejDatabaseExportService(provider.GetRequiredService<IParadoxTableReader>()));
        services.AddSingleton<StructureTemplateMaterializer>(static _ => new StructureTemplateMaterializer());
        services.AddSingleton<IHejDatabaseRestoreService>(
            static provider => new HejDatabaseRestoreService(
                provider.GetRequiredService<IHejReader>(),
                provider.GetRequiredService<IHejDatabaseExportService>(),
                provider.GetRequiredService<IParadoxTableReader>(),
                provider.GetRequiredService<StructureTemplateMaterializer>()));
        services.AddSingleton<CommandDispatcher>(
            static provider => new CommandDispatcher(
                provider.GetRequiredService<IHejInspectionService>(),
                provider.GetRequiredService<IHejDatabaseExportService>(),
                provider.GetRequiredService<IHejDatabaseRestoreService>(),
                provider.GetRequiredService<IHejWriter>(),
                provider.GetRequiredService<IParadoxTableReader>(),
                provider.GetRequiredService<StructureTemplateMaterializer>(),
                provider.GetRequiredService<IConsoleAdapter>(),
                provider.GetRequiredService<IPasswordCredentialStore>()));

        IServiceProvider provider = services.BuildServiceProvider();
        try
        {
            return provider.GetRequiredService<CommandDispatcher>().Run(arguments);
        }
        catch (HejFormatException exception)
        {
            provider.GetRequiredService<IConsoleAdapter>().WriteErrorLine($"Invalid HEJ file: {exception.Message}");
            return 1;
        }
        catch (IOException exception)
        {
            provider.GetRequiredService<IConsoleAdapter>().WriteErrorLine($"File operation failed: {exception.Message}");
            return 1;
        }
        catch (UnauthorizedAccessException exception)
        {
            provider.GetRequiredService<IConsoleAdapter>().WriteErrorLine($"Access denied: {exception.Message}");
            return 1;
        }
        catch (ArgumentException exception)
        {
            provider.GetRequiredService<IConsoleAdapter>().WriteErrorLine(exception.Message);
            return 2;
        }
        catch (NotSupportedException exception)
        {
            provider.GetRequiredService<IConsoleAdapter>().WriteErrorLine($"Unsupported Paradox data: {exception.Message}");
            return 1;
        }
        catch (Win32Exception exception)
        {
            provider.GetRequiredService<IConsoleAdapter>().WriteErrorLine($"Windows Credential Manager operation failed: {exception.Message}");
            return 1;
        }
        catch (OperationCanceledException exception)
        {
            provider.GetRequiredService<IConsoleAdapter>().WriteErrorLine(exception.Message);
            return 130;
        }
    }
}
