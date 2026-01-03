using System;
using System.ComponentModel;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using libMachLearn.Models;

namespace Mnist2Learn;

class Program
{
    static void Init()
    {
        IoC.GetReqSrv = t => t switch
        {
            _ when t == typeof(NeuralNetwork) => new NeuralNetwork(0.08, 784, 64, 10),
            _ when t == typeof(IRandom) => new CRandom(),
            _ => throw new NotImplementedException($"No service for {t}"),
        };
    }

    static void Main(string[] args)
    {
        Init();
        // 1. Netzwerk initialisieren: 
        // 2 Eingänge (für 0 oder 1)
        // 3 Hidden Neurons (reicht für XOR locker aus)
        // 1 Ausgang (Ergebnis 0 oder 1)
        // Lernrate: 0.5
        var rnd = IoC.GetRequiredService<IRandom>();
        NeuralNetwork nn = IoC.GetRequiredService<NeuralNetwork>();

        string imgPath = "train-images.idx3-ubyte";
        string lblPath = "train-labels.idx1-ubyte"; // Du brauchst diese Datei!

        Console.WriteLine("Lade MNIST Daten...");
        var trainingData = MnistReader.ReadTrainingData(imgPath, lblPath).ToList();

        Console.WriteLine($"Starte Training mit {trainingData.Count} Bildern...");
        for (int epoch = 0; epoch < 2; epoch++) // Mehr Epochen verbessern die Genauigkeit
        {
            // Eine Epoche reicht oft schon für >90% Genauigkeit
            for (int i = 0; i < trainingData.Count; i++)
            {
                nn.Train(trainingData[i].Data, trainingData[i].Label);
                var rData = new float[trainingData[i].Data.Length];
                for (int j = 0; j < trainingData[i].Data.Length; j++)
                    rData[j] = (float)(((trainingData[i].Data[j] * 2 - 1) * (0.9 + rnd.NextDouble() * 0.1)) * 0.5 + 1); // Add Noise
                for (int j = 0; j < trainingData[i].Data.Length; j++)
                    trainingData[i].Data[j] = 1f - trainingData[i].Data[j];
                nn.Train(rData, trainingData[i].Label);
                nn.Train(trainingData[i].Data, trainingData[i].Label);

                if (i % 5000 == 0)
                    Console.WriteLine($"E{epoch + 1} {i} Bilder verarbeitet...");
            }

            // Testen mit einem Bild aus dem Datensatz
            string timgPath = "t10k-images.idx3-ubyte";
            string tlblPath = "t10k-labels.idx1-ubyte"; // Du brauchst diese Datei!
            var testData = MnistReader.ReadTrainingData(timgPath, tlblPath).ToList();

            int correct = 0;
            int correct2 = 0;
            int correct3 = 0;
            foreach (var testImg in testData)
            {
                float[] prediction = nn.FeedForward(testImg.Data);
                int predictedDigit = Array.IndexOf(prediction, prediction.Max());
                float prob = prediction[predictedDigit];
                if (predictedDigit == Array.IndexOf(testImg.Label, 1d)) correct++;
                var rData = new float[testImg.Data.Length];

                for (int i = 0; i < testImg.Data.Length; i++)
                    rData[i] = (float)(((testImg.Data[i] * 2 - 1) * (0.9 + rnd.NextDouble() * 0.1)) * 0.5 + 1); // Add Noise
                float[] prediction2 = nn.FeedForward(rData);
                int predictedDigit2 = Array.IndexOf(prediction2, prediction2.Max());
                double prob2 = prediction[predictedDigit];
                if (predictedDigit2 == Array.IndexOf(testImg.Label, 1d)) correct2++;

                for (int i = 0; i < testImg.Data.Length; i++)
                    rData[i] = (float)(((testImg.Data[i] * 2 - 1) * (0.8 + rnd.NextDouble() * 0.2)) * 0.5 + 1); // Add Noise
                float[] prediction3 = nn.FeedForward(rData);
                int predictedDigit3 = Array.IndexOf(prediction3, prediction3.Max());
                double prob3 = prediction[predictedDigit];
                if (predictedDigit3 == Array.IndexOf(testImg.Label, 1d)) correct3++;
            }

            Console.WriteLine($"Ergebnis E{epoch + 1} ohne Rauschen: {correct} von {testData.Count} richtig ({(double)correct / testData.Count * 100:0.00}%)");
            Console.WriteLine($"Ergebnis E{epoch + 1} mit Rauschen: {correct2} von {testData.Count} richtig ({(double)correct2 / testData.Count * 100:0.00}%)");
            Console.WriteLine($"Ergebnis E{epoch + 1} mit Rauschen: {correct3} von {testData.Count} richtig ({(double)correct3 / testData.Count * 100:0.00}%)");
        }
    }

}