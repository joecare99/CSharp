using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ollama.CodingAgent.Console.Commands;

/// <summary>
/// Parses REPL commands without passing raw shell syntax to any process.
/// </summary>
public static class ConsoleCommandParser
{
    private static readonly IReadOnlyDictionary<string, ConsoleCommandKind> SingleTokenCommands =
        new Dictionary<string, ConsoleCommandKind>(StringComparer.Ordinal)
        {
            ["help"] = ConsoleCommandKind.Help,
            ["status"] = ConsoleCommandKind.Status,
            ["transcript"] = ConsoleCommandKind.Transcript,
            ["reload"] = ConsoleCommandKind.Reload,
            ["clear"] = ConsoleCommandKind.Clear,
            ["cancel"] = ConsoleCommandKind.Cancel,
            ["approvals"] = ConsoleCommandKind.Approvals,
            ["exit"] = ConsoleCommandKind.Exit,
            ["quit"] = ConsoleCommandKind.Exit,
        };

    private static readonly IReadOnlyDictionary<string, ConsoleCommandKind> SingleArgumentCommands =
        new Dictionary<string, ConsoleCommandKind>(StringComparer.Ordinal)
        {
            ["approve"] = ConsoleCommandKind.Approve,
            ["reject"] = ConsoleCommandKind.Reject,
        };

    /// <summary>
    /// Parses one input line. Lines not beginning with a colon are agent prompts.
    /// </summary>
    public static ConsoleCommandParseResult Parse(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return Success(ConsoleCommandKind.Empty);
        }

        string trimmedLine = line.Trim();
        if (!trimmedLine.StartsWith(':') && !trimmedLine.StartsWith('/'))
        {
            return new ConsoleCommandParseResult
            {
                Command = new ConsoleCommand
                {
                    Kind = ConsoleCommandKind.Prompt,
                    Argument = trimmedLine,
                },
            };
        }

        if (!TryTokenize(trimmedLine[1..], out IReadOnlyList<string>? tokens, out string? error))
        {
            return new ConsoleCommandParseResult { Error = error };
        }

        if (tokens.Count == 0)
        {
            return new ConsoleCommandParseResult { Error = "Enter a command after ':' or use :help." };
        }

        string commandName = tokens[0].ToLowerInvariant();
        if (commandName == "config")
        {
            int separatorIndex = trimmedLine.IndexOfAny([' ', '\t']);
            return new ConsoleCommandParseResult
            {
                Command = new ConsoleCommand
                {
                    Kind = ConsoleCommandKind.Config,
                    Argument = separatorIndex < 0 ? null : trimmedLine[(separatorIndex + 1)..].Trim(),
                },
            };
        }

        if (SingleTokenCommands.TryGetValue(commandName, out ConsoleCommandKind singleTokenCommand))
        {
            return tokens.Count == 1
                ? Success(singleTokenCommand)
                : UnknownOrMalformed(tokens[0]);
        }

        if (SingleArgumentCommands.TryGetValue(commandName, out ConsoleCommandKind singleArgumentCommand))
        {
            return ParseSingleArgument(singleArgumentCommand, tokens);
        }

        return commandName == "prompt"
            ? ParsePrompt(tokens)
            : UnknownOrMalformed(tokens[0]);
    }

    private static ConsoleCommandParseResult ParseSingleArgument(ConsoleCommandKind kind, IReadOnlyList<string> tokens)
    {
        if (tokens.Count != 2 || string.IsNullOrWhiteSpace(tokens[1]))
        {
            return new ConsoleCommandParseResult { Error = $":{kind.ToString().ToLowerInvariant()} requires one approval identifier." };
        }

        return new ConsoleCommandParseResult
        {
            Command = new ConsoleCommand
            {
                Kind = kind,
                Argument = tokens[1],
            },
        };
    }

    private static ConsoleCommandParseResult ParsePrompt(IReadOnlyList<string> tokens)
    {
        if (tokens.Count < 2)
        {
            return new ConsoleCommandParseResult { Error = ":prompt requires prompt text." };
        }

        return new ConsoleCommandParseResult
        {
            Command = new ConsoleCommand
            {
                Kind = ConsoleCommandKind.Prompt,
                Argument = string.Join(" ", tokens.Skip(1)),
            },
        };
    }

    private static ConsoleCommandParseResult Success(ConsoleCommandKind kind)
        => new()
        {
            Command = new ConsoleCommand { Kind = kind },
        };

    private static ConsoleCommandParseResult UnknownOrMalformed(string commandName)
        => new()
        {
            Error = $"Unknown or malformed command ':{commandName}'. Use :help.",
        };

    private static bool TryTokenize(string input, out IReadOnlyList<string> tokens, out string? error)
    {
        List<string> parsedTokens = [];
        StringBuilder token = new();
        bool inQuotes = false;
        bool isEscaping = false;
        bool hasToken = false;

        foreach (char character in input)
        {
            if (isEscaping)
            {
                if (character is not '"' and not '\\')
                {
                    tokens = [];
                    error = "Only quotes and backslashes can be escaped in commands.";
                    return false;
                }

                token.Append(character);
                isEscaping = false;
                hasToken = true;
                continue;
            }

            if (character == '\\')
            {
                isEscaping = true;
                continue;
            }

            if (character == '"')
            {
                inQuotes = !inQuotes;
                hasToken = true;
                continue;
            }

            if (char.IsWhiteSpace(character) && !inQuotes)
            {
                if (hasToken)
                {
                    parsedTokens.Add(token.ToString());
                    token.Clear();
                    hasToken = false;
                }

                continue;
            }

            token.Append(character);
            hasToken = true;
        }

        if (isEscaping || inQuotes)
        {
            tokens = [];
            error = "Command contains an unfinished escape or quoted value.";
            return false;
        }

        if (hasToken)
        {
            parsedTokens.Add(token.ToString());
        }

        tokens = parsedTokens;
        error = null;
        return true;
    }
}
