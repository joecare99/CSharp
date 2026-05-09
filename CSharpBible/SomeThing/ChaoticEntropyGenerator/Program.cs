using ChaoticEntropyGenerator;
using System;
using System.Linq;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("==================================================");
        Console.WriteLine(" CHAOTIC ENTROPY STREAM GENERATOR (C# v8)");
        Console.WriteLine("==================================================");

        try
        {
            var generator = new ChaoticEntropyStream();

            // Define how long the stream should run (e.g., 100 points)
            const int PointsToGenerate = 100;
            Console.WriteLine($"\nGenerating {PointsToGenerate} chaotic entropy points...");

            // Generate the data
            var stream = generator.GenerateStream(PointsToGenerate);

            Console.WriteLine("\n--- GENERATED ENTROPY STREAM (First 10 Points) ---");

            // Display the first few generated points
            for (int i = 0; i < Math.Min(10, stream.Count); i++)
            {
                Console.WriteLine($"Point {i + 1}: {stream[i].StringRepresentation}");
            }

            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine("Stream Generation Complete.");
            Console.WriteLine("The sequence is non-repeating and highly dependent on internal state.");

        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: Invalid input provided. {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }
    }
}
