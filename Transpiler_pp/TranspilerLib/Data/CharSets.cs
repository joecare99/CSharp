using BaseLib.Helper;
using System.Linq;

namespace TranspilerLib.Data
{
    public static class CharSets
    {
        /// <summary>
        /// The set of operators    
        /// </summary>
        public static readonly char[] operatorSet = [';', ',', '.', '+', '-', '*', '/', '%', '&', '|', '^', '!', '~', '=', '<', '>', '?', ':', '"', '\'', '\\', '#'];
        /// <summary>
        /// The set of brackets
        /// </summary>
        public static readonly char[] bracketsSet = ['(', ')', '{', '}', '[', ']'];
        /// <summary>
        /// The set of whitespaces
        /// </summary>
        public static readonly char[] whitespace = [' ', '\t', '\r', '\n', '\u0000'];
        /// <summary>
        /// The set of numbers
        /// </summary>
        public static readonly char[] numbers = '0'.To('9');
        /// <summary>
        /// The set of extended numbers
        /// </summary>
        public static readonly char[] numbersExt = numbers.Concat(['e', '.', '-']).ToArray();
        /// <summary>
        /// The hexadecimal numbers/
        /// </summary>
        public static readonly char[] hexNumbers = numbers.Concat('A'.To('F')).Concat('a'.To('f')).ToArray();
        /// <summary>
        /// The set of letters
        /// </summary>
        public static readonly char[] letters = 'A'.To('Z').Concat('a'.To('z')).ToArray();
        /// <summary>
        /// The set of letters and numbers
        /// </summary>
        public static readonly char[] lettersAndNumbers = letters.Concat(numbers).ToArray();
    }
}