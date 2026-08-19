using McpTools;

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
        builder.Services.AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        WebApplication application = builder.Build();
        application.MapMcp();
        return application;
    }
}
