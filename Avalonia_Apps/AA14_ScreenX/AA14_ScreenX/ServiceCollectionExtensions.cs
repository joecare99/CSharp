using Microsoft.Extensions.DependencyInjection;
using ScreenX.Base;

namespace AA14_ScreenX;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScreenX(this IServiceCollection services)
    {
        services.AddSingleton<IRendererService, RendererService>();
        return services;
    }
}
