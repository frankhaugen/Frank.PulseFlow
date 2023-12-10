using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Frank.PulseFlow.Tests.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        IConfig? config = DefaultConfig.Instance;
        Summary? summary = BenchmarkRunner.Run<Benchmarks>(config, args);

        // Use this to select benchmarks from the console:
        // var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
    }
}