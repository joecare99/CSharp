using Microsoft.Extensions.DependencyInjection;
using Workbench.Builder.Host;

ServiceProvider serviceProvider = ServiceRegistration.CreateServiceProvider();
HostApplication application = ActivatorUtilities.CreateInstance<HostApplication>(serviceProvider);
int exitCode = await application.RunAsync(args).ConfigureAwait(false);
return exitCode;
