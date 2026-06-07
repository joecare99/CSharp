namespace CommandlineHelper;

public sealed class CommandParseResult<TOptions>
{
    public CommandParseResult(bool success, bool requestHelp, TOptions? options, string? errorMessage)
    {
        Success = success;
        RequestHelp = requestHelp;
        Options = options;
        ErrorMessage = errorMessage;
    }

    public bool Success { get; }

    public bool RequestHelp { get; }

    public TOptions? Options { get; }

    public string? ErrorMessage { get; }

    public static CommandParseResult<TOptions> FromSuccess(TOptions options)
        => new(true, false, options, null);

    public static CommandParseResult<TOptions> FromHelpRequest()
        => new(false, true, default, null);

    public static CommandParseResult<TOptions> FromError(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message must not be empty.", nameof(errorMessage));

        return new CommandParseResult<TOptions>(false, false, default, errorMessage);
    }
}
