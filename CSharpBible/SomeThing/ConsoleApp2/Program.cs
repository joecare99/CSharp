using System;
using System.Text;

public class Program
{
    // --- 1. Obfuscated String Handling (Encryption/Decryption) ---

    // A simple XOR encryption function for demonstration
    public static string XOR(string input, byte key)
    {
        var output = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            output.Append((char)(input[i] ^ key));
        }
        return output.ToString();
    }

    // The secret message, encrypted
    private static string EncryptedMessage = XOR("SecretKey123", 42);


    // --- 2. Obfuscated Core Logic ---

    public static void Main(string[] args)
    {
        // Obfuscated variable names
        string inputData = GetObfuscatedInput();
        string result = ProcessData(inputData);

        // Decrypt the result before displaying (Simulated runtime decryption)
        string finalOutput = DecryptResult(result);

        DisplayResult(finalOutput);
    }

    // Method 1: Obfuscated Input Acquisition
    private static string GetObfuscatedInput()
    {
        // Obfuscated logic: Simulating input retrieval
        Console.WriteLine("--- Obfuscated Input Phase ---");
        Console.Write("Enter a value for processing: ");
        string rawInput = Console.ReadLine();

        if (string.IsNullOrEmpty(rawInput))
        {
            // We use the encrypted message to hide the error message
            Console.WriteLine(XOR(EncryptedMessage, 42));
            return null;
        }
        return rawInput;
    }

    // Method 2: Obfuscated Processing Logic
    private static string ProcessData(string data)
    {
        // Obfuscated arithmetic and variable assignment
        int multiplier = 10;
        int value = 0;

        if (data.Length > 5)
        {
            value = data.Length * multiplier;
        }
        else
        {
            value = data.Length * 2;
        }

        // The value is stored temporarily in a variable named 'tempHash'
        string tempHash = "ProcessedValue: " + value.ToString();

        // Simulating a complex check using conditional nesting
        if (value > 20)
        {
            return tempHash + " [HighScore]";
        }
        else
        {
            return tempHash + " [LowScore]";
        }
    }

    // Method 3: Obfuscated Output Display (Decryption)
    private static string DecryptResult(string encryptedText)
    {
        // Decrypt the output using the same XOR function
        return XOR(encryptedText, 42);
    }

    // Method 4: Obfuscated Display
    private static void DisplayResult(string finalMessage)
    {
        Console.WriteLine("\n===================================");
        Console.WriteLine("Obfuscation Complete!");
        Console.WriteLine("-----------------------------------");
        Console.WriteLine($"Final Result: {finalMessage}");
        Console.WriteLine("===================================");
    }
}
