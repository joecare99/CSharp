using BaseLib.Helper;
using BaseLib.Models.Interfaces;

namespace libMachLearn.Models;

public class Layer
{
    public float[] Neurons;      // Werte der Neuronen in diesem Layer
    public float[] Biases;       // Schwellenwerte
    public float[][] Weights;    // Gewichte zum vorherigen Layer
    public float[] Deltas;       // Fehlerwerte für Backpropagation
    public bool[] DropoutMask;    // Dropaut-Maske
    public Func<float, float> activation;
    public Func<float, float> actDeriv;

    private IRandom _random = IoC.GetRequiredService<IRandom>();

    public Layer(int size, int previousLayerSize, bool xHeInit = false, Func<float, float>? activation=null, Func<float, float>? actDeriv=null)
    {
        Neurons = new float[size];
        Biases = new float[size];
        Deltas = new float[size];
        DropoutMask = new bool[size];
        this.activation = activation ?? Activation.Sigmoid; 
        this.actDeriv = actDeriv ?? Activation.SigmoidDerivative; 

        // Gewichte initialisieren (kleine Zufallswerte)
        if (previousLayerSize > 0)
        {
            Weights = new float[size][];
            for (int i = 0; i < size; i++)
            {
                Weights[i] = new float[previousLayerSize];
                if (xHeInit)
                {
                    float stddev = (float)Math.Sqrt(2.0 / previousLayerSize);
                    Biases[i] = 0.01f;
                    for (int j = 0; j < previousLayerSize; j++)
                    {
                        // Box-Muller für Normalverteilung
                        Weights[i][j] = 
                            (float)Math.Sqrt(-2.0 * Math.Log(1.0 - _random.NextDouble())) * 
                            (float)Math.Sin(2.0 * Math.PI * _random.NextDouble()) * stddev;
                    }
                    continue;
                }
                Biases[i] = (float)_random.NextDouble() * 2f - 1f;
                for (int j = 0; j < previousLayerSize; j++)
                {
                    Weights[i][j] = (float)_random.NextDouble() * 2f - 1f;
                }
            }
        }
    }

    public void ApplyDropout(double dropoutRate)
    {
        DropoutMask = new bool[Neurons.Length];
        Random rnd = new Random();
        for (int i = 0; i < Neurons.Length; i++)
        {
            // Wenn Random < Rate, wird das Neuron auf 0 gesetzt (tot)
            DropoutMask[i] = rnd.NextDouble() > dropoutRate;
            if (!DropoutMask[i])
            {
                Neurons[i] = 0;
            }
            else
            {
                // Skalierung, damit die Gesamtsumme der Aktivierung stabil bleibt
                Neurons[i] /= (1.0f - (float)dropoutRate);
            }
        }
    }
}