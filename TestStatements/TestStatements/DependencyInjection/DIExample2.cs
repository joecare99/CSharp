using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.DependencyInjection;

public class DIExample2
{

    class testClass1(IMessageWriter messageWriter)
    {
        IMessageWriter mw { get; } = messageWriter;

        public void Write(string msg)
        {
            mw.Write(msg);
        }
    }

    public static ServiceProvider container { get; private set; }
    

    public static void TestDependencyInjection()
    {
        var sc = new ServiceCollection()
            .AddSingleton<IMessageWriter, MessageWriter>()
            .AddSingleton<ILogger,Logger<object>>()
            .AddTransient<Worker>()
            .AddTransient<testClass1>();

        container = sc.BuildServiceProvider();

        
        var worker = container.GetRequiredService<Worker>();

        var test = container.GetRequiredService<testClass1>();

        test.Write("Hello World");
    }
}
