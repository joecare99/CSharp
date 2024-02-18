using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.DependencyInjection
{
    public class DIExample
    {
        public static void Main(params string[] args)
        {
            HostApplicationBuilder builder = new Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>()
                .AddSingleton<IMessageWriter, MessageWriter>();

            using IHost host = builder.Build();

            host.Run();
        }
    }
}
