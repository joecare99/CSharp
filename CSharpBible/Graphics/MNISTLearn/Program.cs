using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using libMachLearn.Models;

namespace Cnt3Learn;

class Program
{
    static void Init()
    {
        IoC.GetReqSrv = t => t switch
        {
            _ when t == typeof(NeuralNetwork) => new NeuralNetwork(0.005, 784, 128, 10),
            _ when t == typeof(IRandom) => new CRandom(),
            _ => throw new NotImplementedException($"No service for {t}"),
        };
    }

    static void Main(string[] args)
    {
        Init();
        var rnd = IoC.GetRequiredService<IRandom>();
        NeuralNetwork nn = IoC.GetRequiredService<NeuralNetwork>();

        string imgPath = "train-images.idx3-ubyte";
        string lblPath = "train-labels.idx1-ubyte";

        Console.WriteLine("Lade MNIST Daten...");
        var trainingData = MnistReader.ReadTrainingData(imgPath, lblPath).ToList();
        int[] idx = [-29, -28, -27, -1, 0, 1, 27, 28, 29];

        Console.WriteLine($"Starte Training mit {trainingData.Count} Bildern...");
        for (int epoch = 0; epoch < 30; epoch++)
        {
            for (int i = 5; i > epoch; i--)
            {
                nn.AdjustILWeights(idx,
                               [0.001, 0.002, 0.001, 0.002, 0.99, 0.002, 0.001, 0.002, 0.001]);
            }

            nn.LearningRate *= 0.95;
            for (int i = 0; i < trainingData.Count; i++)
            {
                //nn.Train(trainingData[i].Data, trainingData[i].Label);
                
                var rOffs = rnd.Next(9);
                var rAmt = rnd.NextDouble() * 0.2;
                var rData = new double[trainingData[i].Data.Length];
                rData.Initialize();
                for (int j = 0; j < trainingData[i].Data.Length; j++)
                    if (j + idx[rOffs] >= 0 && j + idx[rOffs] < trainingData[i].Data.Length)
                    rData[j] = ((trainingData[i].Data[j + idx[rOffs]] * 2 - 1) * (1.0-rAmt + rnd.NextDouble() * rAmt)) * 0.5 + 1;
             
                nn.Train(rData, trainingData[i].Label);
          //      nn.Train(trainingData[i].Data, trainingData[i].Label);
                
                if (i % 5000 == 0)
                    Console.WriteLine($"E{epoch + 1} {i} Bilder verarbeitet...");
            }


            string timgPath = "t10k-images.idx3-ubyte";
            string tlblPath = "t10k-labels.idx1-ubyte";
            var testData = MnistReader.ReadTrainingData(timgPath, tlblPath).ToList();

            int correct = 0;
            int correct2 = 0;
            int correct3 = 0;
            foreach (var testImg in testData)
            {
                double[] prediction = nn.FeedForward(testImg.Data);
                int predictedDigit = Array.IndexOf(prediction, prediction.Max());
                if (predictedDigit == Array.IndexOf(testImg.Label, 1d)) correct++;
                var rData = new double[testImg.Data.Length];

                for (int i = 0; i < testImg.Data.Length; i++)
                    rData[i] = ((testImg.Data[i] * 2 - 1) * (0.9 + rnd.NextDouble() * 0.1)) * 0.5 + 1;
                double[] prediction2 = nn.FeedForward(rData);
                int predictedDigit2 = Array.IndexOf(prediction2, prediction2.Max());
                if (predictedDigit2 == Array.IndexOf(testImg.Label, 1d)) correct2++;

                for (int i = 0; i < testImg.Data.Length; i++)
                    rData[i] = ((testImg.Data[i] * 2 - 1) * (0.8 + rnd.NextDouble() * 0.2)) * 0.5 + 1;
                double[] prediction3 = nn.FeedForward(rData);
                int predictedDigit3 = Array.IndexOf(prediction3, prediction3.Max());
                if (predictedDigit3 == Array.IndexOf(testImg.Label, 1d)) correct3++;
            }

            Console.WriteLine($"Ergebnis E{epoch + 1} ohne Rauschen: {correct} von {testData.Count} richtig ({(double)correct / testData.Count * 100:0.00}%)");
            Console.WriteLine($"Ergebnis E{epoch + 1} mit Rauschen: {correct2} von {testData.Count} richtig ({(double)correct2 / testData.Count * 100:0.00}%)");
            Console.WriteLine($"Ergebnis E{epoch + 1} mit Rauschen: {correct3} von {testData.Count} richtig ({(double)correct3 / testData.Count * 100:0.00}%)");
            VisualizeImportantNeurons(nn);
        }

    }

    static void VisualizeImportantNeurons(NeuralNetwork nn)
    {
        const int inputSize = 784;
        const int hiddenSize = 128;
        const int outputSize = 10;
        const int imageSide = 28;

        if (!TryResolveWeightMatrices(nn, inputSize, hiddenSize, outputSize, out var matrices))
        {
            Console.WriteLine("Visualisierung nicht möglich: Gewichtsmatrizen wurden nicht gefunden.");
            return;
        }

        Console.WriteLine("\nVisualisierung der wichtigsten Neuronen:");
        for (int digit = 0; digit < outputSize; digit++)
        {
            var hoRow = matrices.HiddenOutput[digit];
            int maxIdx = IndexOfMax(hoRow);
            int minIdx = IndexOfMin(hoRow);

            var outp = new double[outputSize];
            outp.Initialize();
            outp[digit] = 1.0;
            var optinput = nn.GenerateInputForTarget(outp, 1000, 0.1);  

            Console.WriteLine($"Ziffer {digit}: höchster Einfluss Hidden #{maxIdx}, niedrigster Einfluss Hidden #{minIdx}");
            RenderNeuron($"   Höchster Einfluss (Gewicht {hoRow[maxIdx]:0.000})  Niedrigster Einfluss (Gewicht {hoRow[minIdx]:0.000})", matrices.InputHidden[maxIdx], matrices.InputHidden[minIdx], optinput, imageSide);
        }
    }

    static void RenderNeuron(string title, double[] weights,double[] weights2, double[] optinput, int side)
    {
        Console.WriteLine(title);
        if (weights.Length != side * side)
        {
            Console.WriteLine("   (Visualisierung übersprungen: Unerwartete Eingangsdimension)");
            return;
        }

        double maxAbs = weights.Select(Math.Abs).DefaultIfEmpty(0d).Max();
        double maxAbs2 = weights2.Select(Math.Abs).DefaultIfEmpty(0d).Max();
        double maxAbs3 = optinput.Select(Math.Abs).DefaultIfEmpty(0d).Max();
        if (maxAbs == 0d) maxAbs = 1d;
        if (maxAbs2 == 0d) maxAbs2 = 1d;
        if (maxAbs3 == 0d) maxAbs3 = 1d;

        for (int row = 0; row < side; row += 2)
        {
            var line = new StringBuilder();
          
            for (int col = 0; col < side; col++)
            {
                double topValue = weights[row * side + col];
                double bottomValue = weights[(row + 1) * side + col];
                line.Append(BuildHalfBlock(topValue, bottomValue, maxAbs));
            }

            line.Append("\u001b[0m ");

            for (int col = 0; col < side; col++)
            {
                double topValue = weights2[row * side + col];
                double bottomValue = weights2[(row + 1) * side + col];
                line.Append(BuildHalfBlock(topValue, bottomValue, maxAbs2));
            }

            line.Append("\u001b[0m ");

            for (int col = 0; col < side; col++)
            {
                double topValue = optinput[row * side + col];
                double bottomValue = optinput[(row + 1) * side + col];
                line.Append(BuildHalfBlock(topValue, bottomValue, maxAbs3));
            }

            line.Append("\u001b[0m.");
            Console.WriteLine(line.ToString());
        }

        Console.WriteLine("\u001b[0m");
    }

    static string BuildHalfBlock(double topValue, double bottomValue, double maxAbs)
    {
        var topColor = MapValueToColor(topValue, maxAbs);
        var bottomColor = MapValueToColor(bottomValue, maxAbs);

        return $"\u001b[38;2;{topColor.r};{topColor.g};{topColor.b}m\u001b[48;2;{bottomColor.r};{bottomColor.g};{bottomColor.b}m▀";
    }

    static (int r, int g, int b) MapValueToColor(double value, double maxAbs)
    {
        if (Math.Abs(value) < double.Epsilon)
        {
            return (0, 0, 0);
        }

        double normalized = Math.Clamp(Math.Abs(value) / maxAbs, 0d, 1d);
        int intensity = (int)Math.Round(normalized * 255d, MidpointRounding.AwayFromZero);

        return value > 0d ? (0, intensity, 0) : (intensity, 0, 0);
    }

    static int IndexOfMax(double[] values)
    {
        int index = 0;
        double max = double.MinValue;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] > max)
            {
                max = values[i];
                index = i;
            }
        }

        return index;
    }

    static int IndexOfMin(double[] values)
    {
        int index = 0;
        double min = double.MaxValue;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] < min)
            {
                min = values[i];
                index = i;
            }
        }

        return index;
    }

    static bool TryResolveWeightMatrices(NeuralNetwork nn, int inputSize, int hiddenSize, int outputSize, out WeightMatrices matrices)
    {
        matrices = default;
        var candidates = nn.Layers.Select(l=>l.Weights).ToList();
        if (candidates.Count == 0)
        {
            return false;
        }

        double[][]? inputHidden = null;
        double[][]? hiddenOutput = null;

        foreach (var candidate in candidates)
        {
            inputHidden ??= MatchMatrix(candidate, hiddenSize, inputSize);
            hiddenOutput ??= MatchMatrix(candidate, outputSize, hiddenSize);

            if (inputHidden != null && hiddenOutput != null)
            {
                matrices = new WeightMatrices(inputHidden, hiddenOutput);
                return true;
            }
        }

        return false;
    }

    static double[][]? MatchMatrix(double[][]? matrix, int expectedRows, int expectedCols)
    {
        if (matrix?.Length == expectedRows && HasExpectedColumns(matrix, expectedCols))
        {
            return matrix;
        }

        if (matrix?.Length == expectedCols && HasExpectedColumns(matrix, expectedRows))
        {
            return Transpose(matrix);
        }

        return null;
    }

    static bool HasExpectedColumns(double[][] matrix, int expectedCols)
        => matrix.All(row => row != null && row.Length == expectedCols);

    static double[][] Transpose(double[][] matrix)
    {
        if (matrix.Length == 0 || matrix[0] == null)
        {
            return Array.Empty<double[]>();
        }

        int rows = matrix.Length;
        int cols = matrix[0].Length;
        var transposed = new double[cols][];
        for (int c = 0; c < cols; c++)
        {
            transposed[c] = new double[rows];
            for (int r = 0; r < rows; r++)
            {
                transposed[c][r] = matrix[r][c];
            }
        }

        return transposed;
    }

    private sealed record WeightMatrices(double[][] InputHidden, double[][] HiddenOutput);
}