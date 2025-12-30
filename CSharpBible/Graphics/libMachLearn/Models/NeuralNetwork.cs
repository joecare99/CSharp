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
    public double[] FeedForward(double[] inputs)
    {
        _layers[0].Neurons = inputs;

        for (int i = 1; i < _layers.Length; i++)
        {
            Layer current = _layers[i];
            Layer previous = _layers[i - 1];

            for (int j = 0; j < current.Neurons.Length; j++)
            {
                double sum = current.Biases[j];
                for (int k = 0; k < previous.Neurons.Length; k++)
                {
                    sum += current.Weights[j][k] * previous.Neurons[k];
                }
                current.Neurons[j] = Activation.Sigmoid(sum);
            }
        }
        return _layers[^1].Neurons;
    }

    public void AdjustILWeights(int[] idx, double[] qnt)
    {
        for (int i = 0; i < _layers[1].Weights.Length; i++)
        {
            var Layer = _layers[1].Weights[i];

            Parallel.For(0, _layers[1].Weights[i].Length, (k) =>
            {
                var value = 0.0d;
                for (int j = 0; j < idx.Length; j++)
                    if (k + idx[j] >= 0 && k + idx[j] < _layers[1].Weights[i].Length)
                        value += Layer[k + idx[j]] * qnt[j];
                Layer[k] = value;
            });

            _layers[1].Weights[i] = Layer;
        }
    }

    // Eingabevektor für gegebenes Ziel erzeugen
    public double[] GenerateInputForTarget(double[] targets, int iterations, double inputLearningRate)
    {
        double[] inputs = new double[_layers[0].Neurons.Length];
        double[] inputGradients = new double[_layers[0].Neurons.Length];

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            FeedForward(inputs);

            Layer outputLayer = _layers[^1];
            Parallel.For(0, outputLayer.Neurons.Length, i =>
            {
                double error = targets[i] - outputLayer.Neurons[i];
                outputLayer.Deltas[i] = error * Activation.SigmoidDerivative(outputLayer.Neurons[i]);
            });

            for (int i = _layers.Length - 2; i > 0; i--)
            {
                Layer current = _layers[i];
                Layer next = _layers[i + 1];

                Parallel.For(0, current.Neurons.Length, j =>
                {
                    double error = 0;
                    for (int k = 0; k < next.Neurons.Length; k++)
                    {
                        error += next.Deltas[k] * next.Weights[k][j];
                    }
                    current.Deltas[j] = error * Activation.SigmoidDerivative(current.Neurons[j]);
                });
            }

            Layer firstHidden = _layers.Length > 1 ? _layers[1] : null;
            if (firstHidden == null)
            {
                return inputs;
            }

            Parallel.For(0, _layers[0].Neurons.Length, j =>
            {
                double gradient = 0;
                for (int k = 0; k < firstHidden.Neurons.Length; k++)
                {
                    gradient += firstHidden.Deltas[k] * firstHidden.Weights[k][j];
                }
                inputGradients[j] = gradient;
            });

            for (int j = 0; j < inputs.Length; j++)
            {
                inputs[j] += inputLearningRate * inputGradients[j];
            }
        }

        return inputs;
    }

    // Training mittels Backpropagation
    public void Train(double[] inputs, double[] targets)
    {
        FeedForward(inputs);

        // 1. Fehler am Output-Layer berechnen
        Layer outputLayer = _layers[^1];
        Parallel.For(0, outputLayer.Neurons.Length, i =>
        {
            double error = targets[i] - outputLayer.Neurons[i];
            outputLayer.Deltas[i] = error * Activation.SigmoidDerivative(outputLayer.Neurons[i]);
        });

        // 2. Fehler rückwärts durch die Hidden Layers leiten
        for (int i = _layers.Length - 2; i > 0; i--)
        {
            Layer current = _layers[i];
            Layer next = _layers[i + 1];

            Parallel.For(0, current.Neurons.Length, j =>
            {
                double error = 0;
                for (int k = 0; k < next.Neurons.Length; k++)
                {
                    error += next.Deltas[k] * next.Weights[k][j];
                }
                current.Deltas[j] = error * Activation.SigmoidDerivative(current.Neurons[j]);
            });
        }

        // 3. Gewichte und Biases aktualisieren (Gradient Descent)
        for (int i = 1; i < _layers.Length; i++)
        {
            Layer current = _layers[i];
            Layer previous = _layers[i - 1];

            Parallel.For(0, current.Neurons.Length, j =>
            {
                for (int k = 0; k < previous.Neurons.Length; k++)
                {
                    current.Weights[j][k] += _learningRate * current.Deltas[j] * previous.Neurons[k];
                }
                current.Biases[j] += _learningRate * current.Deltas[j];
            });
        }
    }
}