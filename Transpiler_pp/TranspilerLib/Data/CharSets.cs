using BaseLib.Helper;
using System.Linq;

namespace TranspilerLib.Data;

/// <summary>
/// Provides reusable character sets used by tokenizers for quick membership checks.
/// </summary>
public static class CharSets
{
    /// <summary>
    /// The set of operator characters.
    /// </summary>
    public static readonly char[] operatorSet = [';', ',', '.', '+', '-', '*', '/', '%', '&', '|', '^', '!', '~', '=', '<', '>', '?', ':', '"', '\'', '\\', '#', '@'];
    /// <summary>
    /// The set of bracket characters: (), {}, [].
    /// </summary>
    public static readonly char[] bracketsSet = ['(', ')', '{', '}', '[', ']'];
    /// <summary>
    /// The set of whitespace characters considered by tokenizers.
    /// </summary>
    public static readonly char[] whitespace = [' ', '\t', '\r', '\n', '\u0000'];
    /// <summary>
    /// The set of decimal digit characters.
    /// </summary>
    public static readonly char[] numbers = '0'.To('9');
    /// <summary>
    /// Extended number characters including exponent and sign.
    /// </summary>
    public static readonly char[] numbersExt = numbers.Concat(['e', '.', '-']).ToArray();
    /// <summary>
    /// Hexadecimal digits (0-9, A-F, a-f).
    /// </summary>
    public static readonly char[] hexNumbers = numbers.Concat('A'.To('F')).Concat('a'.To('f')).ToArray();
    /// <summary>
    /// ASCII letters (A-Z, a-z).
    /// </summary>
    public static readonly char[] letters = 'A'.To('Z').Concat('a'.To('z')).ToArray();
    /// <summary>
    /// Alphanumeric set combining letters and digits.
    /// </summary>
    public static readonly char[] lettersAndNumbers = letters.Concat(numbers).ToArray();
    /// <summary>
    /// All visible characters combining letters, digits, operators, and brackets.
    /// </summary>
    public static readonly char[] allVisible = lettersAndNumbers.Concat(operatorSet).Concat(bracketsSet).ToArray();
}