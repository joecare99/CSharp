namespace Workbench.Builder.Core.Models.Projects;

/// <summary>
/// Represents the evaluated project properties that drive inspection, test detection, and later compilation decisions.
/// </summary>
public sealed class ProjectPropertySet
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectPropertySet"/>.
    /// </summary>
    /// <param name="assemblyName">The evaluated assembly name.</param>
    /// <param name="rootNamespace">The evaluated root namespace.</param>
    /// <param name="targetFramework">The evaluated target framework moniker.</param>
    /// <param name="outputType">The evaluated output type.</param>
    /// <param name="langVersion">The evaluated C# language version.</param>
    /// <param name="nullable">The evaluated nullable mode.</param>
    /// <param name="defineConstants">The evaluated preprocessor constants.</param>
    /// <param name="implicitUsings">The evaluated implicit usings mode.</param>
    /// <param name="isPackable">The evaluated packable flag.</param>
    /// <param name="isTestProject">The evaluated test project flag.</param>
    /// <param name="configuration">The evaluated build configuration.</param>
    /// <param name="runtimeIdentifier">The evaluated runtime identifier.</param>
    /// <param name="outputPath">The evaluated output path, when available.</param>
    /// <param name="intermediateOutputPath">The evaluated intermediate output path, when available.</param>
    /// <param name="projectAssetsFile">The evaluated project assets file path, when available.</param>
    public ProjectPropertySet(
        string assemblyName,
        string rootNamespace,
        string targetFramework,
        string? outputType,
        string? langVersion,
        string? nullable,
        string? defineConstants,
        string? implicitUsings,
        string? isPackable,
        string? isTestProject,
        string? configuration,
        string? runtimeIdentifier,
        string? outputPath,
        string? intermediateOutputPath,
        string? projectAssetsFile)
    {
        AssemblyName = assemblyName;
        RootNamespace = rootNamespace;
        TargetFramework = targetFramework;
        OutputType = outputType;
        LangVersion = langVersion;
        Nullable = nullable;
        DefineConstants = defineConstants;
        ImplicitUsings = implicitUsings;
        IsPackable = isPackable;
        IsTestProject = isTestProject;
        Configuration = configuration;
        RuntimeIdentifier = runtimeIdentifier;
        OutputPath = outputPath;
        IntermediateOutputPath = intermediateOutputPath;
        ProjectAssetsFile = projectAssetsFile;
    }

    /// <summary>
    /// Gets the evaluated assembly name.
    /// </summary>
    public string AssemblyName { get; }

    /// <summary>
    /// Gets the evaluated root namespace.
    /// </summary>
    public string RootNamespace { get; }

    /// <summary>
    /// Gets the evaluated target framework moniker.
    /// </summary>
    public string TargetFramework { get; }

    /// <summary>
    /// Gets the evaluated output type.
    /// </summary>
    public string? OutputType { get; }

    /// <summary>
    /// Gets the evaluated C# language version.
    /// </summary>
    public string? LangVersion { get; }

    /// <summary>
    /// Gets the evaluated nullable mode.
    /// </summary>
    public string? Nullable { get; }

    /// <summary>
    /// Gets the evaluated preprocessor constants.
    /// </summary>
    public string? DefineConstants { get; }

    /// <summary>
    /// Gets the evaluated implicit usings mode.
    /// </summary>
    public string? ImplicitUsings { get; }

    /// <summary>
    /// Gets the evaluated packable flag.
    /// </summary>
    public string? IsPackable { get; }

    /// <summary>
    /// Gets the evaluated test project flag.
    /// </summary>
    public string? IsTestProject { get; }

    /// <summary>
    /// Gets the evaluated build configuration.
    /// </summary>
    public string? Configuration { get; }

    /// <summary>
    /// Gets the evaluated runtime identifier.
    /// </summary>
    public string? RuntimeIdentifier { get; }

    /// <summary>
    /// Gets the evaluated output path, when available.
    /// </summary>
    public string? OutputPath { get; }

    /// <summary>
    /// Gets the evaluated intermediate output path, when available.
    /// </summary>
    public string? IntermediateOutputPath { get; }

    /// <summary>
    /// Gets the evaluated project assets file path, when available.
    /// </summary>
    public string? ProjectAssetsFile { get; }
}
