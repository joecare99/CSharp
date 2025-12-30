using BaseLib.Helper;
using BaseLib.Models.Interfaces;

namespace libMachLearn.Models;

public class Layer
{
    public double[] Neurons;      // Werte der Neuronen in diesem Layer
    public double[] Biases;       // Schwellenwerte
    public double[][] Weights;    // Gewichte zum vorherigen Layer
    public double[] Deltas;       // Fehlerwerte für Backpropagation

    private IRandom _random = IoC.GetRequiredService<IRandom>();

    public Layer(int size, int previousLayerSize)
    {
        Neurons = new double[size];
        Biases = new double[size];
        Deltas = new double[size];

        // Gewichte initialisieren (kleine Zufallswerte)
        if (previousLayerSize > 0)
        {
            Weights = new double[size][];
            for (int i = 0; i < size; i++)
            {
                Weights[i] = new double[previousLayerSize];
                Biases[i] = _random.NextDouble() * 2 - 1;
                for (int j = 0; j < previousLayerSize; j++)
                {
                    Weights[i][j] = _random.NextDouble() * 2 - 1;
                }
            }
        }
    }
}