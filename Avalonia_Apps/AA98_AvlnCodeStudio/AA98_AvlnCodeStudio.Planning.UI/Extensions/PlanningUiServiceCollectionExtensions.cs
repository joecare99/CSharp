using AA98_AvlnCodeStudio.Planning.Local.Extensions;
using AA98_AvlnCodeStudio.Planning.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AA98_AvlnCodeStudio.Planning.UI.Extensions;

/// <summary>
/// Registers reusable planning UI services together with the default local planning driver.
/// </summary>
public static class PlanningUiServiceCollectionExtensions
{
    public static IServiceCollection AddPlanningUi(this IServiceCollection services)
    {
        services.AddLocalPlanning();
        services.AddSingleton<PlanningExplorerViewModel>();
        return services;
    }
}
