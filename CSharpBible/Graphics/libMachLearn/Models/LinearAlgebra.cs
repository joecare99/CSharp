using System.Numerics;

namespace libMachLearn.Models;

public static class LinearAlgebra
{
    public static float VectorDotProduct(float[] vecA, float[] vecB)
    {
        int i = 0;
        int vCount = Vector<float>.Count; // Anzahl der Floats pro Register (z.B. 8 bei AVX2)
        var sumVector = Vector<float>.Zero;

        // SIMD-Schleife: Verarbeitet vCount Elemente gleichzeitig
        for (; i <= vecA.Length - vCount; i += vCount)
        {
            var va = new Vector<float>(vecA, i);
            var vb = new Vector<float>(vecB, i);
            sumVector += va * vb;
        }

        // Ergebnisse des Vektors zusammenführen (Horizontal Sum)
        float totalSum = Vector.Dot(sumVector, Vector<float>.One);

        // Den Rest verarbeiten, falls die Länge nicht durch vCount teilbar ist
        for (; i < vecA.Length; i++)
        {
            totalSum += vecA[i] * vecB[i];
        }

        return totalSum;
    }

    public static void VectorAddScaled(float[] weights, float[] inputs, float scale)
    {
        int i = 0;
        int vCount = Vector<float>.Count;
        var vScale = new Vector<float>(scale); // Der Skalar (learningRate * delta)

        // SIMD-Schleife
        for (; i <= weights.Length - vCount; i += vCount)
        {
            var vW = new Vector<float>(weights, i);
            var vI = new Vector<float>(inputs, i);

            // W = W + (Scale * I)
            var vResult = Vector.Add(vW, Vector.Multiply(vScale, vI));
            vResult.CopyTo(weights, i);
        }

        // Rest verarbeiten
        for (; i < weights.Length; i++)
        {
            weights[i] += scale * inputs[i];
        }
    }
}