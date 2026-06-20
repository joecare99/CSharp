namespace Workbench.Builder.Core.Models.Projects;

/// <summary>
/// Represents the evaluated high-level project information needed for inspection and later compilation.
/// </summary>
public sealed class BuildProjectInfo
{
    /// <summary>
    /// Initializes a new instance of <see cref="BuildProjectInfo"/>.
    /// </summary>
    /// <param name="projectFilePath">The full path to the project file.</param>
    /// <param name="projectDirectory">The full path to the project directory.</param>
    /// <param name="assemblyName">The evaluated assembly name.</param>
    /// <param name="rootNamespace">The evaluated root namespace.</param>
    /// <param name="targetFramework">The evaluated target framework moniker.</param>
    /// <param name="outputType">The optional evaluated output type.</param>
    /// <param name="langVersion">The optional evaluated C# language version.</param>
    /// <param name="nullable">The optional evaluated nullable mode.</param>
    /// <param name="defineConstants">The optional evaluated preprocessor constants.</param>
    /// <param name="implicitUsings">The optional evaluated implicit usings mode.</param>
    /// <param name="configuration">The optional evaluated build configuration.</param>
    /// <param name="runtimeIdentifier">The optional evaluated runtime identifier.</param>
    /// <param name="outputPath">The optional evaluated output path.</param>
    /// <param name="intermediateOutputPath">The optional evaluated intermediate output path.</param>
    /// <param name="isSdkStyle">A value indicating whether the project appears to be SDK-style.</param>
    /// <param name="isPackable">A value indicating whether the project is packable, when known.</param>
    public BuildProjectInfo(
        string projectFilePath,
        string projectDirectory,
        string assemblyName,
        string rootNamespace,
        string targetFramework,
        string? outputType,
        string? langVersion,
        string? nullable,
        string? defineConstants,
        string? implicitUsings,
        string? configuration,
        string? runtimeIdentifier,
        string? outputPath,
        string? intermediateOutputPath,
        bool isSdkStyle,
        bool? isPackable)
    {
        ProjectFilePath = projectFilePath;
        ProjectDirectory = projectDirectory;
        AssemblyName = assemblyName;
        RootNamespace = rootNamespace;
        TargetFramework = targetFramework;
        OutputType = outputType;
        LangVersion = langVersion;
        Nullable = nullable;
        DefineConstants = defineConstants;
        ImplicitUsings = implicitUsings;
        Configuration = configuration;
        RuntimeIdentifier = runtimeIdentifier;
        OutputPath = outputPath;
        IntermediateOutputPath = intermediateOutputPath;
        IsSdkStyle = isSdkStyle;
        IsPackable = isPackable;
    }

    /// <summary>
    /// Gets the full path to the project file.
    /// </summary>
    public string ProjectFilePath { get; }

    /// <summary>
    /// Gets the full path to the project directory.
    /// </summary>
    public string ProjectDirectory { get; }

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
    /// Gets the optional evaluated output type.
    /// </summary>
    public string? OutputType { get; }

    /// <summary>
    /// Gets the optional evaluated C# language version.
    /// </summary>
    public string? LangVersion { get; }

    /// <summary>
    /// Gets the optional evaluated nullable mode.
    /// </summary>
    public string? Nullable { get; }

    /// <summary>
    /// Gets the optional evaluated preprocessor constants.
    /// </summary>
    public string? DefineConstants { get; }

    /// <summary>
    /// Gets the optional evaluated implicit usings mode.
    /// </summary>
    public string? ImplicitUsings { get; }

    /// <summary>
    /// Gets the optional evaluated build configuration.
    /// </summary>
    public string? Configuration { get; }

    /// <summary>
    /// Gets the optional evaluated runtime identifier.
    /// </summary>
    public string? RuntimeIdentifier { get; }

    /// <summary>
    /// Gets the optional evaluated output path.
    /// </summary>
    public string? OutputPath { get; }

    /// <summary>
    /// Gets the optional evaluated intermediate output path.
    /// </summary>
    public string? IntermediateOutputPath { get; }

    /// <summary>
    /// Gets a value indicating whether the project appears to be SDK-style.
    /// </summary>
    public bool IsSdkStyle { get; }

    /// <summary>
    /// Gets a value indicating whether the project is packable, when known.
    /// </summary>
    public bool? IsPackable { get; }
}
