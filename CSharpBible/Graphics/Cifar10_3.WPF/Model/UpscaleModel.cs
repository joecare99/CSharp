using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;

namespace Cifar10.WPF.Model;

public sealed class UpscalePixelInput
{
    public const int LowResSide = 16;
    public const int LowResFeatureLength = LowResSide * LowResSide * 3;
    public const int HighResSide = 32;
    public const int FeatureLength = LowResFeatureLength + 3; // +x, y, channel

    [VectorType(FeatureLength)]
    public float[] Features { get; set; } = Array.Empty<float>();

    public float Label { get; set; }
}

public sealed class UpscalePixelPrediction
{
    public float Score { get; set; }
}

public sealed class UpscaleModel
{
    private readonly MLContext _context;
    private ITransformer? _model;
    private PredictionEngine<UpscalePixelInput, UpscalePixelPrediction>? _engine;

    public UpscaleModel(MLContext context)
    {
        _context = context;
    }

    public RegressionMetrics Train(IEnumerable<UpscalePixelInput> samples)
    {
        var dataView = _context.Data.LoadFromEnumerable(samples);
        var pipeline = _context.Regression.Trainers.Sdca(
            labelColumnName: nameof(UpscalePixelInput.Label),
            featureColumnName: nameof(UpscalePixelInput.Features));

        _model = pipeline.Fit(dataView);
        _engine = _context.Model.CreatePredictionEngine<UpscalePixelInput, UpscalePixelPrediction>(_model);

        var predictions = _model.Transform(dataView);
        return _context.Regression.Evaluate(
            predictions,
            labelColumnName: nameof(UpscalePixelInput.Label),
            scoreColumnName: "Score");
    }

    public static UpscalePixelInput CreateSample(float[] lowResFeatures, int x, int y, int channel, float label)
    {
        var features = new float[UpscalePixelInput.FeatureLength];
        Array.Copy(lowResFeatures, features, lowResFeatures.Length);
        features[UpscalePixelInput.LowResFeatureLength] = x / (float)UpscalePixelInput.HighResSide;
        features[UpscalePixelInput.LowResFeatureLength + 1] = y / (float)UpscalePixelInput.HighResSide;
        features[UpscalePixelInput.LowResFeatureLength + 2] = channel;

        return new UpscalePixelInput
        {
            Features = features,
            Label = label
        };
    }

    public static float[] MergeLowResChannels(float[] r, float[] g, float[] b)
    {
        var merged = new float[UpscalePixelInput.LowResFeatureLength];
        for (int i = 0; i < r.Length; i++)
        {
            merged[i * 3] = r[i];
            merged[i * 3 + 1] = g[i];
            merged[i * 3 + 2] = b[i];
        }
        return merged;
    }

    public (float[] r, float[] g, float[] b) Upscale(float[] lowResFeatures)
    {
        if (_engine == null)
            throw new InvalidOperationException("Model has not been trained.");

        var r = new float[UpscalePixelInput.HighResSide * UpscalePixelInput.HighResSide];
        var g = new float[r.Length];
        var b = new float[r.Length];

        for (int y = 0; y < UpscalePixelInput.HighResSide; y++)
        {
            for (int x = 0; x < UpscalePixelInput.HighResSide; x++)
            {
                r[y * UpscalePixelInput.HighResSide + x] = PredictPixel(lowResFeatures, x, y, 0);
                g[y * UpscalePixelInput.HighResSide + x] = PredictPixel(lowResFeatures, x, y, 1);
                b[y * UpscalePixelInput.HighResSide + x] = PredictPixel(lowResFeatures, x, y, 2);
            }
        }

        return (r, g, b);
    }

    private float PredictPixel(float[] lowResFeatures, int x, int y, int channel)
    {
        if (_engine == null)
            throw new InvalidOperationException("Model has not been trained.");

        var sample = CreateSample(lowResFeatures, x, y, channel, 0f);
        var score = _engine.Predict(sample).Score;
        if (score < 0f) return 0f;
        if (score > 1f) return 1f;
        return score;
    }
}
