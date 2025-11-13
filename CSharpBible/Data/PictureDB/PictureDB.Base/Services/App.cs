using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PictureDB.Base.Models;
using PictureDB.Base.Models.Interfaces;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class App
{
    private readonly IImageLoader _loader;
    private readonly IImageProcessor _processor;
    private readonly ILLMClient _llm;
    private readonly ICategorizer _categorizer;
    private readonly IEvaluator _evaluator;
    private readonly ISorter _sorter;

    public App(
        IImageLoader loader,
        IImageProcessor processor,
        ILLMClient llm,
        ICategorizer categorizer,
        IEvaluator evaluator,
        ISorter sorter)
    {
        _loader = loader;
        _processor = processor;
        _llm = llm;
        _categorizer = categorizer;
        _evaluator = evaluator;
        _sorter = sorter;
    }

    public async Task RunAsync(string folderPath)
    {
        var results = new List<ImageResult>();

        foreach (var file in _loader.LoadImages(folderPath))
        {
            string base64 = _processor.ConvertToBase64(file);
            string response = await _llm.AnalyzeImageAsync(base64, "Kategorisiere und bewerte dieses Bild.");

            var result = new ImageResult
            {
                FilePath = file,
                Category = _categorizer.ExtractCategory(response),
                Score = _evaluator.ExtractScore(response)
            };
            results.Add(result);
        }

        var sorted = _sorter.SortByScore(results);

        foreach (var res in sorted)
        {
            Console.WriteLine($"{res.FilePath} | {res.Category} | Score: {res.Score}");
        }
    }
}