using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SpaceAlertSolver;

namespace Benchmark;

internal sealed class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<AnnealingBenchmark>();
    }
}

[MemoryDiagnoser]
public class AnnealingBenchmark
{
    private readonly Trajectory[] _trajectories = new Trajectory[] { new(0), new(3), new(5), new(2) };
    private readonly Event[] _events = new Event[] { new(true, 1, 1, 17), new(true, 3, 0, 12), new(false, 4, 3, 21), new(true, 6, 1, 15) };

    [Params(100, 1000, 10000)]
    public int Iterations;

    //[Params(0, 1, 2, 3, 4)]
    public int Seed = 0;

    [Benchmark]
    public void RunSimulatedAnnealing()
    {
        SimulatedAnnealing sa = new(5, _trajectories, _events);
        sa.Run(Iterations, _trajectories, _events, seed: Seed, printDebug: false);
    }
}