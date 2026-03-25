using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ObfuscatedConsole
{
    /// <summary>
    /// Horribly over-engineered, multi-layer obfuscated README printer.
    /// Intentionally not a simple "File.ReadAllText + WriteLine".
    /// </summary>
    internal static class ReadmePrinter
    {
        // Step 1: raw bytes of "SomeThing/ReadMe.md" are never stored directly.
        // Instead we store multiple chunks, each xor+base64+reverse+rot13 encoded.

        private static readonly string[] _chunks =
        {
            // Each string is: rot13(base64(xor(originalChunk))) reversed.
            // The content is generated once and then never touched by humans again.
            "==5vVQG0lVQ/NUUgV1E2c3NvVklTM1JZb0hHVkNnPT0=", // placeholder-like noise
            "=xZ1VHJ3Y3B1U1ZFSHdZQWlUSFJES2VlTkN3PT0=",
            "=t9bU2h5Wldsb1ZrU2dZQWlUQ1ZGT3lZbkJ3PT0=",
            "=0GCaAxIUu0oMW1ZGyxIiA3p2R1ItHHA/DIy0TDIi5=="
        };

        // Single source of pseudo-randomness, but seeded from something ridiculous.
        private static readonly Random _rnd = new Random(Environment.MachineName.Sum(c => c) ^ DateTime.Now.DayOfYear);

        // Public entry point used by Program.Main (but not obviously).
        public static void Invoke()
        {
            // Layer 1: Decide at runtime whether we "pretend" to load from disk.
            var mode = ChooseMode();
            IEnumerable<string> lines = mode switch
            {
                PrintMode.FileSystemWhenAvailable => TryReadFromFileSystem(),
                PrintMode.InMemoryReconstruction => ReconstructFromChunks(),
                PrintMode.NoiseThenReconstruction => EmitNoiseThenReconstruct(),
                _ => ReconstructFromChunks(),
            };

            // Layer 2: Randomized line shuffler that actually restores the original order.
            lines = DeterministicallyUnscramble(lines.ToArray());

            // Layer 3: Random decorator that may or may not add comments around lines.
            foreach (var rendered in DecorateLines(lines))
            {
                Console.WriteLine(rendered);
            }
        }

        private static PrintMode ChooseMode()
        {
            // 0 / 1 / 2 selected in a confusing way.
            var r = _rnd.Next();
            var bucket = (r ^ (r >> 3)) % 7; // 0..6, but we only care about 0..2
            if (bucket < 0) bucket = -bucket;
            return bucket switch
            {
                0 => PrintMode.FileSystemWhenAvailable,
                1 => PrintMode.NoiseThenReconstruction,
                _ => PrintMode.InMemoryReconstruction,
            };
        }

        private static IEnumerable<string> TryReadFromFileSystem()
        {
            // Tries a few relative paths. If all fail, silently falls back to reconstruction.
            var candidates = new[]
            {
                Path.Combine("SomeThing", "ReadMe.md"),
                Path.Combine("..", "SomeThing", "ReadMe.md"),
                "ReadMe.md" // just in case it is copied
            };

            foreach (var c in candidates)
            {
                try
                {
                    if (File.Exists(c))
                    {
                        return File.ReadAllLines(c, Encoding.UTF8);
                    }
                }
                catch
                {
                    // Swallow everything and continue. This is intentionally unhelpful.
                }
            }

            return ReconstructFromChunks();
        }

        private static IEnumerable<string> EmitNoiseThenReconstruct()
        {
            // Emit some fake corruption lines first.
            for (int i = 0; i < 3; i++)
            {
                yield return $"# [checksum mismatch {i:X}] attempting recovery…";
            }

            foreach (var l in ReconstructFromChunks())
            {
                yield return l;
            }
        }

        private static IEnumerable<string> ReconstructFromChunks()
        {
            // Step 1: decode each chunk and concatenate; assume newline separators embedded.
            var builder = new StringBuilder();
            foreach (var chunk in _chunks)
            {
                builder.Append(DecodeChunk(chunk));
            }

            var combined = builder.ToString();

            // Step 2: simulate weird line separators.
            var normalized = combined
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\u2028", "\n"); // line separator we probably never use

            foreach (var line in normalized.Split(new[] { '\n' }, StringSplitOptions.None))
            {
                yield return line;
            }
        }

        private static string DecodeChunk(string pseudo)
        {
            // Reverse -> un-rot13 -> base64-decode -> xor-decrypt -> UTF8
            var reversed = new string(pseudo.Reverse().ToArray());
            var unrot = Rot13(reversed);
            var b64 = Encoding.ASCII.GetString(Convert.FromBase64String(unrot));

            var obfuscated = Convert.FromBase64String(b64);
            var key = GetKeyMaterial();
            for (int i = 0; i < obfuscated.Length; i++)
            {
                obfuscated[i] ^= key[i % key.Length];
            }

            return Encoding.UTF8.GetString(obfuscated);
        }

        static string EncodeChunk(string plain)
        {
            // UTF8 -> XOR -> base64 -> rot13 -> reverse
            var key = GetKeyMaterial();

            var bytes = Encoding.UTF8.GetBytes(plain);
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] ^= key[i % key.Length];

            var b64 = Convert.ToBase64String(bytes);
            var rot = Rot13(b64);
            var reversed = new string(rot.Reverse().ToArray());
            return reversed;
           }

        private static byte[] GetKeyMaterial()
        {
            // Horrible misuse of RNG and hashes to compute a static key.
            using var sha = SHA256.Create();
            var seed = Encoding.UTF8.GetBytes(Environment.UserName + "|" + typeof(ReadmePrinter).FullName);
            var hash = sha.ComputeHash(seed);

            // Fold hash down to 8 bytes as XOR key.
            var key = new byte[8];
            for (int i = 0; i < hash.Length; i++)
            {
                key[i % key.Length] ^= hash[i];
            }
            return key;
        }

        private static IEnumerable<string> DeterministicallyUnscramble(IReadOnlyList<string> original)
        {
            // Shuffle indices with a PRNG, then immediately un-shuffle.
            int n = original.Count;
            var indices = Enumerable.Range(0, n).ToArray();
            var shuffled = (int[])indices.Clone();

            for (int i = shuffled.Length - 1; i > 0; i--)
            {
                int j = _rnd.Next(i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }

            var inverse = new int[n];
            for (int i = 0; i < n; i++) inverse[shuffled[i]] = i;

            for (int i = 0; i < n; i++)
            {
                yield return original[inverse[i]];
            }
        }

        private static IEnumerable<string> DecorateLines(IEnumerable<string> lines)
        {
            int lineNumber = 0;
            foreach (var l in lines)
            {
                lineNumber++;

                // Occasionally insert a bogus comment or blank line.
                if (lineNumber % 13 == 0)
                {
                    yield return $"<!-- line {lineNumber} mysteriously skipped -->";
                    continue;
                }

                if (lineNumber % 7 == 0)
                {
                    yield return $"<!-- {ComputeFakeChecksum(l)} -->";
                }

                yield return l;
            }
        }

        private static string ComputeFakeChecksum(string line)
        {
            unchecked
            {
                int h = 17;
                foreach (var c in line)
                {
                    h = h * 31 + c;
                }
                return $"cs:{(h & 0xFFFF):X4}";
            }
        }

        private static string Rot13(string input)
        {
            var buffer = input.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                char c = buffer[i];
                if (c is >= 'a' and <= 'z')
                {
                    buffer[i] = (char)('a' + (c - 'a' + 13) % 26);
                }
                else if (c is >= 'A' and <= 'Z')
                {
                    buffer[i] = (char)('A' + (c - 'A' + 13) % 26);
                }
            }

            return new string(buffer);
        }

        private enum PrintMode
        {
            FileSystemWhenAvailable,
            InMemoryReconstruction,
            NoiseThenReconstruction
        }
    }
}
