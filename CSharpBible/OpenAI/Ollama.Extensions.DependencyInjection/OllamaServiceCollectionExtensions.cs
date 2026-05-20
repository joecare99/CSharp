using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Ollama.Extensions.DependencyInjection;

/// <summary>
/// Provides dependency injection registration helpers for the Ollama client layer.
/// </summary>
public static class OllamaServiceCollectionExtensions
{
    /// <summary>
    /// Registers the root Ollama client as a singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="endpoint">The Ollama endpoint.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOllamaClient(this IServiceCollection services, Uri endpoint)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(endpoint);

        services.AddSingleton(new Ollama.Client.OllamaClientOptions(endpoint));
        services.AddSingleton<HttpClient>();
        services.AddSingleton<Ollama.Client.OllamaClient>(serviceProvider =>
        {
            HttpClient httpClient = serviceProvider.GetRequiredService<HttpClient>();
            Ollama.Client.OllamaClientOptions options = serviceProvider.GetRequiredService<Ollama.Client.OllamaClientOptions>();
            return new Ollama.Client.OllamaClient(httpClient, options);
        });

        return services;
    }

    /// <summary>
    /// Registers a model-scoped chat client as a singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="model">The model name.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOllamaChatClient(this IServiceCollection services, string model)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<Ollama.Client.OllamaClient>().GetChatClient(model));

        return services;
    }

    /// <summary>
    /// Registers a model-scoped generate client as a singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="model">The model name.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOllamaGenerateClient(this IServiceCollection services, string model)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<Ollama.Client.OllamaClient>().GetGenerateClient(model));

        return services;
    }

    /// <summary>
    /// Registers a model-scoped embedding client as a singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="model">The model name.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOllamaEmbeddingClient(this IServiceCollection services, string model)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<Ollama.Client.OllamaClient>().GetEmbeddingClient(model));

        return services;
    }
}
