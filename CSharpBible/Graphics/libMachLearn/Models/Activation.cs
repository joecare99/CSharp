namespace libMachLearn.Models;

public static class Activation
{
    // Sigmoid: 1 / (1 + e^-x)
    public static float Sigmoid(float x) => 1.0f / (float)(1.0f + Math.Exp(-x));

    // Ableitung der Sigmoid-Funktion für das Training (Backpropagation)
    public static float SigmoidDerivative(float x) => x * (1.0f - x);

    // Rectified Linear Unit
    public static float ReLU(float x) => Math.Max(0,x);

    public static float ReLUderivation(float x)  => 1.0f;
}