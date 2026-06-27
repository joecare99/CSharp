using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Testing.DependencyInjection;
using AA98_AvlnCodeStudio.Building.Workbench.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Services.Compilation;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;

namespace AA98_AvlnCodeStudio.Building.Workbench.DependencyInjection;

/// <summary>
/// Provides registration helpers for Workbench.Builder-backed builder services.
/// </summary>
public static class WorkbenchBuilderServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Workbench.Builder-backed Code Studio builder services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddWorkbenchCodeStudioBuilding(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IProjectLoader, MsBuildProjectLoader>();
        services.AddSingleton<IReferenceResolver, ReferenceResolver>();
        services.AddSingleton<ITestProjectDetector, TestProjectDetector>();
        services.AddSingleton<IProjectEmitSupportEvaluator, ProjectEmitSupportEvaluator>();
        services.AddSingleton<IProjectCompilationService, ProjectCompilationService>();
        services.AddSingleton<IProjectInspectionService, ProjectInspectionService>();
        services.AddCodeStudioTesting();
        services.AddSingleton<ICodeStudioBuilderService, WorkbenchCodeStudioBuilderService>();
        return services;
    }
}
