using System;
using System.Text;
using System.Text.RegularExpressions;

public class ObfuscatedProgram
{
    public static void Main(string[] args)
    {
        // --- Obfuscation Techniques ---

        // 1.  String Encoding (Basic)
        string encodedString = "This is a secret message.  It contains sensitive data.";
        string decryptedString = Encoding.UTF8.GetString(Encoding.UTF32.GetBytes(encodedString));
        Console.WriteLine("Decrypted: " + decryptedString);

        // 2.  Regular Expression Substitution (Simple)
        string plainText = "This is a secret message.";
        string replacedText = Regex.Replace(plainText, "secret", "message");
        Console.WriteLine("Replaced: " + replacedText);

        // 3.  String Replacement with a Mask (Basic)
        string maskedText = "This is a secret message.";
        string maskedResult = Regex.Replace(maskedText, "[^a-zA-Z0-9]", "");
        Console.WriteLine("Masked: " + maskedResult);


        // 4.  Variable Naming Obfuscation (Simplified)
        int num = 10;
        string varName = "num";
        string result = "result";
        Console.WriteLine(varName + " = " + num);
        Console.WriteLine(result);

        // 5.  Control Flow Obfuscation (Simple)
        int temp = 10;
        int result2 = temp * 2;
        Console.WriteLine("Result2: " + result2);

        // 6.  Escape Characters
        string escapeCharacter = "!";
        string escapedString = "This is a secret message!";
        Console.WriteLine(escapedString);


        // --- End Obfuscation ---

        Console.WriteLine("Program finished.");
    }
}

