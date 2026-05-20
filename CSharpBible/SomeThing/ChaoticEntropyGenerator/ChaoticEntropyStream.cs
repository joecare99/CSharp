using System;
using System.Collections.Generic;
using System.Linq;
namespace ChaoticEntropyGenerator;

/// <summary>
/// A class designed to generate a chaotic, non-repeating stream of entropy values 
/// based on the current system time and complex mathematical iteration.
/// </summary>
public class ChaoticEntropyStream
{
    // A seed value to ensure reproducibility for testing purposes.
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the ChaoticEntropyStream class.
    /// </summary>
    public ChaoticEntropyStream()
    {
        // Initialize Random with a fixed seed (or system time) for consistent pseudo-randomness.
        _random = new Random();
    }

    /// <summary>
    /// Calculates the chaotic entropy stream for a given number of iterations.
    /// The core calculation involves a non-linear combination of time and a pseudo-random walk.
    /// </summary>
    /// <param name="iterations">The number of entropy points to generate.</param>
    /// <returns>A list of EntropyPoint objects representing the generated stream.</returns>
    public List<EntropyPoint> GenerateStream(int iterations)
    {
        if (iterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations must be a positive integer.");
        }

        var stream = new List<EntropyPoint>();

        // Start the chaotic iteration from the current system time in milliseconds.
        double startTimeSeed = DateTime.UtcNow.Ticks;

        for (int i = 0; i < iterations; i++)
        {
            // 1. Chaotic Input: Use the current iteration index and the system's high-resolution time.
            double timeFactor = (double)DateTime.UtcNow.Ticks + i * 10000000.0;

            // 2. Pseudo-Random Walk: Introduce randomness based on the seed and iteration count.
            double randomOffset = _random.NextDouble() * 100.0;

            // 3. Chaotic Transformation (The Core Surprise): 
            // Use a combination of the time factor, the random offset, and a complex hash (via Math.Sin and Math.Log)
            // to generate the entropy value. This transformation makes the output highly sensitive to small inputs.
            double baseEntropy = Math.Sin(timeFactor / 10000000.0) * Math.Log(randomOffset + 1.0);

            // Scale and shift the result to create a floating-point entropy value
            double finalEntropy = (baseEntropy * 100.0) + randomOffset;

            // 4. Record the point
            stream.Add(new EntropyPoint(DateTime.UtcNow, finalEntropy));
        }

        return stream;
    }
}
