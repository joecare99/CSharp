using BaseLib.Helper;
using BaseLib.Models;
using libMachLearn.Models;

namespace libMachLearn.Tests;

[TestClass]
public sealed class NeuralNetworkTests
{
    [TestInitialize]
    public void InitializeServices()
    {
        IoC.GetReqSrv = type => type == typeof(BaseLib.Models.Interfaces.IRandom)
            ? new CRandom()
            : throw new InvalidOperationException($"Unexpected service: {type}");
    }

    [TestMethod]
    public void FeedForward_AndParallelFeedForward_ProduceEquivalentResults()
    {
        NeuralNetwork sequential = CreateDeterministicNetwork();
        NeuralNetwork parallel = CreateDeterministicNetwork();

        float[] sequentialResult = sequential.FeedForward([0.25f, -0.5f]);
        float[] parallelResult = parallel.FeedForward_Parallel([0.25f, -0.5f]);

        Assert.HasCount(1, sequentialResult);
        Assert.AreEqual(sequentialResult[0], parallelResult[0], 0.000001f);
    }

    [TestMethod]
    public void SaveModel_ThenLoadModel_PreservesNetworkState()
    {
        NeuralNetwork original = CreateDeterministicNetwork();
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");

        try
        {
            original.SaveModel(path);
            NeuralNetwork loaded = NeuralNetwork.LoadModel(path);

            Assert.AreEqual(original.LearningRate, loaded.LearningRate, 0.000001);
            Assert.AreEqual(original.Layers.Length, loaded.Layers.Length);
            for (int layerIndex = 1; layerIndex < original.Layers.Length; layerIndex++)
            {
                CollectionAssert.AreEqual(original.Layers[layerIndex].Biases, loaded.Layers[layerIndex].Biases);
                for (int neuronIndex = 0; neuronIndex < original.Layers[layerIndex].Weights.Length; neuronIndex++)
                {
                    CollectionAssert.AreEqual(
                        original.Layers[layerIndex].Weights[neuronIndex],
                        loaded.Layers[layerIndex].Weights[neuronIndex]);
                }
            }
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [TestMethod]
    public void Train_WithNoDropout_UpdatesOutputParameters()
    {
        NeuralNetwork network = CreateDeterministicNetwork();
        float initialBias = network.Layers[^1].Biases[0];

        network.Train([1f, 0f], [1f], dropOut: 0f);

        Assert.AreNotEqual(initialBias, network.Layers[^1].Biases[0]);
    }

    private static NeuralNetwork CreateDeterministicNetwork()
    {
        NeuralNetwork network = new(0.1, 2, (1, eActivation.Sigmoid));
        network.Layers[1].Weights[0] = [0.5f, -0.25f];
        network.Layers[1].Biases[0] = 0.1f;
        return network;
    }
}
