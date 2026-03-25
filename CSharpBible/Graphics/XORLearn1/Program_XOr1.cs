using System;
using System.ComponentModel;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using libMachLearn.Models;

namespace XORLearn1;
class Program_XOr1
{
    static void Init()
    {
        IoC.GetReqSrv= t => t switch { 
            _ when t == typeof(NeuralNetwork) => new NeuralNetwork(0.2, 2, (3,eActivation.ReLU), (1,eActivation.Sigmoid)),
            _ when t == typeof(IRandom) => new CRandom(),
            _ => throw new NotImplementedException($"No service for {t}") ,
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

        // 2. Trainingsdaten (XOR-Logik)
        float[][] inputs =
        [
            [0, 0],
            [0, 1],
            [1, 0],
            [1, 1]
        ];

        float[][] targets =
        [
            [0],
            [1],
            [1],
            [0]
        ];
        nn.LearningRate = 0.5f;
        Console.WriteLine("Starte Training...");
        float accuracy = 0f;
        // 3. Training: Wir lassen das Netz 50.000 Mal über die Daten laufen (Epochs)
        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < inputs.Length; j++)
            {
                nn.Train(inputs[j], targets[j],0.1f);
                accuracy += 1f- MathF.Abs(nn.Layers[2].Neurons[0] - targets[j][0]);
            }

            // Fortschritt alle 10.000 Epochen anzeigen
            if (i % 100 == 0) Console.WriteLine($"Epoche {i} abgeschlossen... {accuracy/i/4f}");
        }

        Console.WriteLine("\nTraining beendet. Testergebnisse:");
        Console.WriteLine("------------------------------------");

        // 4. Testen
        foreach (var input in inputs)
        {
            float[] output = nn.FeedForward(input);
            Console.WriteLine($"Input: {input[0]}, {input[1]} | Vorhersage: {output[0]:F4}");
        }

        Console.ReadLine();
    }
}