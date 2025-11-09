using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace NebelEbook.Benchmarks
{
    [MemoryDiagnoser]
    public class BuildOutputFileNameBenchmark
    {
        private Func<string?, string, string>? _build;
        private string[] _titles = Array.Empty<string>();
        private string _key = "html";

        [GlobalSetup]
        public void Setup()
        {
            var mi = typeof(Program).GetMethod("BuildOutputFileNameFromTitle", BindingFlags.Static | BindingFlags.NonPublic);
            if (mi == null)
                throw new InvalidOperationException("Methode BuildOutputFileNameFromTitle nicht gefunden.");

            _build = (Func<string?, string, string>)mi.CreateDelegate(typeof(Func<string?, string, string>));

            _titles = new[]
            {
                "Nebel über Bretten",
                " A  very..long___title -- with invalid:chars*?<>| and dots... ",
                "CON",
                "LPT1",
                new string('ä', 200),
                "Résumé de l'été à São Paulo",
                "?? ?? ??",
            };

            _key = "html";
        }

        [Benchmark]
        public string Run_AllSamples()
        {
            string last = string.Empty;
            foreach (var t in _titles)
            {
                last = _build!(t, _key);
            }
            return last;
        }

        [Benchmark]
        public string Run_ShortAscii()
            => _build!("Hello World", _key);

        [Benchmark]
        public string Run_LongDiacritics()
            => _build!(new string('ä', 500), _key);
    }
}
