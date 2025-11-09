using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace BenchmarkSuite1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator);
            var _ = BenchmarkRunner.Run(typeof(Program).Assembly, config);
        }
    }
}
