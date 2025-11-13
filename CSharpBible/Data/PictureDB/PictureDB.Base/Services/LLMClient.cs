using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class LLMClient : ILLMClient
{
    private readonly HttpClient _httpClient;

    public LLMClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AnalyzeImageAsync(string base64Image, string prompt)
    {
        var payload = $"{{\"image\":\"{base64Image}\",\"prompt\":\"{prompt}\"}}";
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("analyze", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}