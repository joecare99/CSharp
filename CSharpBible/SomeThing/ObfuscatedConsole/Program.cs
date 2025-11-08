using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Intentionally obfuscated console program
internal static class P
{
    private static readonly Func<int, int> _f = n => Enumerable.Range(1, n).Aggregate(1, (a, b) => a * b);

    private static string d(string s)
    {
        // XOR obfuscation with rotating key
        byte[] k = { 0x13, 0x37, 0x42, 0x99 };
        var b = Encoding.UTF8.GetBytes(s);
        for (int i = 0; i < b.Length; i++) b[i] ^= k[i % k.Length];
        return Convert.ToBase64String(b);
    }

    private static string e(string b64)
    {
        var b = Convert.FromBase64String(b64);
        byte[] k = { 0x13, 0x37, 0x42, 0x99 };
        for (int i = 0; i < b.Length; i++) b[i] ^= k[i % k.Length];
        return Encoding.UTF8.GetString(b);
    }

    public static void Main(string[] _) // entry point
    {
        // Confusing variable names and dead branches
        var O0 = new[] { d("Hello World!"), d(" from obfuscated app") };
        if (DateTime.UtcNow.Year < 1900) Console.WriteLine("never");

        // Hidden greeting reconstruction
        var m = string.Join("", O0.Select((x, i) => i == 0 ? e(x) : e(x)));
        var t = new string(m.Reverse().Reverse().ToArray());
        Console.WriteLine(t);

        // Regex used in a roundabout way
        var z = Regex.Replace("1,2;3|4", "[^0-9]+", ":");
        var s = z.Split(':').Where(x => x.Length > 0).Select(int.Parse).Sum();
        Console.WriteLine($"Sum={s},5!={_f(5)}");

        // Escapes and misleading encoding
        var a = "Line1\nLine2\t\u263A";
        var bytes = Encoding.UTF8.GetBytes(a);
        var a2 = Encoding.UTF8.GetString(bytes);
        Console.WriteLine(a2);
    }
}