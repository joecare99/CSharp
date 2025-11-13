using System.Threading.Tasks;

namespace PictureDB.Base.Services.Interfaces;

public interface ILLMClient
{
    Task<string> AnalyzeImageAsync(string base64Image, string prompt);
}