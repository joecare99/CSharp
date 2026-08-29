using System.Net.Http;
using Leonardo.Models.Interfaces;

namespace Leonardo.ST.Cxaml;

/// <summary>Supplies Leonardo's model contract with the standard HTTP client implementation.</summary>
internal sealed class LeonardoHttpClient : HttpClient, IHttpClient
{
}
