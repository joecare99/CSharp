using AA98_AvlnCodeStudio.Base.AI.Models;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Sends chat-completions requests to OpenAI-compatible local or remote providers.
/// </summary>
public sealed class OpenAICompatibleAIClient(OpenAICompatibleAISettings settings) : IAIClient
{
    private static readonly TimeSpan s_requestTimeout = TimeSpan.FromSeconds(500);
    private static readonly HttpClient s_httpClient = new() { Timeout = Timeout.InfiniteTimeSpan };
    private readonly OpenAICompatibleAISettings _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    private readonly HttpClient _httpClient = s_httpClient;
    private readonly TimeSpan _requestTimeout = s_requestTimeout;

    public OpenAICompatibleAIClient(OpenAICompatibleAISettings settings, HttpClient httpClient)
        : this(settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public OpenAICompatibleAIClient(OpenAICompatibleAISettings settings, HttpClient httpClient, TimeSpan requestTimeout)
        : this(settings, httpClient)
    {
        if (requestTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(requestTimeout), "The AI request timeout must be greater than zero.");
        }

        _requestTimeout = requestTimeout;
    }

    public async Task<AICompletionResponse> CompleteAsync(
        AICompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        Uri endpoint = ValidateEndpoint(_settings.Endpoint);
        string model = string.IsNullOrWhiteSpace(request.Options.ModelId)
            ? _settings.ModelId
            : request.Options.ModelId;
        if (string.IsNullOrWhiteSpace(model))
        {
            throw new InvalidOperationException("Configure an AI model before sending a chat message.");
        }

        Dictionary<string, string> providerToolNames = CreateProviderToolNames(request.Tools);
        Dictionary<string, string> internalToolNames = providerToolNames
            .ToDictionary(static entry => entry.Value, static entry => entry.Key, StringComparer.Ordinal);
        object payload = new
        {
            model,
            messages = request.Messages.Select(message => ToProviderMessage(message, providerToolNames)).ToArray(),
            tools = request.Tools.Count == 0
                ? null
                : request.Tools.Select(tool => ToProviderTool(tool, providerToolNames[tool.Name])).ToArray(),
            tool_choice = request.Tools.Count == 0 ? null : "auto",
            temperature = request.Options.Temperature,
            max_tokens = request.Options.MaxTokens,
        };
        using HttpRequestMessage httpRequest = new(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
        };
        string apiKey = ResolveApiKey(_settings.ApiKey);
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        (HttpStatusCode StatusCode, string? ReasonPhrase, string Body) providerResponse =
            await SendRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        if ((int)providerResponse.StatusCode is < 200 or >= 300)
        {
            string detail = providerResponse.Body.Length > 2000 ? providerResponse.Body[..2000] : providerResponse.Body;
            throw new HttpRequestException(
                $"AI provider returned HTTP {(int)providerResponse.StatusCode} ({providerResponse.ReasonPhrase}): {detail}",
                null,
                providerResponse.StatusCode);
        }

        using JsonDocument document = JsonDocument.Parse(providerResponse.Body);
        JsonElement message = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message");
        string content = message.TryGetProperty("content", out JsonElement contentElement)
            && contentElement.ValueKind == JsonValueKind.String
            ? contentElement.GetString() ?? string.Empty
            : string.Empty;
        List<AIChatToolCall> calls = [];
        if (message.TryGetProperty("tool_calls", out JsonElement toolCalls))
        {
            foreach (JsonElement call in toolCalls.EnumerateArray())
            {
                JsonElement function = call.GetProperty("function");
                using JsonDocument arguments = JsonDocument.Parse(function.GetProperty("arguments").GetString() ?? "{}");
                string providerToolName = function.GetProperty("name").GetString()
                    ?? throw new JsonException("An AI tool call has no name.");
                calls.Add(new AIChatToolCall(
                    call.GetProperty("id").GetString() ?? throw new JsonException("An AI tool call has no id."),
                    internalToolNames.TryGetValue(providerToolName, out string? internalToolName)
                        ? internalToolName
                        : providerToolName,
                    arguments.RootElement));
            }
        }

        return new AICompletionResponse(
            new AIMessage(AIMessageRole.Assistant, content, toolCalls: calls),
            calls);
    }

    private async Task<(HttpStatusCode StatusCode, string? ReasonPhrase, string Body)> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(_requestTimeout);
        try
        {
            using HttpResponseMessage response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                timeoutSource.Token).ConfigureAwait(false);
            string body = await response.Content.ReadAsStringAsync(timeoutSource.Token).ConfigureAwait(false);
            return (response.StatusCode, response.ReasonPhrase, body);
        }
        catch (OperationCanceledException exception)
            when (!cancellationToken.IsCancellationRequested && timeoutSource.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"The AI provider request exceeded the {_requestTimeout.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture)}-second timeout.",
                exception);
        }
    }

    private static Uri ValidateEndpoint(string? endpointText)
    {
        if (!Uri.TryCreate(endpointText, UriKind.Absolute, out Uri? endpoint)
            || (endpoint.Scheme != Uri.UriSchemeHttps && endpoint.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException("The AI endpoint must be an absolute HTTP or HTTPS URL.");
        }

        if (endpoint.Scheme == Uri.UriSchemeHttp && !endpoint.IsLoopback)
        {
            throw new InvalidOperationException("Non-local AI endpoints must use HTTPS to protect credentials and source context.");
        }

        return endpoint;
    }

    private static string ResolveApiKey(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return string.Empty;
        }

        const string EnvironmentVariablePrefix = "env:";
        if (!apiKey.StartsWith(EnvironmentVariablePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return apiKey;
        }

        string variableName = apiKey[EnvironmentVariablePrefix.Length..].Trim();
        if (string.IsNullOrWhiteSpace(variableName))
        {
            throw new InvalidOperationException("The API key environment reference must use the form env:VARIABLE_NAME.");
        }

        return Environment.GetEnvironmentVariable(variableName)
            ?? throw new InvalidOperationException(
                $"The API key environment variable '{variableName}' is not defined for this process.");
    }

    private static object ToProviderMessage(
        AIMessage message,
        IReadOnlyDictionary<string, string> providerToolNames)
    {
        return message.Role switch
        {
            AIMessageRole.Tool => new
            {
                role = "tool",
                content = message.Content,
                tool_call_id = message.ToolCallId,
            },
            AIMessageRole.Assistant when message.ToolCalls.Count > 0 => new
            {
                role = "assistant",
                content = string.IsNullOrEmpty(message.Content) ? null : message.Content,
                tool_calls = message.ToolCalls.Select(call => new
                {
                    id = call.CallId,
                    type = "function",
                    function = new
                    {
                        name = providerToolNames.TryGetValue(call.Name, out string? providerName)
                            ? providerName
                            : call.Name,
                        arguments = call.Arguments.GetRawText(),
                    },
                }).ToArray(),
            },
            _ => new
            {
                role = message.Role switch
                {
                    AIMessageRole.System => "system",
                    AIMessageRole.User => "user",
                    AIMessageRole.Assistant => "assistant",
                    _ => throw new ArgumentOutOfRangeException(nameof(message)),
                },
                content = message.Content,
            },
        };
    }

    private static Dictionary<string, string> CreateProviderToolNames(
        IReadOnlyList<AIChatToolDescriptor> tools)
    {
        HashSet<string> reservedNames = tools
            .Where(static tool => IsProviderCompatibleToolName(tool.Name))
            .Select(static tool => tool.Name)
            .ToHashSet(StringComparer.Ordinal);
        HashSet<string> usedNames = new(StringComparer.Ordinal);
        Dictionary<string, string> providerNames = new(StringComparer.Ordinal);
        int fallbackIndex = 0;

        foreach (AIChatToolDescriptor tool in tools)
        {
            string providerName = tool.Name;
            if (!IsProviderCompatibleToolName(providerName))
            {
                char[] sanitizedName = providerName
                    .Select(static character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-'
                        ? character
                        : '_')
                    .ToArray();
                providerName = new string(sanitizedName);
                if (!IsProviderCompatibleToolName(providerName)
                    || reservedNames.Contains(providerName)
                    || usedNames.Contains(providerName))
                {
                    do
                    {
                        providerName = $"avln_tool_{fallbackIndex++}";
                    }
                    while (reservedNames.Contains(providerName) || usedNames.Contains(providerName));
                }
            }

            usedNames.Add(providerName);
            providerNames.Add(tool.Name, providerName);
        }

        return providerNames;
    }

    private static bool IsProviderCompatibleToolName(string name)
    {
        return name.Length <= 64
            && name.All(static character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-');
    }

    private static object ToProviderTool(AIChatToolDescriptor descriptor, string providerName)
    {
        return new
        {
            type = "function",
            function = new
            {
                name = providerName,
                description = descriptor.Description,
                parameters = descriptor.InputSchema,
            },
        };
    }
}
