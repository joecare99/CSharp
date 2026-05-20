using System;
using System.Text;
using System.Threading;

public class TunnelSimulator
{
    // --- Obfuscation Layer 1: Encrypted Strings ---
    // In a real scenario, these strings would be encrypted at runtime
    // and decrypted just before use.
    private const string EncryptedTunnelTitle = @"þÂÏäÏÒßÙíËÞÏ";
    private const string HiddenMessage = @"þßÄÄÏÆìÆÅÝãÄÃÞÃËÞÏÎïÄÀÅÓÞÂÏÜÃÏÝ";
    private const string HiddenRamp = "\u008a\u0084\u0090\u0087\u0097\u0081\u0080\u0089\u008fê";


    // Simple XOR Decryption function
    private static string DecryptString(string encrypted, byte key)
    {
        var decrypted = new StringBuilder();
        for (int i = 0; i < encrypted.Length; i++)
        {
            decrypted.Append((char)(encrypted[i] ^ key));
        }
        return decrypted.ToString();
    }

    public static void Main(string[] args)
    {
        // The key used for simple XOR encryption
        const byte ObfuscationKey = 0xAA;

        // Decrypt the strings just before they are used
        string title = DecryptString(EncryptedTunnelTitle, ObfuscationKey);
        string message = DecryptString(HiddenMessage, ObfuscationKey);

        Console.Title = title;
        Console.Clear();

        // --- Core Program Logic (Simulated Flow) ---

        // Simulate a loading or preparation phase with a delay
        Thread.Sleep(1000);

        // Initiate the visual effect
        DrawTunnel(15, 10, 40, 20);

        // Simulate movement and interaction
        for (int i = 0; i < 20; i++)
        {
            // A small pause simulates time passing and movement
            Thread.Sleep(100);

            // In a complex obfuscated program, this loop body would be heavily
            // flattened with jump tables and conditional logic to hide the sequence.
            DrawTunnel(15 + (i % 10), 10, 40, 20);
        }

        Console.WriteLine("\n--- Simulation Complete ---");
        Console.WriteLine(message);
        Console.ReadKey();
    }

    // --- Wow-Effect: Pseudo-3D Tunnel Drawing Function ---
    private static void DrawTunnel(int x, int y, int width, int height)
    {
        double centerX = x + ((width - 1) / 2.0);
        double centerY = y + ((height - 1) / 2.0);
        double halfWidth = width / 2.0;
        double halfHeight = height / 2.0;

        for (int row = y; row < y + height; row++)
        {
            Console.SetCursorPosition(x, row);
            var line = " ";
            for (int col = x; col < x + width; col++)
            {
                double dx = (col - centerX) / halfWidth;
                double dy = (row - centerY) / halfHeight;

                double radius = Math.Sqrt((dx * dx) + (dy * dy));

                // Virtual brightness:
                // brightest near the tunnel wall, darker toward the center and outside.
                double brightness = 1.0 - Math.Clamp(Math.Abs(1.0 - radius) * 3.0, 0.0, 1.0);
                char pixel = GetBrightnessCharacter(brightness);

                line+=pixel;
            }
            Console.Write(line);
        }
    }

    private static readonly string brightnessRamp = DecryptString(HiddenRamp, 0xAA);
    private static char GetBrightnessCharacter(double brightness)
    {

        int index = (int)Math.Round(Math.Clamp(brightness, 0.0, 1.0) * (brightnessRamp.Length - 1));
        return brightnessRamp[index];
    }
}
