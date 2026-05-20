using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Client.Models;

namespace PictureDB.OllamaTest;

internal static class Program
{
    private const string DefaultEndpoint = "http://localhost:11434/";
    private const string DefaultModel = "llava";
    private const string DefaultPrompt = "Describe this image in detail.";
    private const int MinDimension = 336;

    private static async Task<int> Main(string[] args)
    {
        Console.WriteLine("PictureDB Ollama Vision Test\n");

        if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
        {
            Console.WriteLine("Usage: PictureDB.OllamaTest <ImagePath> [ModelName] [Prompt]");
            Console.WriteLine($"Default Model: {DefaultModel}");
            Console.WriteLine($"Default Prompt: {DefaultPrompt}");
            return 0;
        }

        string imagePath = args[0];
        string model = args.Length > 1 ? args[1] : DefaultModel;
        string prompt = args.Length > 2 ? string.Join(" ", args.Skip(2)) : DefaultPrompt;

        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Error: Image file '{imagePath}' not found.");
            return 1;
        }

        try
        {
            Console.WriteLine($"Attempting to load and process image '{imagePath}'...");

            // Note: System.Drawing requires Windows or libgdiplus on Linux
            using Image originalImage = Image.FromFile(imagePath);
            int safeWidth = originalImage.Width;
            int safeHeight = originalImage.Height;

            if (safeWidth < MinDimension || safeHeight < MinDimension)
            {
                int maxCurrent = Math.Max(safeWidth, safeHeight);
                double scale = (double)MinDimension / maxCurrent;
                safeWidth = (int)(safeWidth * scale);
                safeHeight = (int)(safeHeight * scale);
            }

            using Bitmap normalizedBitmap = new(safeWidth, safeHeight, PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(normalizedBitmap))
            {
                g.Clear(Color.White);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, 0, 0, safeWidth, safeHeight);
            }

            using MemoryStream ms = new();
            ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg) ?? throw new InvalidOperationException("JPEG encoder not found.");
            EncoderParameters encoderParameters = new(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 95L);

            normalizedBitmap.Save(ms, jpegEncoder, encoderParameters);

            string base64Image = Convert.ToBase64String(ms.ToArray());
            Console.WriteLine("Image normalization complete.");

            string endpointValue = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? DefaultEndpoint;

            using HttpClient httpClient = new()
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };

            Console.WriteLine($"Connecting to Ollama at {endpointValue}");
            Console.WriteLine($"Using model: {model}");
            Console.WriteLine($"Prompt: {prompt}\n");

            OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri(endpointValue)));
            OllamaChatClient chatClient = client.GetChatClient(model);

            ChatCompletionOptions options = new()
            {
                Messages =
                [
                    new OllamaClientChatMessage
                    {
                        Role = "user",
                        Content = prompt,
                        Images = [base64Image]
                    }
                ]
            };

            Console.WriteLine("--- Response ---");

            await foreach (var update in chatClient.CompleteChatStreamingAsync(options))
            {
                if (!string.IsNullOrEmpty(update.Content))
                {
                    Console.Write(update.Content);
                }
            }

            Console.WriteLine("\n\n--- Done ---");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError during vision inference: {ex.Message}");
            return 1;
        }
    }

    private static ImageCodecInfo? GetEncoder(ImageFormat format)
    {
        return ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
    }
}