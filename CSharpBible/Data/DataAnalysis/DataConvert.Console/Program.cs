namespace DataConvert.Console;
internal class Program
{
    static async Task Main(string[] args)
    {
        var app = new App();
        await app.RunAsync(args);
    }
}
