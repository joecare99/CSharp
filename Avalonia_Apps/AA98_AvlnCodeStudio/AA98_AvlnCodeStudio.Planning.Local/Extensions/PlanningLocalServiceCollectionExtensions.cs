using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.Planning.Local.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AA98_AvlnCodeStudio.Planning.Local.Extensions;

/// <summary>
/// Registers the markdown-backed local planning driver.
/// </summary>
public static class PlanningLocalServiceCollectionExtensions
{
    public static IServiceCollection AddLocalPlanning(this IServiceCollection services)
    {
        services.AddSingleton<IPlanningProvider, LocalPlanningProvider>();
        services.AddSingleton<ILocalPlanningProjectScaffolder, LocalPlanningProjectScaffolder>();
        return services;
    }
}
