using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Filter.CSV.Filters;
using TraceAnalysis.Filter.MovirunText.Filters;
using TraceAnalysis.Filter.MovirunTrace.Filters;

namespace TraceCsv2realCsv
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return Run(args, Console.Out, CultureInfo.CurrentUICulture);
        }

        public static int Run(string[] args, TextWriter output, CultureInfo uiCulture)
        {
            using var serviceProvider = CreateServiceProvider();
            var conversionService = serviceProvider.GetRequiredService<TraceCsvConversionService>();
            var helpTextProvider = serviceProvider.GetRequiredService<TraceCsv2realCsvHelpTextProvider>();

            if (args.Length < 1 || args.Length > 2)
            {
                output.Write(helpTextProvider.BuildHelpText(conversionService, uiCulture));
                return 1;
            }

            if (!File.Exists(args[0]))
                return 1;

            var sOutputPath = args.Length >= 2
                ? args[1]
                : conversionService.GetDefaultOutputPath(args[0]);

            if (File.Exists(sOutputPath))
                return 1;

            conversionService.ConvertFile(args[0], sOutputPath);
            return 0;
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IInputFilterSelector, InputFilterSelector>();
            services.AddSingleton<IAnalyzableInputFilter, TraceCsvInputFilter>();
            services.AddSingleton<IAnalyzableInputFilter, FlatCsvInputFilter>();
            services.AddSingleton<IAnalyzableInputFilter, MovirunTextTraceInputFilter>();
            services.AddSingleton<IAnalyzableInputFilter, MovirunTraceXmlInputFilter>();
            services.AddSingleton<IOutputFilter>(_ => new CsvOutputFilter());
            services.AddSingleton<TraceCsvConversionService>();
            services.AddSingleton<TraceCsv2realCsvHelpTextProvider>();
            return services.BuildServiceProvider();
        }
    }
}