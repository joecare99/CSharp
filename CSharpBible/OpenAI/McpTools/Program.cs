using System;
using System.IO;
using System.Threading.Tasks;
using McpTools;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    internal static Func<WebApplication, Task> ApplicationRunner { get; set; } = static application => application.RunAsync();

    internal static async Task Main(string[] args)
    {
        await using WebApplication application = CreateApplication(args);
        await ApplicationRunner(application);
    }

    internal static Task RunApplicationAsync(WebApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return application.RunAsync();
    }

    internal static WebApplication CreateApplication(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton(ResolveMcpToolsOptions(builder.Configuration));
        builder.Services.AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        WebApplication application = builder.Build();
        application.MapMcp();
        return application;
    }

    internal static McpToolsOptions ResolveMcpToolsOptions(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        McpToolsOptions options = new()
        {
            SourceDependenciesScript = FirstNonWhiteSpace(
                configuration["McpTools:SourceDependenciesScript"],
                Environment.GetEnvironmentVariable("MCP_SOURCE_DEPENDENCIES_SCRIPT")),
            TestCoverageScript = FirstNonWhiteSpace(
                configuration["McpTools:TestCoverageScript"],
                Environment.GetEnvironmentVariable("MCP_TEST_COVERAGE_SCRIPT")),
        };

        string? timeoutMinutes = FirstNonWhiteSpace(
            configuration["McpTools:ExecutionTimeoutMinutes"],
            Environment.GetEnvironmentVariable("MCP_EXECUTION_TIMEOUT_MINUTES"));
        if (timeoutMinutes is not null && int.TryParse(timeoutMinutes, out int parsedTimeout) && parsedTimeout > 0)
        {
            options.ExecutionTimeoutMinutes = parsedTimeout;
        }

        return options;
    }

    private static string? FirstNonWhiteSpace(string? first, string? second)
        => string.IsNullOrWhiteSpace(first) ? (string.IsNullOrWhiteSpace(second) ? null : second) : first;
}
