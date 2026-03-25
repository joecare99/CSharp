using System;
using System.ComponentModel;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using libMachLearn.Models;

namespace Mnist1Learn;

class Program
{
    static void Init()
    {
        IoC.GetReqSrv = t => t switch
        {
            _ when t == typeof(NeuralNetwork) => new NeuralNetwork(0.1, 784, (64, eActivation.Sigmoid), (10, eActivation.Sigmoid)),
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
        NeuralNetwork nn = IoC.GetRequiredService<NeuralNetwork>();

        string imgPath = "train-images.idx3-ubyte";
        string lblPath = "train-labels.idx1-ubyte"; // Du brauchst diese Datei!

        Console.WriteLine("Lade MNIST Daten...");
        var trainingData = MnistReader.ReadTrainingData(imgPath, lblPath).ToList();

        Console.WriteLine($"Starte Training mit {trainingData.Count} Bildern...");

        // Eine Epoche reicht oft schon für >90% Genauigkeit
        for (int i = 0; i < trainingData.Count; i++)
        {
            nn.Train_Parallel(trainingData[i].Data, trainingData[i].Label);
            for (int j = 0; j < trainingData[i].Data.Length; j++)
                trainingData[i].Data[j] = 1f - trainingData[i].Data[j];
            nn.Train_Parallel(trainingData[i].Data, trainingData[i].Label);

            if (i % 5000 == 0)
                Console.WriteLine($"{i} Bilder verarbeitet...");
        }

        // Testen mit einem Bild aus dem Datensatz
        var rnd = IoC.GetRequiredService<IRandom>();
        var testImg = trainingData[rnd.Next(trainingData.Count)];
        float[] prediction = nn.FeedForward_Parallel(testImg.Data);
        for (int i = 0; i < testImg.Data.Length; i++)
            testImg.Data[i] = ((testImg.Data[i] * 2 - 1) * (0.75f + (float)rnd.NextDouble() * 0.25f)) * 0.5f + 1; // Add Noise
        float[] prediction2 = nn.FeedForward_Parallel(testImg.Data);

        int predictedDigit = Array.IndexOf(prediction, prediction.Max());
        int predictedDigit2 = Array.IndexOf(prediction2, prediction2.Max());

        Console.WriteLine($"\nTest-Ergebnis:");
        Console.WriteLine($"Echte Ziffer: {testImg.ActualDigit}");
        Console.WriteLine($"KI Vorhersage: {predictedDigit} (Konfidenz: {prediction[predictedDigit]:P2})");
        Console.WriteLine($"KI Vorhersage2: {predictedDigit2} (Konfidenz: {prediction2[predictedDigit2]:P2})");


    }

}