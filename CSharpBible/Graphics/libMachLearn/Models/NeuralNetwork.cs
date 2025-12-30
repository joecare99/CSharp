using System.Threading.Tasks;

namespace libMachLearn.Models;

public class NeuralNetwork
{
    private Layer[] _layers;
    private double _learningRate;

    public Layer[] Layers => _layers;
    public double LearningRate { get => _learningRate; set => _learningRate = value; }

    public NeuralNetwork(double learningRate, params int[] layerSizes)
    {
        _learningRate = learningRate;
        _layers = new Layer[layerSizes.Length];

        for (int i = 0; i < layerSizes.Length; i++)
        {
            int prevSize = (i == 0) ? 0 : layerSizes[i - 1];
            _layers[i] = new Layer(layerSizes[i], prevSize);
        }
    }



    // Vorhersage berechnen
    public float[] FeedForward(float[] inputs,Func<float, float>? activFunc =null)
    {
        if (activFunc == null) activFunc = Activation.ReLU;
        _layers[0].Neurons = inputs;

        for (int i = 1; i < _layers.Length; i++)
        {
            Layer current = _layers[i];
            Layer previous = _layers[i - 1];

            // Parallelisierung der Neuronen-Berechnung eines Layers
            Parallel.For(0, current.Neurons.Length, j =>
            {
                float sum = current.Biases[j] + LinearAlgebra.VectorDotProduct(previous.Neurons, current.Weights[j]);
                current.Neurons[j] = activFunc(sum)/current.Neurons.Length;
            });
        }
        return _layers[^1].Neurons;
    }

    public void AdjustILWeights(int[] idx, float[] qnt)
    {
        for (int i = 0; i < _layers[1].Weights.Length; i++)
        {
            var Layer = _layers[1].Weights[i];

            Parallel.For(0, _layers[1].Weights[i].Length, (k) =>
            {
                var value = 0.0f;
                for (int j = 0; j < idx.Length; j++)
                    if (k + idx[j] >= 0 && k + idx[j] < _layers[1].Weights[i].Length)
                        value += Layer[k + idx[j]] * qnt[j];
                Layer[k] = value;
            });

            _layers[1].Weights[i] = Layer;
        }
    }

    // Eingabevektor für gegebenes Ziel erzeugen
    public float[] GenerateInputForTarget(float[] targets, int iterations, float inputLearningRate )
    {
        float[] inputs = new float[_layers[0].Neurons.Length];
        float[] gradients = targets;
        for (var l = Layers.Length - 1; l > 0; l--)
        {
            var next = new float[_layers[l-1].Neurons.Length];
            next.Initialize();
            for (var k = 0; k < next.Length; k++)
                for (var n = 0; n < _layers[l].Neurons.Length; n++)
                    next[k] += _layers[l].Weights[n][k] * gradients[n];
            gradients = next;
        }
        var gMax = gradients.Max();
        var gMin = gradients.Min();
        if (gMax> gMin) 
            gradients=gradients.Select(g=> (g - gMin)/ (gMax-gMin)).ToArray();
        return gradients;
    }

    // Training mittels Backpropagation
    public void Train(float[] inputs, float[] targets, float dropOut=0.2f, Func<float, float>? derivative= null)
    {
        if (derivative == null) derivative = Activation.ReLUderivation;

        FeedForward(inputs, Activation.ReLU);
        // 
        _layers[1].ApplyDropout(dropOut);
        // 1. Fehler am Output-Layer berechnen
        Layer outputLayer = _layers[^1];         
        Parallel.For(0, outputLayer.Neurons.Length, i =>
        {
            float error = targets[i] - outputLayer.Neurons[i];
            outputLayer.Deltas[i] = error * derivative(outputLayer.Neurons[i]+ error * 0.5f);
        });

        // 2. Fehler rückwärts durch die Hidden Layers leiten
        for (int i = _layers.Length - 2; i > 0; i--)
        {
            Layer current = _layers[i];
            Layer next = _layers[i + 1];

            Parallel.For(0, current.Neurons.Length, j =>
            {
                // WICHTIG: Wenn das Neuron durch Dropout deaktiviert war, ist sein Fehler 0
                if (!current.DropoutMask[j])
                {
                    current.Deltas[j] = 0;
                }
                else
                {
                    float error = 0;
                    for (int k = 0; k < next.Neurons.Length; k++)
                    {
                        error += next.Deltas[k] * next.Weights[k][j];
                    }
                    current.Deltas[j] = error * derivative(current.Neurons[j]+error *0.5f);
                }
            });
        }

        // Innerhalb von NeuralNetwork.Train, nach der Fehlerberechnung:
        for (int i = 1; i < _layers.Length; i++)
        {
            Layer current = _layers[i];
            Layer previous = _layers[i - 1];

            // Parallel über die Neuronen des aktuellen Layers
            Parallel.For(0, current.Neurons.Length, j =>
            {
                float scale = (float)_learningRate * current.Deltas[j];

                // Vektorisierte Aktualisierung der Gewichte für dieses Neuron
                LinearAlgebra.VectorAddScaled(current.Weights[j], previous.Neurons, scale);

                // Bias ist nur ein einzelner Wert
                current.Biases[j] += scale;
            });
        }
    }
}