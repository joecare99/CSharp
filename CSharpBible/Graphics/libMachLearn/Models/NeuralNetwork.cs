using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace libMachLearn.Models;

public class NeuralNetwork
{
    private Layer[] _layers;
    private double _learningRate;

    public class NetworkModelData
    {
        public required int[] LayerSizes { get; set; }
        public float LearningRate { get; set; }
        public required List<LayerData> Layers { get; set; }
    }

    public class LayerData
    {
        public required float[] Biases { get; set; }
        public required float[][] Weights { get; set; } // Gewichte als flaches oder 2D Array
        public eActivation activation { get; set; }
    }

    public Layer[] Layers => _layers;
    public double LearningRate { get => _learningRate; set => _learningRate = value; }

    public NeuralNetwork(double learningRate, int input, params (int size,eActivation eAct)[] layerDef)
    {
        _learningRate = learningRate;
        _layers = new Layer[layerDef.Length+1];
        _layers[0] = new Layer(input, 0);
        for (int i = 0; i < layerDef.Length; i++)
        {
            int prevSize = (i == 0) ? input : layerDef[i - 1].size;
            _layers[i+1] = new Layer(layerDef[i].size, prevSize, layerDef[i].eAct==eActivation.ReLU, 
                layerDef[i].eAct == eActivation.ReLU ? Activation.ReLU : Activation.Sigmoid,
                layerDef[i].eAct == eActivation.ReLU ? Activation.ReLUderivation : Activation.SigmoidDerivative);
        }
    }


    public void SaveModel(string path)
    {
        var modelData = new NetworkModelData
        {
            LayerSizes = this._layers.Select(l => l.Neurons.Length).ToArray(), // Musst du in deiner Klasse speichern
            LearningRate = (float)this._learningRate,
            Layers = _layers.Skip(1).Select(l => new LayerData
            {
                Biases = l.Biases,
                Weights = l.Weights,
                activation = l.activation == Activation.ReLU ? eActivation.ReLU : eActivation.Sigmoid
            }).ToList()
        };

        string json = JsonSerializer.Serialize(modelData, new JsonSerializerOptions { WriteIndented = false });
        File.WriteAllText(path, json);
    }

    public static NeuralNetwork LoadModel(string path)
    {
        string json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<NetworkModelData>(json);

        var nn = new NeuralNetwork(data.LearningRate, data.LayerSizes[0],data.LayerSizes.Zip(data.Layers.Select(l=> l.activation)).Skip(1).ToArray());
        // Hier die Gewichte/Biases aus data.Layers zurück in die nn.Layers kopieren
        for (int i = 1; i < data.LayerSizes.Length; i++)
        {
            nn.Layers[i].Weights = data.Layers[i].Weights;
            nn.Layers[i].Biases = data.Layers[i].Biases;           
        }

        return nn;
    }

    public void SaveBinary(string path)
    {
        using var writer = new BinaryWriter(File.Open(path, FileMode.Create));

        // Header
        writer.Write(_layers.Length);
        foreach (var size in _layers.Select(l => l.Neurons.Length)) writer.Write(size);

        // Daten
        foreach (var layer in _layers.Skip(1))
        {
            foreach (var b in layer.Biases) writer.Write(b);
            foreach (var row in layer.Weights)
                foreach (var w in row) writer.Write(w);
        }
    }

    // Vorhersage berechnen
    public float[] FeedForward_Parallel(float[] inputs)
    {
        _layers[0].Neurons = inputs;

        for (int i = 1; i < _layers.Length; i++)
        {
            Layer current = _layers[i];
            Layer previous = _layers[i - 1];

            // Parallelisierung der Neuronen-Berechnung eines Layers
            Parallel.For(0, current.Neurons.Length, j =>
            {
                float sum = current.Biases[j] + LinearAlgebra.VectorDotProduct(previous.Neurons, current.Weights[j]);
                current.Neurons[j] = current.activation(sum);
            });
        }
        return _layers[^1].Neurons;
    }

    // Vorhersage berechnen
    public float[] FeedForward(float[] inputs)
    {
        _layers[0].Neurons = inputs;

        for (int i = 1; i < _layers.Length; i++)
        {
            Layer current = _layers[i];
            Layer previous = _layers[i - 1];

            // Berechnen der Neuronen-Berechnung eines Layers
            for(var j =0; j< current.Neurons.Length; j ++)
            {
                float sum = 0.0f;
                for (var k = 0; k < previous.Neurons.Length; k++)
                {
                    sum += previous.Neurons[k] * current.Weights[j][k];
                }
                sum += current.Biases[j] ;
                current.Neurons[j] = current.activation(sum );
            };
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
    public float[] GenerateInputForTarget(float[] targets, int iterations, float inputLearningRate)
    {
        float[] inputs = new float[_layers[0].Neurons.Length];
        float[] gradients = targets;
        for (var l = Layers.Length - 1; l > 0; l--)
        {
            var next = new float[_layers[l - 1].Neurons.Length];
            next.Initialize();
            for (var k = 0; k < next.Length; k++)
                for (var n = 0; n < _layers[l].Neurons.Length; n++)
                    next[k] += _layers[l].Weights[n][k] * gradients[n];
            gradients = next;
        }
        var gMax = gradients.Max();
        var gMin = gradients.Min();
        if (gMax > gMin)
            gradients = gradients.Select(g => (g - gMin) / (gMax - gMin)).ToArray();
        return gradients;
    }

    // Training mittels Backpropagation
    public void Train_Parallel(float[] inputs, float[] targets, float dropOut = 0.2f)
    {
        FeedForward_Parallel(inputs);
        // 
        _layers[1].ApplyDropout(dropOut);
        // 1. Fehler am Output-Layer berechnen
        Layer outputLayer = _layers[^1];
        Parallel.For(0, outputLayer.Neurons.Length, i =>
        {
            float error = targets[i] - outputLayer.Neurons[i];
            outputLayer.Deltas[i] = error * outputLayer.actDeriv(outputLayer.Neurons[i]);
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
                    current.Deltas[j] = error * current.actDeriv(current.Neurons[j]);
                }
            });
        }

        // Innerhalb von NeuralNetwork.Train_Parallel, nach der Fehlerberechnung:
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
    public void Train(float[] inputs, float[] targets, float dropOut = 0.2f)
    {
        FeedForward(inputs);
        // 
        _layers[1].ApplyDropout(dropOut);
        // 1. Fehler am Output-Layer berechnen
        Layer outputLayer = _layers[^1];
        for (var i = 0; i < outputLayer.Neurons.Length; i++) 
        {
            float error = targets[i] - outputLayer.Neurons[i];
            outputLayer.Deltas[i] = error * outputLayer.actDeriv(outputLayer.Neurons[i]);
        }
        

        // 2. Fehler rückwärts durch die Hidden Layers leiten
        for (int i = _layers.Length - 2; i > 0; i--)
        {
            Layer current = _layers[i];
            Layer next = _layers[i + 1];

            for(var j=0; j < current.Neurons.Length; j++)
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
                    current.Deltas[j] = error * current.actDeriv(current.Neurons[j] );
                }
            }
        }

        // Innerhalb von NeuralNetwork.Train_Parallel, nach der Fehlerberechnung:
        for (int i = 1; i < _layers.Length; i++)
        {
            Layer current = _layers[i];
            Layer previous = _layers[i - 1];

            // Parallel über die Neuronen des aktuellen Layers
            for(var j =0; j<current.Neurons.Length; j++ )            {
                float scale = (float)_learningRate * current.Deltas[j];

                // Vektorisierte Aktualisierung der Gewichte für dieses Neuron
                for (var k =0; k< previous.Neurons.Length; k++)
                {
                    current.Weights[j][k] += scale * previous.Neurons[k];
                }
                // Bias ist nur ein einzelner Wert
                current.Biases[j] += scale;
            }
        }
    }
}