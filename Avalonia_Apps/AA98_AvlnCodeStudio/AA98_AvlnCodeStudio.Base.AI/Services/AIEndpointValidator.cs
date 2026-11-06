using System;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>Validates the transport used by an OpenAI-compatible endpoint.</summary>
public static class AIEndpointValidator
{
    /// <summary>Returns a valid endpoint or throws a user-facing configuration error.</summary>
    public static Uri Validate(string? endpointText)
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

        if (!string.IsNullOrEmpty(endpoint.UserInfo) || !string.IsNullOrEmpty(endpoint.Fragment))
        {
            throw new InvalidOperationException("The AI endpoint must not contain user credentials or a URL fragment.");
        }

        return endpoint;
    }
}
