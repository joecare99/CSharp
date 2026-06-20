using Microsoft.Extensions.DependencyInjection;
using Workbench.Builder.Analysis;

ServiceProvider serviceProvider = ServiceRegistration.CreateServiceProvider();
AnalysisApplication application = ActivatorUtilities.CreateInstance<AnalysisApplication>(serviceProvider);
int exitCode = await application.RunAsync(args).ConfigureAwait(false);
return exitCode;
