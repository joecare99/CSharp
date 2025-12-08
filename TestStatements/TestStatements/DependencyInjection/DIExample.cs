using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TestStatements.DependencyInjection
{
    public class DIExample
    {
        public static void Main(params string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>()
                .AddSingleton<IMessageWriter, MessageWriter>();

            using IHost host = builder.Build();

            host.Run();
        }
    }
}
