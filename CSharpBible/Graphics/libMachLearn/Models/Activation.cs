namespace libMachLearn.Models;

public static class Activation
{
    // Sigmoid: 1 / (1 + e^-x)
    public static double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));

    // Ableitung der Sigmoid-Funktion für das Training (Backpropagation)
    public static double SigmoidDerivative(double x) => x * (1.0 - x);
}