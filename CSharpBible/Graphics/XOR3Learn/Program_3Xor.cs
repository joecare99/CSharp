using System;
using System.ComponentModel;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using libMachLearn.Models;

namespace XOR3Learn;

class Program_3Xor
{
    static void Init()
    {
        IoC.GetReqSrv = t => t switch
        {
            _ when t == typeof(NeuralNetwork) => new NeuralNetwork(0.5, 3, (3, eActivation.Sigmoid), (1, eActivation.Sigmoid)),
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

        // 2. Trainingsdaten (XOR-Logik)
        float[][] inputs =
        [
            [0, 0, 0],
            [0, 1 ,0],
            [1, 0 ,0],
            [1, 1, 0],
            [0, 0, 1],
            [0, 1 ,1],
            [1, 0 ,1],
            [1, 1, 1]
        ];

        float[][] targets =
        [
            [0],
            [1],
            [1],
            [0],
            [1],
            [0],
            [0],
            [1]
        ];

        Console.WriteLine("Starte Training...");

        // 3. Training: Wir lassen das Netz 50.000 Mal über die Daten laufen (Epochs)
        for (int i = 0; i < 50000; i++)
        {
            for (int j = 0; j < inputs.Length; j++)
            {
                nn.Train(inputs[j], targets[j]);
            }

            // Fortschritt alle 10.000 Epochen anzeigen
            if (i % 10000 == 0)
            {
                Console.WriteLine($"Epoche {i} abgeschlossen...");
                var result = 1.0;
                foreach (var tuple in inputs.Zip(targets))
                {
                    float[] output = nn.FeedForward(tuple.First);
                    result *= (output[0] - 0.5) * (tuple.Second[0] - 0.5) * 4;
                }
                Console.WriteLine($"Zwischenergebnis: {result:F4}");
            }
        }
            Console.WriteLine("\nTraining beendet. Testergebnisse:");
            Console.WriteLine("------------------------------------");

            // 4. Testen
            var finalresult = 1.0;
            foreach (var tuple in inputs.Zip(targets))
            {
                float[] output = nn.FeedForward(tuple.First);
                Console.WriteLine($"Input: {tuple.First[0]}, {tuple.First[1]}, {tuple.First[2]} | Vorhersage: {output[0]:F4}");
                finalresult *= (output[0] - 0.5) * (tuple.Second[0] - 0.5) * 4;
            }
            Console.WriteLine($"Ergebnis: {finalresult:F4}");


            Console.ReadLine();
        
    }
}