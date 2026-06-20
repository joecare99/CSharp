using System;

namespace Workbench.Builder.Cli;

/// <summary>
/// Provides shared command-line parsing for project-based hosts.
/// </summary>
/// <typeparam name="TOptions">The concrete options type.</typeparam>
public abstract class ProjectCommandLineParserBase<TOptions>
    where TOptions : ProjectCommandOptionsBase
{
    /// <summary>
    /// Parses the specified command-line arguments.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    /// <returns>The parsed command options.</returns>
    public TOptions Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        string? projectFilePath = null;
        bool showHelp = false;

        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];

            if (IsHelpArgument(argument))
            {
                showHelp = true;
                continue;
            }

            if (TryHandleArgument(args, ref i, argument, ref projectFilePath))
            {
                continue;
            }

            if (projectFilePath is null)
            {
                projectFilePath = argument;
                continue;
            }

            throw new ArgumentException($"Unexpected argument '{argument}'.", nameof(args));
        }

        return CreateOptions(projectFilePath, showHelp);
    }

    /// <summary>
    /// Determines whether the specified argument is a shared help argument.
    /// </summary>
    /// <param name="argument">The argument to inspect.</param>
    /// <returns><see langword="true"/> when the argument requests help; otherwise <see langword="false"/>.</returns>
    protected static bool IsHelpArgument(string argument)
    {
        return string.Equals(argument, "-h", StringComparison.OrdinalIgnoreCase)
            || string.Equals(argument, "--help", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Handles a host-specific argument.
    /// </summary>
    /// <param name="args">The raw argument list.</param>
    /// <param name="index">The current argument index.</param>
    /// <param name="argument">The current argument value.</param>
    /// <param name="projectFilePath">The currently resolved project file path.</param>
    /// <returns><see langword="true"/> when the argument was handled; otherwise <see langword="false"/>.</returns>
    protected abstract bool TryHandleArgument(string[] args, ref int index, string argument, ref string? projectFilePath);

    /// <summary>
    /// Creates the concrete options instance.
    /// </summary>
    /// <param name="projectFilePath">The resolved project file path.</param>
    /// <param name="showHelp">A value indicating whether help output was requested.</param>
    /// <returns>The parsed options instance.</returns>
    protected abstract TOptions CreateOptions(string? projectFilePath, bool showHelp);
}
